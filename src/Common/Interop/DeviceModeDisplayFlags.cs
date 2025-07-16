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
/// Specifies flags for a device's display mode.
/// </summary>
[Flags]
internal enum DeviceModeDisplayFlags : uint
{
    /// <summary>
    /// The display mode is noninterlaced. <see cref="Interlaced"/> must not be set for this to go into effect.
    /// </summary>
    Noninterlaced = 0x0,
    /// <summary>
    /// The display is a noncolor device. This flag is apparently no longer valid.
    /// </summary>
    Grayscale = 0x2,
    /// <summary>
    /// The display mode is interlaced. If this flag is not set, then the display mode is noninterlaced.
    /// </summary>
    Interlaced = 0x4
}
