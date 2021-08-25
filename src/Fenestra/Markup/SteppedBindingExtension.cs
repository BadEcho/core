//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
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
        public SteppedBindingExtension(PropertyPath path) 
            : base(path)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SteppedBindingExtension"/> class.
        /// </summary>
        public SteppedBindingExtension()
        { }

        /// <summary>
        /// Gets or sets the total duration of a binding update stepping sequence.
        /// </summary>
        public TimeSpan SteppingDuration
        { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of steps required in order for a stepping sequence to be executed.
        /// </summary>
        /// <remarks>
        /// If the number of steps falls short of this setting, then any binding update changes are immediately propagated
        /// to the target property in a single step. Setting this equal to or less than its default value of 0 results in
        /// a stepping sequence always being executed in response to a change, regardless of the number of steps involved.
        /// </remarks>
        public int MinimumSteps 
        { get; set; }

        /// <inheritdoc/>
        protected override object ExtendBinding(IServiceProvider serviceProvider, DependencyObject? targetObject, DependencyProperty targetProperty)
        {
            if (targetObject == null)
                return this;

            UpdateBindingMode(targetObject, targetProperty);

            var binder = new SteppedBinder(targetObject,
                                           targetProperty,
                                           new SteppingOptions(SteppingDuration, MinimumSteps, this));
            
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