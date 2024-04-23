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

using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Defines an entity able to be positioned and moved on the screen in two dimensions.
/// </summary>
public interface IPositionalEntity
{
    /// <summary>
    /// Gets or sets the current drawing location of the entity.
    /// </summary>
    public Vector2 Position
    { get; set; }

    /// <summary>
    /// Gets the change to the entity's position that occurred from its last update.
    /// </summary>
    public Vector2 LastMovement
    { get; }

    /// <summary>
    /// Gets or sets the rate of change of the entity's position.
    /// </summary>
    public Vector2 Velocity
    { get; set; }

    /// <summary>
    /// Gets the amount that the entity is currently being rotated about its point of rotation.
    /// </summary>
    public float Angle
    { get; }

    /// <summary>
    /// Gets or sets the rate of change of the entity's angle.
    /// </summary>
    public float AngularVelocity
    { get; set; }
}
