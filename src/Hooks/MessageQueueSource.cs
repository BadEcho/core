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

using BadEcho.Hooks.Interop;

namespace BadEcho.Hooks;

/// <summary>
/// Provides a publisher of messages being read from a message queue.
/// </summary>
public sealed class MessageQueueSource : HookSource
{
    private readonly GetMessageProcedure _callback;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageQueueSource"/> class.
    /// </summary>
    /// <param name="callback">The delegate that will be executed when a hook event has occured.</param>
    /// <param name="threadId">The identifier for the thread whose message queue we're hooking into.</param>
    public MessageQueueSource(GetMessageProcedure callback, int threadId)
        : base(HookType.GetMessage, threadId)
    {
        Require.NotNull(callback, nameof(callback));

        _callback = callback;
    }

    /// <inheritdoc/>
    protected override nint OnHookEvent(nint hWnd, uint msg, nint wParam, nint lParam)
    {
        uint localMsg = msg;
        nint localWParam = wParam;
        nint localLParam = lParam;

        _callback(ref localMsg, ref localWParam, ref localLParam);

        // The hook procedure will only apply the changes if we answer with the token identifying them.
        if (localMsg != msg || localWParam != wParam || localLParam != lParam)
            return Native.ChangeMessageDetails(ThreadId, localMsg, localWParam, localLParam);

        return 0;
    }
}
