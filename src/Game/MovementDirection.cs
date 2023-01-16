//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Game;

/// <summary>
/// Specifies a direction of movement.
/// </summary>
public enum MovementDirection
{
    /// <summary>
    /// No movement in any direction.
    /// </summary>
    None,
    /// <summary>
    /// Movement in the upward direction.
    /// </summary>
    Up,
    /// <summary>
    /// Movement in the downward direction.
    /// </summary>
    Down,
    /// <summary>
    /// Movement in the leftward direction.
    /// </summary>
    Left,
    /// <summary>
    /// Movement in the rightward direction.
    /// </summary>
    Right
}
