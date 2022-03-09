//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides a collection of actions that are able to be executed by the controlling behavior.
/// </summary>
/// <typeparam name="T">The type of <see cref="DependencyObject"/> this collection can attach to.</typeparam>
public sealed class BehaviorActionCollection<T> : AttachableComponentCollection<T, BehaviorAction<T>>
    where T : DependencyObject
{
    /// <summary>
    /// Executes the actions in this collection until either an action fails or all actions are exhausted.
    /// </summary>
    /// <returns>True if all actions executed successfully; otherwise, false.</returns>
    public bool ExecuteActions() 
        => this.All(action => action.Execute());

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore() 
        => new BehaviorActionCollection<T>();
}