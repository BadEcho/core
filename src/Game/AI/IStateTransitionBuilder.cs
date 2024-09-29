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

namespace BadEcho.Game.AI;

 /// <summary>
 /// Defines a fluent interface for configuring a state's transition.
 /// </summary>
 /// <typeparam name="T">Type used as the descriptor of the state.</typeparam>
public interface IStateTransitionBuilder<T>
    where T : notnull
{
    /// <summary>
    /// Defines a condition that must be met in order for the transition to proceed.
    /// </summary>
    /// <param name="condition">A method that will return true if the transition should proceed.</param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateOrTransitionBuilder<T> WhenTrue(Func<T, bool> condition);
    
    /// <summary>
    /// Specifies that the transition will occur when registered components can no longer continue processing.
    /// </summary>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateOrTransitionBuilder<T> WhenComponentsDone();

    /// <summary>
    /// Defines the amount of time that must pass before a transition can proceed.
    /// </summary>
    /// <param name="duration">The amount of time the state must execute for before the transition can proceed.</param>
    /// <returns>A <see cref="IStateBuilder{T}"/> instance that can be used to further configure the state.</returns>
    IStateOrTransitionBuilder<T> After(TimeSpan duration);
}