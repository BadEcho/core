//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
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
/// Specifies the types of transitions a game state undergoes when activating and deactivating.
/// </summary>
[Flags]
public enum StateTransitions
{
    /// <summary>
    /// The state transitions onto the screen in the direction specified by <see cref="GameState.TransitionDirection"/>
    /// and transitions off the screen in the opposite direction.
    /// </summary>
    Move = 0x1,
    /// <summary>
    /// The state transitions onto the screen by fading in and transitions off the screen by fading out.
    /// </summary>
    Fade = 0x2,
    /// <summary>
    /// The state transitions onto the screen by zooming in and transitions off the screen by zooming out.
    /// </summary>
    Zoom = 0x4,
    /// <summary>
    /// The state transitions onto and off the screen doing a full rotation.
    /// </summary>
    Rotate = 0x8
}
