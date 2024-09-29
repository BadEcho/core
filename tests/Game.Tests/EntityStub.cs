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

namespace BadEcho.Game.Tests;
public sealed class EntityStub : IEntity
{
    public IShape Bounds
        => RectangleF.Empty;

    public ICollection<Component> Components 
    { get; } = [];

    public Vector2 Position 
    { get; set; }

    public Vector2 LastMovement 
        => Vector2.Zero;

    public Vector2 Velocity 
    { get; set; }

    public float Angle 
        => 0f;

    public float AngularVelocity 
    { get; set; }
}