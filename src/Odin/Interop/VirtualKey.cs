//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin.Interop;

/// <summary>
/// Specifies a virtual key, which may be anything from an actual keyboard key to a button on the mouse.
/// </summary>
public enum VirtualKey : uint
{
    /// <summary>
    /// A key that does not exist. The complete absence of a key. A key with no name, no purpose, and no hope.
    /// </summary>
    None = 0,
    /// <summary>
    /// The L key.
    /// </summary>
    L = 0x4C,
    /// <summary>
    /// The Z key.
    /// </summary>
    Z = 0x5A,
    /// <summary>
    /// The F11 key.
    /// </summary>
    F11 = 0x7A,
    /// <summary>
    /// The F12 key.
    /// </summary>
    F12 = 0x7B
}