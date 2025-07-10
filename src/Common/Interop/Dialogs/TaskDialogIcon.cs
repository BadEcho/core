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

namespace BadEcho.Interop.Dialogs;

/// <summary>
/// Specifies a predefined icon for a task dialog.
/// </summary>
public enum TaskDialogIcon
{
    /// <summary>
    /// No icon.
    /// </summary>
    None = 0,
    /// <summary>
    /// An information icon.
    /// </summary>
    Information = ushort.MaxValue - 2,
    /// <summary>
    /// A warning icon.
    /// </summary>
    Warning = ushort.MaxValue,
    /// <summary>
    /// An error icon.
    /// </summary>
    Error = ushort.MaxValue - 1,
    /// <summary>
    /// A User Account Control shield icon.
    /// </summary>
    Shield = ushort.MaxValue - 3,
    /// <summary>
    /// A User Account Control shield icon over a blue background.
    /// </summary>
    ShieldBlueBar = ushort.MaxValue - 4,
    /// <summary>
    /// A User Account Control shield icon over a gray background.
    /// </summary>
    ShieldGrayBar = ushort.MaxValue - 8,
    /// <summary>
    /// A User Account Control shield icon over a yellow background.
    /// </summary>
    ShieldYellowBar = ushort.MaxValue - 5,
    /// <summary>
    /// A User Account Control shield icon over a red background.
    /// </summary>
    ShieldRedBar = ushort.MaxValue - 6,
    /// <summary>
    /// A User Account Control shield icon over a green background.
    /// </summary>
    ShieldGreenBar = ushort.MaxValue - 7
}
