// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
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

public class MessageQueueSourceTests : IDisposable
{
    private readonly ManualResetEventSlim _mre = new();

    public MessageQueueSourceTests()
    {   // Required for test runner to see BadEcho.Hooks.Native.dll.
        Kernel32.AddDllDirectory(Environment.CurrentDirectory);
    }

    [Fact]
    public async Task SendMessage_HookedNativeTestApp_MessageReceived()
    {
        var process = NativeProcesses.Create(1)[0];

        try
        {
            (nint processWindow, int threadId) = NativeProcesses.GetWindowInformation(process);
            bool receivedActivate = false;
            
            Assert.NotEqual(IntPtr.Zero, processWindow);
            Assert.NotEqual(0, threadId);

            using (var source = new MessageQueueSource(threadId))
            {
                source.AddCallback(GetMessage);
                await Task.Delay(1000);
                
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

    public void Dispose()
    {
        _mre.Dispose();
    }
}
