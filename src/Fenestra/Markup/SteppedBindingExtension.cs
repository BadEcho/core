//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Data;

namespace BadEcho.Fenestra.Markup
{
    /// <summary>
    /// Provides a binding markup extension that will connect the properties of target objects and any data source such that
    /// changes in a source numeric value will be propagated to the target property in a stepped, incremental fashion.
    /// </summary>
    /// <remarks>
    /// The described stepped mechanism of change is applied in all appropriate directions as specified by the value 
    /// <see cref="BindingExtension.Mode"/> is set to.
    /// </remarks>
    public sealed class SteppedBindingExtension : BindingExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SteppedBindingExtension"/> class.
        /// </summary>
        /// <param name="path">The path to the binding source property.</param>
        public SteppedBindingExtension(string path) 
            : base(path)
        { }

        /// <summary>
        /// Gets or sets the total duration of a binding update stepping sequence.
        /// </summary>
        public TimeSpan SteppingDuration
        { get; set; }

        /// <inheritdoc/>
        protected override object ExtendBinding(IServiceProvider serviceProvider, DependencyObject? targetObject, DependencyProperty targetProperty)
        {
            if (targetObject == null)
                return this;

            UpdateBindingMode(targetObject, targetProperty);

            var binder = new SteppedBinder(targetObject, targetProperty, ActualBinding, SteppingDuration);

            FreezableCollection<TransientBinder> binders = TransientBinder.GetBinders(targetObject);

            binders.Add(binder);

            return targetObject.GetValue(targetProperty);
        }

        private void UpdateBindingMode(DependencyObject targetObject, DependencyProperty targetProperty)
        {
            if (Mode != BindingMode.Default)
                return;

            Mode = targetProperty.GetMetadata(targetObject.GetType()) is FrameworkPropertyMetadata {BindsTwoWayByDefault: true}
                ? BindingMode.TwoWay
                : BindingMode.OneWay;
        }
    }
}