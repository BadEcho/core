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

namespace BadEcho.Hooks.Interop;

/// <summary>
/// Specifies additional information provided to a low-level keyboard hook procedure.
/// </summary>
[Flags]
internal enum KBDLLHOOKSTRUCTFlags
{
    /// <summary>
    /// An extended key was pressed.
    /// </summary>
    Extended = 0x1,
    /// <summary>
    /// The input event was synthesized.
    /// </summary>
    Injected = 0x10,
    /// <summary>
    /// The input event was synthesized from a process running at a lower integrity level than this one.
    /// </summary>
    InjectedLowerIntegrity = 0x2,
    /// <summary>
    /// The ALT key is being held down at the time of the hook event.
    /// </summary>
    AltDown = 0x20,
    /// <summary>
    /// The transition-state (i.e., this will be 0 if the key is pressed or 1 if it is being released).
    /// </summary>
    KeyUp = 0x80
}
