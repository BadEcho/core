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
/// Specifies how a display device presents a low-resolution mode on a higher-resolution display.
/// </summary>
internal enum DeviceModeDisplayFixedOutput : uint
{
    /// <summary>
    /// The display's default setting.
    /// </summary>
    Default,
    /// <summary>
    /// The low-resolution image is stretched to fill the larger screen space.
    /// </summary>
    Stretch,
    /// <summary>
    /// The low-resolution image is centered in the larger screen space.
    /// </summary>
    Center
}
