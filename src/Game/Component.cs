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
/// Provides a process that acts on an associated entity.
/// </summary>
public abstract class Component
{
    /// <summary>
    /// Executes this component's process on the provided entity.
    /// </summary>
    /// <param name="entity">The entity to act on.</param>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public abstract void Update(IPositionalEntity entity, GameUpdateTime time);
}
