//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Represents a low-level keyboard input event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct KBDLLHOOKSTRUCT
{
    /// <summary>
    /// A virtual-key code.
    /// </summary>
    public uint vkCode;
    /// <summary>
    /// A hardware scan code for the key.
    /// </summary>
    public uint scanCode;
    /// <summary>
    /// Flags containing additional information for the event.
    /// </summary>
    public KBDLLHOOKSTRUCTFlags flags;
    /// <summary>
    /// The timestamp for this message.
    /// </summary>
    public uint time;
    /// <summary>
    /// Additional information associated with the message.
    /// </summary>
    public UIntPtr dwExtraInfo;
}
