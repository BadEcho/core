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
/// Indicates a type of standard message value used when sending or posting messages to windows.
/// </summary>
public enum WindowMessage
{
    /// <summary>
    /// Indicates a window message corresponding to WM_NULL.
    /// </summary>
    Null = 0,
    /// <summary>
    /// Indicates a window message indicating a hot key has been pressed, corresponding to WM_HOTKEY.
    /// </summary>
    HotKey = 0x312
}