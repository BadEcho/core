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

namespace BadEcho.Game;

/// <summary>
/// Specifies the types of transitions a component undergoes when appearing and disappearing from view.
/// </summary>
[Flags]
public enum Transitions
{
    /// <summary>
    /// The component does not transition onto or off the screen.
    /// </summary>
    None = 0x0,
    /// <summary>
    /// The component transitions onto the screen in a leftward direction and transitions off the screen in a rightward
    /// direction.
    /// </summary>
    MoveLeft = 0x1,
    /// <summary>
    /// The component transitions onto the screen in a rightward direction and transitions off the screen in a leftward
    /// direction.
    /// </summary>
    MoveRight = 0x2,
    /// <summary>
    /// The component transitions onto the screen in an upward direction and transitions off the screen in a downward
    /// direction.
    /// </summary>
    MoveUp = 0x4,
    /// <summary>
    /// The component transitions onto the screen in a downward direction and transitions off the screen in an upward
    /// direction.
    /// </summary>
    MoveDown = 0x8,
    /// <summary>
    /// The component transitions onto the screen by fading in and transitions off the screen by fading out.
    /// </summary>
    Fade = 0x10,
    /// <summary>
    /// The component transitions onto the screen by zooming in and transitions off the screen by zooming out.
    /// </summary>
    Zoom = 0x20,
    /// <summary>
    /// The component transitions onto and off the screen doing a full rotation.
    /// </summary>
    Rotate = 0x40

}
