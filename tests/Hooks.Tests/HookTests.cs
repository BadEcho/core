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

using BadEcho.Hooks.Interop;
using BadEcho.Interop;

namespace BadEcho.Hooks.Tests;

[CollectionDefinition("HookTestsCollection", DisableParallelization = true)]
public class HookTestsCollectionDefinition;

[Collection("HookTestsCollection")]
public class HookTests
{
    private const HookType HOOK_TYPE = HookType.CallWindowProcedure;
    private const int MAX_THREADS = 20;

    public HookTests()
    {   // Required for test runner to see BadEcho.Hooks.Native.dll.
        Kernel32.AddDllDirectory(Environment.CurrentDirectory);
    }

    [Fact]
    public async Task AddRemoveHook_OneThread_ReturnsTrue()
    {
        using var pump = new MessageOnlyExecutor();

        await pump.StartAsync();
        Assert.NotNull(pump.Window);

        var process = NativeProcesses.Create(1)[0];

        try
        {
            int threadId = process.Threads[0].Id;

            Assert.True(Native.AddHook(HOOK_TYPE, pump.Window.Handle, threadId));
            Assert.True(Native.RemoveHook(HOOK_TYPE, threadId));
        }
        finally
        {
            process.Kill();
        }
    }
        
    [Fact]
    public async Task AddRemoveHook_MoreThanMaxThreads_ReturnsFalse()
    {
        using var pump = new MessageOnlyExecutor();

        await pump.StartAsync();
        Assert.NotNull(pump.Window);

        var processes = NativeProcesses.Create(MAX_THREADS + 1);
            
        try
        {
            for (int i = 0; i < MAX_THREADS; i++)
            {
                int threadId = processes[i].Threads[0].Id;

                Assert.True(Native.AddHook(HOOK_TYPE, pump.Window.Handle, threadId));
            }

            int lastThreadId = processes[MAX_THREADS].Threads[0].Id;

            Assert.False(Native.AddHook(HOOK_TYPE, pump.Window.Handle, lastThreadId));
        }
        finally
        {
            foreach (var process in processes)
            {
                Native.RemoveHook(HOOK_TYPE, process.Threads[0].Id);
                process.Kill();
            }
        }
    }

    [Fact]
    public async Task AddHookThenRemoveHook_MoreThanMaxThreads_ReturnsTrue()
    {
        using var pump = new MessageOnlyExecutor();

        await pump.StartAsync();
        Assert.NotNull(pump.Window);

        var processes = NativeProcesses.Create(MAX_THREADS + 1);
        int lastThreadId = processes[MAX_THREADS].Threads[0].Id;

        try
        {
            for (int i = 0; i < MAX_THREADS; i++)
            {
                int threadId = processes[i].Threads[0].Id;

                Assert.True(Native.AddHook(HOOK_TYPE, pump.Window.Handle, threadId));
                Assert.True(Native.RemoveHook(HOOK_TYPE, threadId));

            }

            Assert.True(Native.AddHook(HOOK_TYPE, pump.Window.Handle, lastThreadId));
        }
        finally
        {
            Native.RemoveHook(HOOK_TYPE, lastThreadId);

            foreach (var process in processes)
            {
                process.Kill();
            }
        }
    }
}
