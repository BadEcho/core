//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Data;

namespace BadEcho.Fenestra.Behaviors
{
    /// <summary>
    /// Provides a compound behavior that executes a sequence of actions attached to a target dependency object upon
    /// the firing of the <see cref="Binding.TargetUpdatedEvent"/> routed event.
    /// </summary>
    public sealed class TargetUpdatedEventBehavior : EventBehavior
    {
        /// <summary>
        /// Identifies the attached property that gets or sets a <see cref="DependencyObject"/> instance's collection
        /// of actions executed by this type of behavior.
        /// </summary>
        public static readonly DependencyProperty ActionsProperty 
            = RegisterAttachment();

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetUpdatedEventBehavior"/> class.
        /// </summary>
        public TargetUpdatedEventBehavior()
            : base(Binding.TargetUpdatedEvent)
        { }
        
        /// <inheritdoc/>
        protected override DependencyProperty HostedActionsProperty
            => ActionsProperty;

        /// <summary>
        /// Gets the value of the <see cref="ActionsProperty"/> attached property from a given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="source">The dependency object from which to read the property value.</param>
        /// <returns>The collection of actions executed by this type of behavior attached to <c>source</c>.</returns>
        public static BehaviorActionCollection<DependencyObject> GetActions(DependencyObject source)
            => GetAttachment(source, ActionsProperty);

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore() 
            => new TargetUpdatedEventBehavior();

        private static DependencyProperty RegisterAttachment()
        {
            TargetUpdatedEventBehavior behavior = new();

            return RegisterAttachment(NameOf.ReadAccessorEnabledDependencyPropertyName(() => ActionsProperty),
                                      typeof(TargetUpdatedEventBehavior),
                                      behavior.DefaultMetadata);
        }
    }
}