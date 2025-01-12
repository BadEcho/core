//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

using BadEcho.Collections;
using BadEcho.Extensions;
using BadEcho.Logging;
using BadEcho.Interop;
using BadEcho.Hooks.Properties;

namespace BadEcho.Hooks;

/// <summary>
/// Provides a publisher of messages being read from a message queue.
/// </summary>
public sealed class MessageQueueMessageSource : IMessageSource<GetMessageHookProc>, IDisposable
{
    private readonly CachedWeakList<GetMessageHookProc> _hooks = [];
    private readonly MessageOnlyExecutor _hookExecutor = new();
    private readonly int _threadId;

    private bool _disposed;
    private bool _queueHooked;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageQueueMessageSource"/> class.
    /// </summary>
    /// <param name="threadId">
    /// The identifier for the thread whose message pump we're hooking.
    /// </param>
    public MessageQueueMessageSource(int threadId)
    {
        _threadId = threadId;

        HookAdded += async (_,_) 
            => await HandleHookAdded().ConfigureAwait(false);
    }

    private event EventHandler HookAdded;

    /// <inheritdoc/>
    public void AddHook(GetMessageHookProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        _hooks.Add(hook);

        HookAdded.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public void RemoveHook(GetMessageHookProc hook)
    {
        Require.NotNull(hook, nameof(hook));

        _hooks.Remove(hook);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        if (_queueHooked)
        {
            _queueHooked = !Native.RemoveHook(HookType.GetMessage, _threadId);

            if (_queueHooked)
                Logger.Warning(Strings.UnhookMessageQueueFailed.InvariantFormat(_threadId));
        }

        _hookExecutor.Dispose();

        _disposed = true;
    }
    
    private async Task HandleHookAdded()
    {
        if (_hookExecutor.Window != null)
            return;
         
        await _hookExecutor.RunAsync();
            
        if (_hookExecutor.Window == null)
            throw new InvalidOperationException(Strings.MessageQueueForHookFailed);

        _hookExecutor.Window.AddHook(GetMessageProcedure);

        _queueHooked = Native.AddHook(HookType.GetMessage,
                                      _threadId,
                                      _hookExecutor.Window.Handle);
    }

    private HookResult GetMessageProcedure(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        uint localMsg = msg;
        IntPtr localWParam = wParam;
        IntPtr localLParam = lParam;

        foreach (GetMessageHookProc hook in _hooks)
        {
            var result = hook(hWnd, ref localMsg, ref localWParam, ref localLParam);

            if (result.Handled)
                break;
        }
        
        if (localMsg != msg || localWParam != wParam || localLParam != lParam)
            Native.ChangeMessageDetails(localMsg, localWParam, localLParam);

        // We always mark it as handled, we don't want further processing by any supporting
        // infrastructure.
        return new HookResult(IntPtr.Zero, true);
    }
}
