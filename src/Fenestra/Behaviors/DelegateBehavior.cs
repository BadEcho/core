//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using BadEcho.Odin;

namespace BadEcho.Fenestra.Behaviors
{
    /// <summary>
    /// Provides a behavior that will execute a method once its required parameter information is set on the target dependency
    /// object which the behavior is attached to.
    /// </summary>
    /// <typeparam name="TTarget">The type of <see cref="DependencyObject"/> this behavior attaches to.</typeparam>
    /// <typeparam name="TParameter">The type of parameter accepted by method executed by this behavior.</typeparam>
    public sealed class DelegateBehavior<TTarget,TParameter> : Behavior<TTarget, TParameter>
        where TTarget : DependencyObject
    {
        private readonly Action<TTarget, TParameter> _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateBehavior{TTarget,TParameter}"/> class.
        /// </summary>
        /// <param name="action">
        /// The method to execute upon parameter information becoming associated with this behavior.
        /// </param>
        public DelegateBehavior(Action<TTarget, TParameter> action)
        {
            Require.NotNull(action, nameof(action));

            _action = action;
        }

        /// <inheritdoc/>
        protected override void OnValueAssociated(TTarget targetObject, TParameter newValue) 
            => _action(targetObject, newValue);

        /// <inheritdoc/>
        protected override void OnValueDisassociated(TTarget targetObject, TParameter oldValue)
        { }

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
            => new DelegateBehavior<TTarget, TParameter>(_action);
    }
}
