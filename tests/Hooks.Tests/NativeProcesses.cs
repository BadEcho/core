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
using System.Diagnostics;

namespace BadEcho.Hooks.Tests;

internal class NativeProcesses
{
    public static IList<Process> Create(int processCount)
    {
        var processes = new List<Process>();

        for (int i = 0; i < processCount; i++)
        {
            var process = new Process();

            process.StartInfo.FileName = "BadEcho.NativeTestApp.exe";
            process.Start();
            process.WaitForInputIdle();
            processes.Add(process);
        }

        return processes;
    }

    public static (nint Window, int ThreadId) GetWindowInformation(Process process)
    {
        nint window = nint.Zero;
        int threadId = 0;

        User32.EnumWindows(Callback, new IntPtr(process.Id));

        return (window, threadId);

        bool Callback(nint hWnd, nint lParam)
        {
            threadId = (int) User32.GetWindowThreadProcessId(hWnd, out uint processId);

            if (processId == (uint)lParam)
            {
                window = hWnd;
                return false;
            }

            return true;
        }
    }
}
