// -----------------------------------------------------------------------
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
/// Specifies extended styling configurations for windows that can be set using
/// <see cref="User32.SetWindowLongPtr(WindowHandle, WindowAttribute, IntPtr)"/>.
/// </summary>
[Flags]
public enum ExtendedWindowStyles
{
    /// <summary>
    /// The window should not be painted until siblings beneath the window created by the same thread have been painted.
    /// </summary>
    Transparent = 0x00000020,
    /// <summary>
    /// The window is intended to be used as a floating toolbar, which gives it a smaller caption and a thinner frame border
    /// than a standard window.
    /// </summary>
    /// <remarks>
    /// This style also excludes the window from the taskbar and from the switcher displayed when the user presses Alt-Tab.
    /// It is the only measure found to accomplish the latter: neither giving a window an owner nor cloaking it will
    /// keep it out of the switcher.
    /// </remarks>
    ToolWindow = 0x00000080,
    /// <summary>
    /// The window itself contains child windows that should take part in dialog box navigation.
    /// </summary>
    ControlParent = 0x00010000,
    /// <summary>
    /// The window is a layered window, which allows it to be composited with per-window opacity or a transparency color key.
    /// </summary>
    Layered = 0x00080000
}