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
/// Specifies the orientation of a display device.
/// </summary>
internal enum DeviceModeDisplayOrientation : uint
{
    /// <summary>
    /// The orientation is the natural orientation of the display device.
    /// </summary>
    Default,
    /// <summary>
    /// The orientation is rotated 90 degrees from the default.
    /// </summary>
    Rotate90,
    /// <summary>
    /// The orientation is rotated 180 degrees from the default.
    /// </summary>
    Rotate180,
    /// <summary>
    /// The orientation is rotated 270 degrees from the default.
    /// </summary>
    Rotate270
}
