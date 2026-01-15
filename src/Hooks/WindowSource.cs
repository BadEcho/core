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

namespace BadEcho.Hooks;

/// <summary>
/// Provides a publisher of messages being sent to a window procedure.
/// </summary>
public sealed class WindowSource : HookSource<WindowProcedure>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSource"/> class.
    /// </summary>
    /// <param name="threadId">The identifier for the thread whose window we're hooking into.</param>
    public WindowSource(int threadId) 
        : this(threadId, true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSource"/> class.
    /// </summary>
    /// <param name="threadId">The identifier for the thread whose window we're hooking into.</param>
    /// <param name="beforeWindow">
    /// Value indicating if messages should be intercepted before they're sent to the destination window procedure.
    /// </param>
    public WindowSource(int threadId, bool beforeWindow)
        : base(beforeWindow ? HookType.CallWindowProcedure : HookType.CallWindowProcedureReturn, threadId)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSource"/> class.
    /// </summary>
    /// <remarks>This will install a global window hook, capturing messages being sent to all windows on the desktop.</remarks>
    public WindowSource() 
        : this(true)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowSource"/> class.
    /// </summary>
    /// <remarks>This will install a global window hook, capturing messages being sent to all windows on the desktop.</remarks>
    public WindowSource(bool beforeWindow)
        : base(beforeWindow ? HookType.CallWindowProcedure : HookType.CallWindowProcedureReturn)
    { }

    /// <inheritdoc/>
    protected override void OnHookEvent(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        foreach (WindowProcedure callback in Callbacks)
        {
            var result = callback(hWnd, msg, wParam, lParam);
            
            if (result.Handled)
                break;
        }
    }
}
