//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
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
/// Specifies extended styling configurations for windows that can be set using
/// <see cref="User32.SetWindowLongPtr(WindowHandle, WindowAttribute, IntPtr)"/>.
/// </summary>
[Flags]
public enum ExtendedWindowStyles
{
    /// <summary>
    /// The window should not be painted until siblings beneath the window created by the same thread have been painted.
    /// </summary>
    Transparent = 0x00000020
}