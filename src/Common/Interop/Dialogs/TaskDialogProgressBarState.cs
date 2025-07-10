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
/// Specifies the state of a task dialog progress bar.
/// </summary>
public enum TaskDialogProgressBarState
{
    /// <summary>
    /// An uninitialized progress bar that doesn't actually exist!
    /// </summary>
    None,
    /// <summary>
    /// A regular progress bar.
    /// </summary>
    Normal,
    /// <summary>
    /// An error progress bar (red).
    /// </summary>
    Error,
    /// <summary>
    /// A paused progress bar (yellow).
    /// </summary>
    Paused,
    /// <summary>
    /// A marquee progress bar.
    /// </summary>
    Marquee,
    /// <summary>
    /// A paused marquee progress bar.
    /// </summary>
    MarqueePaused
}
