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
/// Specifies the mode of double-sided printing for devices capable of duplex printing.
/// </summary>
internal enum DeviceModeDuplex : short
{
    /// <summary>
    /// The duplex mode is unset.
    /// </summary>
    Unset,
    /// <summary>
    /// Normal (nonduplex) printing.
    /// </summary>
    Simplex,
    /// <summary>
    /// Long-edge binding, that is, the long edge of the page is vertical.
    /// </summary>
    Vertical,
    /// <summary>
    /// Short-edge binding, that is, the long edge of the page is horizontal.
    /// </summary>
    Horizontal
}
