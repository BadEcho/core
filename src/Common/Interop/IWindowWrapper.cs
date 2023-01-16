//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Defines a wrapper around an <c>HWND</c> of a window and the messages it receives.
/// </summary>
public interface IWindowWrapper
{
    /// <summary>
    /// Adds a hook that will receive messages sent to the wrapped window.
    /// </summary>
    /// <param name="hook">The hook to invoke when messages are sent to the wrapped window.</param>
    void AddHook(WindowProc hook);

    /// <summary>
    /// Removes a hook previously receiving messages sent to the wrapped window.
    /// </summary>
    /// <param name="hook">The hook to remove.</param>
    void RemoveHook(WindowProc hook);
}