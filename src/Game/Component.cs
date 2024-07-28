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

namespace BadEcho.Game;

/// <summary>
/// Provides an aspect or behavior ascribed to an entity.
/// </summary>
public abstract class Component
{
    /// <summary>
    /// Applies this component to the provided entity.
    /// </summary>
    /// <param name="entity">The entity to act on.</param>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public abstract void Update(IEntity entity, GameUpdateTime time);
}