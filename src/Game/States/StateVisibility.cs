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
/// Specifies the visibility and appearance of a game state's scene.
/// </summary>
public enum StateVisibility
{
    /// <summary>
    /// The game state is visible on the screen.
    /// </summary>
    Visible,
    /// <summary>
    /// The game state is in the process of transitioning onto the screen.
    /// </summary>
    Activating,
    /// <summary>
    /// The game state is in the process of transitioning off the screen.
    /// </summary>
    Deactivating,
    /// <summary>
    /// The game state is not visible on the screen.
    /// </summary>
    Hidden
}
