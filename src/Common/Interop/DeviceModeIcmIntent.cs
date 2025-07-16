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
/// Specifies which color matching method a device should use by default.
/// </summary>
internal enum DeviceModeIcmIntent : uint
{
    /// <summary>
    /// The color matching method to use is unset.
    /// </summary>
    Unset,
    /// <summary>
    /// Color matching should optimize for color saturation.
    /// </summary>
    Saturate,
    /// <summary>
    /// Color matching should optimize for color contrast.
    /// </summary>
    Contrast,
    /// <summary>
    /// Color matching should optimize to match the exact color requested.
    /// </summary>
    ColorMetric,
    /// <summary>
    /// Color matching should optimize to match the exact color requested without white point mapping.
    /// </summary>
    AbsoluteColorMetric
}
