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
/// Defines a fluent interface for configuring either a state or a state's transition.
/// </summary>
/// <typeparam name="T">Type used as the descriptor of the state.</typeparam>
public interface IStateOrTransitionBuilder<T> : IStateBuilder<T>,
                                                IStateTransitionBuilder<T>
    where T : notnull;
