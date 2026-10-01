// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Interop;

namespace BadEcho.Hooks.Tests;

public class MessageTests : IDisposable
{
    // WM_APP + 1: a message only these tests send, so the callbacks can recognize it.
    private const uint TEST_MESSAGE = 0x8001;

    private readonly ManualResetEventSlim _mre = new();

    public MessageTests()
    {   // Required for test runner to see BadEcho.Hooks.Native.dll.
        Kernel32.AddDllDirectory(Environment.CurrentDirectory);
    }

    [Fact]
    public async Task MessageQueueSource_PostActivate_MessageReceived()
    {
        var process = NativeProcesses.Create(1)[0];

        try
        {
            (nint processWindow, int threadId) = NativeProcesses.GetWindowInformation(process);
            bool receivedActivate = false;
            
            await using (var source = new MessageQueueSource(GetMessage, threadId))
            {
                await source.StartAsync();
                
                User32.PostMessage(processWindow, WindowMessage.Activate, new IntPtr(1), processWindow);
                _mre.Wait(TimeSpan.FromSeconds(3));
            }

            Assert.True(receivedActivate);

            ProcedureResult GetMessage(ref uint msg, ref IntPtr wParam, ref IntPtr lParam)
            {
                if ((WindowMessage) msg == WindowMessage.Activate)
                {
                    receivedActivate = true;
                    _mre.Set();
                }

                return new ProcedureResult(IntPtr.Zero, true);
            }
        }
        finally
        {
            process.Kill();
        }
    }
    
    [Fact]
    public async Task ExternalWindowWrapper_SendActivate_MessageReceived()
    {
        var process = NativeProcesses.Create(1)[0];

        try
        {
            (nint processWindow, int _) = NativeProcesses.GetWindowInformation(process);
            bool receivedActivate = false;

            var handle = new WindowHandle(processWindow, false);

            using (var wrapper = await ExternalWindowWrapper.CreateAsync(handle))
            {
                wrapper.AddCallback(WindowProcedure);

                User32.SendMessage(processWindow, WindowMessage.Activate, new nint(1), processWindow);
                _mre.Wait(TimeSpan.FromSeconds(3));
            }

            Assert.True(receivedActivate);

            ProcedureResult WindowProcedure(nint hWnd, uint msg, nint wParam, nint lParam)
            {
                if ((WindowMessage)msg == WindowMessage.Activate)
                {
                    receivedActivate = true;
                    _mre.Set();
                }

                return new ProcedureResult(nint.Zero, true);
            }
        }
        finally
        {
            process.Kill();
        }
    }

    [Fact]
    public async Task MessageQueueSource_ChangeToClose_TargetWindowCloses()
    {
        var process = NativeProcesses.Create(1)[0];

        try
        {
            (nint processWindow, int threadId) = NativeProcesses.GetWindowInformation(process);

            await using (var source = new MessageQueueSource(GetMessage, threadId))
            {
                await source.StartAsync();

                User32.PostMessage(processWindow, (WindowMessage)TEST_MESSAGE, nint.Zero, nint.Zero);

                // If the app closes from this message, that means our swap to WM_CLOSE was successful.
                Assert.True(process.WaitForExit(3000));
            }

            static ProcedureResult GetMessage(ref uint msg, ref nint wParam, ref nint lParam)
            {
                if (msg == TEST_MESSAGE)
                    msg = (uint)WindowMessage.Close;

                return new ProcedureResult(nint.Zero, true);
            }
        }
        finally
        {
            if (!process.HasExited)
                process.Kill();
        }
    }

    [Fact]
    public async Task MessageQueueSource_NoChange_TargetWindowStaysOpen()
    {
        var process = NativeProcesses.Create(1)[0];

        try
        {
            (nint processWindow, int threadId) = NativeProcesses.GetWindowInformation(process);
            bool receivedTestMessage = false;

            await using (var source = new MessageQueueSource(GetMessage, threadId))
            {
                await source.StartAsync();

                User32.PostMessage(processWindow, (WindowMessage)TEST_MESSAGE, nint.Zero, nint.Zero);
                _mre.Wait(TimeSpan.FromSeconds(3));

                Assert.True(receivedTestMessage);
                Assert.False(process.WaitForExit(500));
            }

            ProcedureResult GetMessage(ref uint msg, ref nint wParam, ref nint lParam)
            {
                if (msg == TEST_MESSAGE)
                {
                    receivedTestMessage = true;
                    _mre.Set();
                }

                return new ProcedureResult(nint.Zero, true);
            }
        }
        finally
        {
            if (!process.HasExited)
                process.Kill();
        }
    }
    public void Dispose()
    {
        _mre.Dispose();
    }
}
