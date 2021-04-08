//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media.Animation;
using BadEcho.Fenestra.Properties;
using BadEcho.Odin.Extensions;

namespace BadEcho.Fenestra.Behaviors
{
    /// <summary>
    /// Provides an infrastructure for attached properties that, when attached to a target object, directly influence the state and
    /// functioning of said object.
    /// </summary>
    /// <typeparam name="TTarget">The type of <see cref="DependencyObject"/> this behavior attaches to.</typeparam>
    /// <typeparam name="TProperty">The type of value accepted by this behavior as an attached property.</typeparam>
    public abstract class Behavior<TTarget,TProperty> : Animatable
        where TTarget: DependencyObject
    {
        private TTarget? _targetObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="Behavior{TTarget,TProperty}"/> class.
        /// </summary>
        protected Behavior()
            => DefaultMetadata = new PropertyMetadata(default(TProperty), OnAttachChanged);

        /// <summary>
        /// Gets the default property metadata to use when registering this behavior as an attached property.
        /// </summary>
        public PropertyMetadata DefaultMetadata
        { get; }

        /// <summary>
        /// Gets the target dependency object this behavior is attached to.
        /// </summary>
        protected TTarget? TargetObject
        {
            get
            {
                ReadPreamble();
                return _targetObject;
            }
            private set
            {
                if (TargetObject.Equals<TTarget>(value))
                    return;

                if (TargetObject != null)
                    throw new InvalidOperationException(Strings.BehaviorCannotTargetMultipleObjects);

                WritePreamble();
                _targetObject = value;
                WritePostscript();
            }
        }

        /// <summary>
        /// Called when this behavior is being attached to a <see cref="DependencyObject"/> instance with the provided
        /// value.
        /// </summary>
        /// <param name="newValue">The value of the attached property.</param>
        protected abstract void OnAttaching(TProperty newValue);

        /// <summary>
        /// Called when this behavior is being detached from a <see cref="DependencyObject"/> instance that it was attached
        /// to previously with the provided value.
        /// </summary>
        /// <param name="oldValue">The value that the attached property was previously set to.</param>
        protected abstract void OnDetaching(TProperty oldValue);

        private void OnAttachChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is not TTarget targetObject)
            {
                throw new InvalidOperationException(
                    Strings.BehaviorUnsupportedTargetObject.InvariantFormat(GetType(), typeof(TTarget)));
            }

            if (args.NewValue == args.OldValue)
                return;
            
            TargetObject = targetObject;

            TProperty? newValue = (TProperty?) args.NewValue;

            if (args.OldValue is TProperty oldValue) 
                OnDetaching(oldValue);

            if (newValue == null)
                TargetObject = null;
            else
                OnAttaching(newValue);
        }
    }
}