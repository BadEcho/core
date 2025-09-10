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
/// Specifies a Desktop Window Manager attribute.
/// </summary>
internal enum DwmWindowAttribute
{
    /// <summary>
    /// Discovers whether non-client rendering is enabled.
    /// </summary>
    NonClientRenderingEnabled = 1,
    /// <summary>
    /// Sets the non-client rendering policy.
    /// </summary>
    NonClientRenderingPolicy,
    /// <summary>
    /// Enables or forcibly disables DWM transitions.
    /// </summary>
    TransitionsForceDisabled,
    /// <summary>
    /// Enables content rendered in the non-client area to be visible on the frame drawn by DWM.
    /// </summary>
    NonClientContentRendered,
    /// <summary>
    /// Retrieves the bounds of the caption button area in the window-relative space.
    /// </summary>
    CaptionButtonBounds,
    /// <summary>
    /// Specifies whether non-client content is right-to-left mirrored.
    /// </summary>
    NonClientRightToLeftMirrored,
    /// <summary>
    /// Forces the window to display an iconic thumbnail or peek representation.
    /// </summary>
    ForceIconicThumbnail,
    /// <summary>
    /// Sets how Flip3D treats the window.
    /// </summary>
    Flip3DPolicy,
    /// <summary>
    /// Retrieves the extended frame bounds rectangle in screen space.
    /// </summary>
    ExtendedFrameBounds,
    /// <summary>
    /// Specifies if the window will provide a bitmap for use by DWM as an iconic thumbnail or peek representation for the window.
    /// </summary>
    HasIconicBitmap,
    /// <summary>
    /// Disables showing peek preview for the window.
    /// </summary>
    DisallowPeek,
    /// <summary>
    /// Prevents a window from fading to a glass sheet when peek is invoked.
    /// </summary>
    ExcludedFromPeek,
    /// <summary>
    /// Cloaks the window such that it is not visible to the user.
    /// </summary>
    Cloak,
    /// <summary>
    /// Retrieves a value explaining why a window is cloaked.
    /// </summary>
    Cloaked,
    /// <summary>
    /// Freeze the window's thumbnail image with its current visuals.
    /// </summary>
    FreezeRepresentation,
    /// <summary>
    /// Updates the window only when desktop composition runs for other reasons.
    /// </summary>
    PassiveUpdateMode,
    /// <summary>
    /// Enables a non-UWP window to use host backdrop brushes.
    /// </summary>
    HostBackdropBrush,
    /// <summary>
    /// Allows the window frame for this window to be drawn in dark mode colors when the dark mode system setting is enabled.
    /// </summary>
    UseImmersiveDarkMode = 20,
    /// <summary>
    /// Specifies the rounded corner preference for a window.
    /// </summary>
    WindowCornerPreference = 33,
    /// <summary>
    /// Specifies the color of the window border.
    /// </summary>
    BorderColor,
    /// <summary>
    /// Specifies the color of the caption.
    /// </summary>
    CaptionColor,
    /// <summary>
    /// Specifies the color of the caption text.
    /// </summary>
    TextColor,
    /// <summary>
    /// Retrieves the width of the outer boarder that the DWM would draw around this window.
    /// </summary>
    VisibleFrameBorderThickness,
    /// <summary>
    /// Retrieves or specifies the system-drawn backdrop material of a window.
    /// </summary>
    SystemBackdropType,
    /// <summary>
    /// Value indicating if GDI redirection bitmap contains premultiplied alpha.
    /// </summary>
    RedirectionBitmapAlpha,
    /// <summary>
    /// Sets the window border margins.
    /// </summary>
    BorderMargins,
    /// <summary>
    /// The maximum recognized attribute value, used for validation purposes.
    /// </summary>
    Last
}
