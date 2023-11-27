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
/// Specifies an attribute of a window class that can be modified with <see cref="User32.SetClassLongPtr"/>.
/// </summary>
internal enum WindowClassAttribute
{
    /// <summary>
    /// The size, in bytes, of the extra memory associated with the class.
    /// </summary>
    ClassExtraBytes = -20,
    /// <summary>
    /// The size, in bytes, of the extra memory associated with each window of the class.
    /// </summary>
    WindowExtraBytes = -18,
    /// <summary>
    /// A handle to the background brush associated with the class.
    /// </summary>
    Background = -10,
    /// <summary>
    /// A handle to the cursor associated with the class.
    /// </summary>
    Cursor = -12,
    /// <summary>
    /// A handle to the icon associated with the class.
    /// </summary>
    Icon = -14,
    /// <summary>
    /// A handle to the small icon associated with the class.
    /// </summary>
    SmallIcon = -34,
    /// <summary>
    /// A handle to the module that registered the class.
    /// </summary>
    Module = -16,
    /// <summary>
    /// A pointer to the menu name string.
    /// </summary>
    MenuName = -8,
    /// <summary>
    /// The class style bits.
    /// </summary>
    Style = -26,
    /// <summary>
    /// A pointer to the window procedure associated with the class.
    /// </summary>
    WindowProcedure = -24
}
