// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

[Collection("MessageOnlyExecutor")]
public class ClipboardNotifierTests
{
    [Fact]
    public async Task Dispose_CalledTwice_NoException()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        executor.Invoke(() =>
        {
            var notifier = new ClipboardNotifier(executor.Window!);

            notifier.Dispose();
            notifier.Dispose();
        });
    }

    [Fact]
    public async Task Dispose_ThenReattached_ListenerCanBeAddedAgain()
    {
        using var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        executor.Invoke(() =>
        {
            var first = new ClipboardNotifier(executor.Window!);
            first.Dispose();

            using var second = new ClipboardNotifier(executor.Window!);
        });
    }

    [Fact]
    public async Task ExecutorDisposed_AfterNotifierDisposed_NoException()
    {
        var executor = new MessageOnlyExecutor();

        await executor.StartAsync();

        executor.Invoke(() => new ClipboardNotifier(executor.Window!).Dispose());

        executor.Dispose();
    }
}
