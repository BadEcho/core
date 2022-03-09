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
/// Provides a base action executed as the result of a behavior's influence on the target object the behavior is attached to.
/// </summary>
/// <typeparam name="T">The type of <see cref="DependencyObject"/> this action can attach to.</typeparam>
public abstract class BehaviorAction<T> : AttachableComponent<T>
    where T : DependencyObject
{
    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <returns>Value indicating the success of the action's execution.</returns>
    public abstract bool Execute();
}