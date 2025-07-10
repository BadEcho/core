﻿// -----------------------------------------------------------------------
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

namespace BadEcho.Interop;

/// <summary>
/// Specifies an attribute of a window that can be retrieved with <see cref="User32.GetWindowLongPtr(WindowHandle, WindowAttribute)"/>
/// and modified with <see cref="User32.SetWindowLongPtr(WindowHandle, WindowAttribute, IntPtr)"/>.
/// </summary>
internal enum WindowAttribute
{
    /// <summary>
    /// The extended window style.
    /// </summary>
    ExtendedStyle = -20,
    /// <summary>
    /// A handle to the application instance.
    /// </summary>
    Instance = -6,
    /// <summary>
    /// A handle to the parent window, if one exists.
    /// This cannot be set through <see cref="User32.SetWindowLongPtr(WindowHandle, WindowAttribute, IntPtr)"/>.
    /// </summary>
    Parent = -8,
    /// <summary>
    /// The identifier of the window, if it is a child window.
    /// </summary>
    Identifier = -12,
    /// <summary>
    /// The window style.
    /// </summary>
    Style = -16,
    /// <summary>
    /// The user data associated with the window.
    /// </summary>
    UserData = -21,
    /// <summary>
    /// The pointer to the window procedure or a handle representing the pointer to the window procedure.
    /// </summary>
    WindowProcedure = -4
}