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
/// Provides a compound behavior that executes a sequence of actions attached to a target dependency object upon
/// the firing of the <see cref="FrameworkElement.LoadedEvent"/> routed event.
/// </summary>
public sealed class LoadedEventBehavior : EventBehavior
{
    /// <summary>
    /// Identifies the attached property that gets or sets a <see cref="DependencyObject"/> instance's collection
    /// of actions executed by an attached instance of this behavior.
    /// </summary>
    public static readonly DependencyProperty ActionsProperty
        = RegisterAttachment();

    /// <summary>
    /// Initializes a new instance of the <see cref="LoadedEventBehavior"/> class.
    /// </summary>
    public LoadedEventBehavior()
        : base(FrameworkElement.LoadedEvent)
    { }

    /// <summary>
    /// Gets the value of the <see cref="ActionsProperty"/> attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="source">The dependency object from which the property value is read.</param>
    /// <returns>The collection of actions executed by the instance of this behavior attached to <c>source</c>.</returns>
    public static BehaviorActionCollection<DependencyObject> GetActions(DependencyObject source)
        => GetAttachment(source, ActionsProperty);

    /// <inheritdoc/>
    protected override DependencyProperty HostedActionsProperty
        => ActionsProperty;

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
        => new LoadedEventBehavior();

    private static DependencyProperty RegisterAttachment()
    {
        LoadedEventBehavior behavior = new();

        return RegisterAttachment(NameOf.ReadAccessorEnabledDependencyPropertyName(() => ActionsProperty),
                                  typeof(LoadedEventBehavior),
                                  behavior.DefaultMetadata);
    }
}