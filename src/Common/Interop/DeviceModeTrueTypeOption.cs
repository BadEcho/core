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
/// Specifies how TrueType fonts should be printed by a device.
/// </summary>
/// <remarks>
/// <para>
/// Most of the DEVMODE enumeration types are named based on their unmanaged definitions. If we followed this practice
/// with this type, however, we'd end up with the name <c>DeviceModeTrueType</c> (due to its members being defined using
/// the prefix <c>DMTT_*</c>).
/// </para>
/// <para>
/// This is simply too strange of a type name for me to bear, so I've appended <c>Option</c> to the type name. Just in case
/// anyone was curious :).
/// </para>
/// </remarks>
internal enum DeviceModeTrueTypeOption : short
{
    /// <summary>
    /// The manner in which TrueType fonts are printed unset.
    /// </summary>
    Unset,
    /// <summary>
    /// Prints TrueType fonts as graphics.
    /// </summary>
    Bitmap,
    /// <summary>
    /// Downloads TrueType fonts as soft fonts.
    /// </summary>
    Download,
    /// <summary>
    /// Substitutes device fonts for TrueType fonts.
    /// </summary>
    SubstituteDeviceFonts,
    /// <summary>
    /// Downloads TrueType fonts as outline soft fonts.
    /// </summary>
    DownloadOutline
}
