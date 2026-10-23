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
using System.Reflection;

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

    [Fact]
    public async Task MouseSource_WheelHookMessage_DataAndCoordinatesReceived()
    {
        var process = NativeProcesses.Create(1)[0];

        try
        {
            (nint _, int threadId) = NativeProcesses.GetWindowInformation(process);
            bool receivedWheel = false;
            int receivedX = 0, receivedY = 0, receivedData = 0;

            await using (var source = new MouseSource(MouseProcedure, threadId))
            {
                await source.StartAsync();

                MessageOnlyWindowWrapper window = GetHookWindow(source);

                // Stands in for the native hook procedure: mouse data in the WPARAM and packed coordinates in the LPARAM.
                User32.SendMessage(window.Handle,
                                   (WindowMessage)((uint)WindowMessage.User + (uint)WindowMessage.MouseWheel),
                                   MakeParam(0, -240),
                                   MakeParam(-5, 300));
            }

            Assert.True(receivedWheel);
            Assert.Equal(-5, receivedX);
            Assert.Equal(300, receivedY);
            Assert.Equal(-240, receivedData);

            ProcedureResult MouseProcedure(MouseEvent mouseEvent, int x, int y, int data)
            {
                if (mouseEvent == MouseEvent.Wheel)
                {
                    receivedWheel = true;
                    receivedX = x;
                    receivedY = y;
                    receivedData = data;
                }

                return new ProcedureResult(nint.Zero, true);
            }
        }
        finally
        {
            process.Kill();
        }
    }

    private static MessageOnlyWindowWrapper GetHookWindow(HookSource source)
    {   // The window receiving hook messages isn't exposed, so it's read from the source's private executor.
        FieldInfo? executorField
            = typeof(HookSource).GetField("_hookExecutor", BindingFlags.Instance | BindingFlags.NonPublic);

        var executor = (MessageOnlyExecutor?)executorField?.GetValue(source);

        Assert.NotNull(executor);
        Assert.NotNull(executor.Window);

        return executor.Window;
    }

    private static nint MakeParam(int low, int high)
        => unchecked((nint)(uint)((ushort)low | ((ushort)high << 16)));

    public void Dispose()
    {
        _mre.Dispose();
    }
}
