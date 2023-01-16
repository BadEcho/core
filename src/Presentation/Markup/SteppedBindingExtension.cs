//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Data;

namespace BadEcho.Presentation.Markup;

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
    /// Gets or sets the amount of change incurred by a single step.
    /// </summary>
    public double StepAmount
    { get; set; } = 1.0;

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

    /// <summary>
    /// Gets or sets a value indicating if the target property is an integer, as opposed to a floating-point number.
    /// </summary>
    /// <remarks>
    /// Unless this is set to true, the target property will be bound with values that are a floating-point type, which,
    /// in comparison to integers, is a type that's more prevalent in WPF.
    /// </remarks>
    public bool IsInteger
    { get; set; }

    /// <inheritdoc/>
    protected override object ExtendBinding(IServiceProvider serviceProvider, DependencyObject? targetObject, DependencyProperty targetProperty)
    {
        if (targetObject == null)
            return this;

        UpdateBindingMode(targetObject, targetProperty);
            
        var binder = new SteppedBinder(targetObject,
                                       targetProperty,
                                       new SteppingOptions(this)
                                       {
                                           SteppingDuration = SteppingDuration,
                                           MinimumSteps = MinimumSteps,
                                           IsInteger = IsInteger,
                                           StepAmount = StepAmount,
                                           Converter = Converter
                                       });
            
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