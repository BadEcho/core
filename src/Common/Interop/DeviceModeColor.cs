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
/// Specifies the color mode of a device.
/// </summary>
internal enum DeviceModeColor : short
{
    /// <summary>
    /// The color mode is unset.
    /// </summary>
    Unset,
    /// <summary>
    /// Monochrome color mode.
    /// </summary>
    Monochrome,
    /// <summary>
    /// Colorful...color mode?
    /// </summary>
    Color
}
