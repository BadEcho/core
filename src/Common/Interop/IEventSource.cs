//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Defines a publisher of events associated with a hook chain.
/// </summary>
/// <typeparam name="THookProc">The type of hook procedure used by the hook type.</typeparam>
public interface IEventSource<in THookProc> where THookProc : Delegate
{
    /// <summary>
    /// Adds a hook that will receives messages sent to the hook chain.
    /// </summary>
    /// <param name="hook">The hook to invoke when messages are sent to the associated hook chain.</param>
    void AddHook(THookProc hook);

    /// <summary>
    /// Removes a hook previously receiving messages sent to the hook chain.
    /// </summary>
    /// <param name="hook">The hook to remove.</param>
    void RemoveHook(THookProc hook);
}