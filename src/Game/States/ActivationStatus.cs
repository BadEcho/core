//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Game.States;

/// <summary>
/// Specifies the activation phase of a game state's scene.
/// </summary>
public enum ActivationStatus
{
    /// <summary>
    /// The game state has been fully activated and is visible on the screen.
    /// </summary>
    Activated,
    /// <summary>
    /// The game state is in the process of activating onto the screen.
    /// </summary>
    Activating,
    /// <summary>
    /// The game state is in the process of deactivating off the screen.
    /// </summary>
    Deactivating,
    /// <summary>
    /// The game state has been deactivated and is not visible on the screen.
    /// </summary>
    Deactivated
}
