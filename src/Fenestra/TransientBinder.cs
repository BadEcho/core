//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Data;
using BadEcho.Odin;
using BadEcho.Odin.Extensions;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides a base freezable object that facilitates that realization of the impermanent state shared between source and
    /// target.
    /// </summary>
    /// <remarks>
    /// In other words, this is an object that can be attached to a <see cref="DependencyObject"/> to influence what actually
    /// happens during a binding update. This class is abstract because, while it would be fully functional on its own, simply
    /// attaching a <see cref="TransientBinder"/> instance would result in completely vanilla binding update behavior. Derive
    /// from this class in order to customize what exactly occurs during source-to-target and target-to-source binding updates.
    /// </remarks>
    internal abstract class TransientBinder : Freezable, IHandlerBypassable
    {
        /// <summary>
        /// Identifies the attached property that gets or sets a <see cref="DependencyObject"/> instance's collection of custom
        /// binders.
        /// </summary>
        public static readonly DependencyProperty BindersProperty
            = DependencyProperty.RegisterAttached(NameOf.ReadDependencyPropertyName(() => BindersProperty),
                                                  typeof(FreezableCollection<TransientBinder>),
                                                  typeof(TransientBinder));
        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Source),
                                          typeof(object),
                                          typeof(TransientBinder),
                                          new PropertyMetadata(OnSourceChanged));
        /// <summary>
        /// Identifies the <see cref="Target"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetProperty
            = DependencyProperty.Register(nameof(Target),
                                          typeof(object),
                                          typeof(TransientBinder),
                                          new PropertyMetadata(OnTargetChanged));

        private readonly string _stringFormat;
        private readonly BindingMode _mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientBinder"/> class.
        /// </summary>
        /// <param name="targetObject">The target dependency object containing the property to bind.</param>
        /// <param name="targetProperty">The target dependency property to bind.</param>
        /// <param name="binding">The underlying binding to augment.</param>
        /// <param name="mode">The mode of binding being used.</param>
        protected TransientBinder(DependencyObject targetObject, DependencyProperty targetProperty, BindingBase binding, BindingMode mode)
        {
            Require.NotNull(targetProperty, nameof(targetProperty));
            Require.NotNull(binding, nameof(binding));

            _mode = mode;

            var targetBinding = new Binding
                                {
                                    Source = targetObject,
                                    Path = new PropertyPath(targetProperty),
                                    Mode = BindingMode.TwoWay
                                };

            _stringFormat = binding.StringFormat ?? "{0}";

            this.BypassHandlers(() =>
                                {
                                    BindingOperations.SetBinding(this, TargetProperty, targetBinding);
                                    BindingOperations.SetBinding(this, SourceProperty, binding);
                                });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientBinder"/> class.
        /// </summary>
        protected TransientBinder()
            => _stringFormat = "{0}";

        /// <summary>
        /// Gets or sets the object to use as the binding source.
        /// </summary>
        public object? Source
        { get; set; }

        /// <summary>
        /// Gets or sets the object to use as the binding target.
        /// </summary>
        public object? Target
        { get; set; }

        /// <summary>
        /// Gets the value of the <see cref="BindersProperty"/> attached property from a given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="source">The dependency object from which to read the property value.</param>
        /// <returns>The collection of custom binders attached to <c>source</c>.</returns>
        public static FreezableCollection<TransientBinder> GetBinders(DependencyObject source)
        {
            Require.NotNull(source, nameof(source));

            if (source.GetValue(BindersProperty) is not FreezableCollection<TransientBinder> binders)
            {
                binders = new FreezableCollection<TransientBinder>();

                SetBinders(source, binders);
            }

            return binders;
        }

        /// <summary>
        /// Sets the value of the <see cref="BindersProperty"/> attached property to a given <see cref="DependencyObject"/>.
        /// </summary>
        /// <param name="source">The dependency object on which to set the attached property.</param>
        /// <param name="value">The collection of custom binders to set.</param>
        public static void SetBinders(DependencyObject source, FreezableCollection<TransientBinder> value)
        {
            Require.NotNull(source, nameof(source));
            Require.NotNull(value, nameof(value));

            source.SetValue(BindersProperty, value);
        }

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
            => CreateBinder();

        /// <summary>
        /// Called in response to a change in the source property that will, by default, update the target property.
        /// </summary>
        protected virtual void OnSourceChanged()
        {
            object sourceValue = GetValue(SourceProperty);

            if (sourceValue != GetValue(TargetProperty))
            {
                this.BypassHandlers(() => WriteTargetValue(sourceValue));
            }
        }

        /// <summary>
        /// Called in response to a change in the target property that will, by default, update the source property.
        /// </summary>
        protected virtual void OnTargetChanged()
        {
            object targetValue = GetValue(TargetProperty);

            if (targetValue != GetValue(SourceProperty))
            {
                this.BypassHandlers(() => WriteSourceValue(targetValue));
            }
        }

        /// <summary>
        /// Commits the provided value to the target property.
        /// </summary>
        /// <param name="value">The value to set the target property to.</param>
        /// <remarks>
        /// This method is executed by <see cref="TransientBinder"/> within a context where event handlers are bypassed, meaning
        /// that any changes to the target property's value will not result in a binding update if one were to go off.
        /// </remarks>
        protected virtual void WriteTargetValue(object value)
            => SetValue(TargetProperty, _stringFormat.CulturedFormat(value));

        /// <summary>
        /// Commits the provided value to the source property.
        /// </summary>
        /// <param name="value">The value to set the source property to.</param>
        /// <remarks>
        /// This method is executed by <see cref="TransientBinder"/> within a context where event handlers are bypassed, meaning
        /// that any changes to the source property's value will not result in a binding update if one were to go off.
        /// </remarks>
        protected virtual void WriteSourceValue(object value)
        {
            SetValue(SourceProperty, value);

            BindingExpressionBase? bindingExpression = BindingOperations.GetBindingExpressionBase(this, SourceProperty);

            bindingExpression?.UpdateSource();
        }

        /// <summary>
        /// Creates a new instance of this binder, used internally by WPF.
        /// </summary>
        /// <returns>The new instance of the binder.</returns>
        protected abstract Freezable CreateBinder();

        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TransientBinder binder = (TransientBinder) sender;

            if (binder._mode == BindingMode.OneWayToSource)
                return;

            if (binder.IsHandlingBypassed())
                return;

            binder.OnSourceChanged();
        }

        private static void OnTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TransientBinder binder = (TransientBinder) sender;
            
            if (binder._mode is not BindingMode.TwoWay and not BindingMode.OneWayToSource)
                return;

            if (binder.IsHandlingBypassed())
                return;

            binder.OnTargetChanged();
        }
    }
}