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
/// Provides a base compound behavior that executes a sequence of actions attached to a target dependency object upon the firing
/// of a <see cref="RoutedEvent"/>.
/// </summary>
public abstract class EventBehavior : CompoundBehavior<FrameworkElement, BehaviorActionCollection<DependencyObject>>
{
    private readonly RoutedEvent _routedEvent;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBehavior"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event </param>
    protected EventBehavior(RoutedEvent routedEvent)
    {
        Require.NotNull(routedEvent, nameof(routedEvent));

        _routedEvent = routedEvent;
    }

    /// <summary>
    /// Gets the dependency property registered as this behavior's action collection.
    /// </summary>
    protected abstract DependencyProperty HostedActionsProperty 
    { get; }

    /// <summary>
    /// Registers the auxiliary action collection of the event behavior as an attached property according to the provided
    /// specifications.
    /// </summary>
    /// <param name="propertyName">The dependency property name to use when registering the action collection.</param>
    /// <param name="ownerType">The type of behavior registering this property.</param>
    /// <param name="defaultMetadata">
    /// The default <see cref="PropertyMetadata"/> of the event behavior registering this property.
    /// </param>
    /// <returns>A registered <see cref="DependencyProperty"/> for the behavior's auxiliary action collection.</returns>
    protected static DependencyProperty RegisterAttachment(string propertyName, Type ownerType, PropertyMetadata defaultMetadata)
        => DependencyProperty.RegisterAttached(propertyName,
                                               typeof(BehaviorActionCollection<DependencyObject>),
                                               ownerType,
                                               defaultMetadata);
    /// <inheritdoc/>
    protected override void OnValueAssociated(FrameworkElement targetObject, BehaviorActionCollection<DependencyObject> newValue)
    {
        base.OnValueAssociated(targetObject, newValue);

        Require.NotNull(targetObject, nameof(targetObject));
            
        targetObject.AddHandler(_routedEvent, (RoutedEventHandler) OnEvent);
    }
        
    /// <inheritdoc/>
    protected override void OnValueDisassociated(FrameworkElement targetObject, BehaviorActionCollection<DependencyObject> oldValue)
    {
        base.OnValueDisassociated(targetObject, oldValue);

        Require.NotNull(targetObject, nameof(targetObject));

        targetObject.RemoveHandler(_routedEvent, (RoutedEventHandler) OnEvent);
    }

    private void OnEvent(object sender, RoutedEventArgs e)
    {
        var targetObject = (DependencyObject) sender;

        BehaviorActionCollection<DependencyObject> actions = GetAttachment(targetObject, HostedActionsProperty);

        foreach (BehaviorAction<DependencyObject> action in actions)
        {
            if (!action.Execute())
                return;
        }
    }
}