//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies the dot per inch (DPI) setting for a monitor.
/// </summary>
/// <remarks>
/// <para>
/// This corresponds to the unmanaged <c>MONITOR_DPI_TYPE</c> enumeration type, however it lacks a corresponding
/// <c>MDT_DEFAULT</c> member, as this member, representing the default DPI setting for a monitor, is always just
/// equal to <c>MDT_EFFECTIVE_DPI</c>.
/// </para>
/// <para>
/// Creating a managed representation for this member and setting it equal to <see cref="Effective"/> gives this enumeration
/// type an appearance of one whose values are bit fields, which they are not. Given that this annoys me, and that the original
/// definition for <c>MONITOR_DPI_TYPE</c> annoys me, I've removed any member that might correspond to <c>MDT_DEFAULT</c>.
/// </para>
/// </remarks>
public enum MonitorDpiType
{
    /// <summary>
    /// DPI that incorporates accessibility overrides and matches what the Desktop Window Manager uses to scale desktop applications.
    /// </summary>
    Effective = 0,
    /// <summary>
    /// DPI that ensures rendering at a compliant angular resolution on screen, without incorporating accessibility overrides.
    /// </summary>
    Angular = 1,
    /// <summary>
    /// DPI that is the linear DPI of the screen as measured on the screen itself.
    /// </summary>
    Raw = 2
}