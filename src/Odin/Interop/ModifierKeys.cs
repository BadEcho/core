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
/// Specifies keys that modify the action of another key when pressed together.
/// </summary>
[Flags]
public enum ModifierKeys
{
    /// <summary>
    /// No modifier keys.
    /// </summary>
    None = 0,
    /// <summary>
    /// The ALT key.
    /// </summary>
    Alt = 1,
    /// <summary>
    /// The CTRL key.
    /// </summary>
    Control = 2,
    /// <summary>
    /// The SHIFT key.
    /// </summary>
    Shift = 4,
    /// <summary>
    /// The Windows logo key.
    /// </summary>
    Windows = 8
}