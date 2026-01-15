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
using BadEcho.Hooks.Interop;

namespace BadEcho.Hooks;

/// <summary>
/// Provides a publisher of messages being read from a message queue.
/// </summary>
public sealed class MessageQueueSource : HookSource<GetMessageProcedure>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageQueueSource"/> class.
    /// </summary>
    /// <param name="threadId">The identifier for the thread whose message queue we're hooking into.</param>
    public MessageQueueSource(int threadId)
        : base(HookType.GetMessage, threadId)
    { }

    /// <inheritdoc/>
    protected override void OnHookEvent(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        uint localMsg = msg;
        IntPtr localWParam = wParam;
        IntPtr localLParam = lParam;

        foreach (GetMessageProcedure callback in Callbacks)
        {
            var result = callback(ref localMsg, ref localWParam, ref localLParam);

            if (result.Handled)
                break;
        }

        if (localMsg != msg || localWParam != wParam || localLParam != lParam)
            Native.ChangeMessageDetails(localMsg, localWParam, localLParam);
    }
}
