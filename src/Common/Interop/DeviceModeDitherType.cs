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
/// Specifies how dithering is to be done by a device.
/// </summary>
internal enum DeviceModeDitherType : uint
{
    /// <summary>
    /// The manner in which dithering is to be done is unset.
    /// </summary>
    Unset,
    /// <summary>
    /// No dithering.
    /// </summary>
    None,
    /// <summary>
    /// Dithering with a coarse brush.
    /// </summary>
    Coarse,
    /// <summary>
    /// Dithering with a fine brush.
    /// </summary>
    Fine,
    /// <summary>
    /// Line art dithering.
    /// </summary>
    LineArt,
    /// <summary>
    /// Device does grayscaling.
    /// </summary>
    Grayscale = 9
}
