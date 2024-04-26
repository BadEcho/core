//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------
    
using System.Windows;
using System.Windows.Data;

namespace BadEcho.Presentation;

/// <summary>
/// Provides a base freezable object that facilitates the realization of the impermanent state shared between source and
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

    /// <summary>
    /// Initializes a new instance of the <see cref="TransientBinder"/> class.
    /// </summary>
    /// <param name="targetObject">The target dependency object containing the property to bind.</param>
    /// <param name="targetProperty">The target dependency property to bind.</param>
    /// <param name="options">Options related to the binding.</param>
    protected TransientBinder(DependencyObject targetObject, DependencyProperty targetProperty, TransientOptions options)
    {
        Require.NotNull(options, nameof(options));

        Binding = options.Binding;

        // We copy the current value for the target property on the target object to this binder's own dependency property.
        // Otherwise, any previously set value for the target property on the target object will be lost when binding this object
        // to it.
        this.BypassHandlers(
            () => SetValue(TargetProperty, targetObject.GetValue(targetProperty)));

        var targetBinding = new Binding
                            {
                                Source = this,
                                Path = new PropertyPath(TargetProperty),
                                Converter = options.Converter,
                                Mode = BindingMode.TwoWay
                            };

        // Any converter found on the original binding has been moved over to the new target binding.
        // We clear the converter on the source binding so that we have access to the original values.
        options.Binding.ClearConverter();

        this.BypassHandlers(() =>
                            {
                                BindingOperations.SetBinding(targetObject, targetProperty, targetBinding);
                                options.Binding.SetBinding(this, SourceProperty);
                            });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TransientBinder"/> class.
    /// </summary>
    protected TransientBinder() 
    { }

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
    /// Gets the underlying binding being augmented.
    /// </summary>
    protected IBinding? Binding
    { get; private set; }

    /// <summary>
    /// Gets the value of the <see cref="BindersProperty"/> attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="source">The dependency object from which the property value is read.</param>
    /// <returns>The collection of custom binders attached to <c>source</c>.</returns>
    public static FreezableCollection<TransientBinder> GetBinders(DependencyObject source)
    {
        Require.NotNull(source, nameof(source));

        if (source.GetValue(BindersProperty) is not FreezableCollection<TransientBinder> binders)
        {
            binders = [];

            SetBinders(source, binders);
        }

        return binders;
    }

    /// <summary>
    /// Sets the value of the <see cref="BindersProperty"/> attached property on a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="source">The dependency object to which the attached property is written.</param>
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

    /// <inheritdoc/>
    /// <remarks>
    /// I'm having issues finding any Microsoft-provided freezables that have any difference at all between <see cref="CloneCore(Freezable)"/>
    /// and <see cref="CloneCurrentValueCore(Freezable)"/>. We only begin to see differences when we start to look at the base clone methods,
    /// defined in <see cref="Freezable"/>. An impressive amount of redundant code!
    /// </remarks>
    protected override void CloneCore(Freezable sourceFreezable)
    {
        base.CloneCore(sourceFreezable);

        Clone(sourceFreezable);
    }

    /// <inheritdoc/>
    /// <seealso cref="CloneCore(Freezable)"/>
    protected override void CloneCurrentValueCore(Freezable sourceFreezable)
    {
        base.CloneCurrentValueCore(sourceFreezable);

        Clone(sourceFreezable);
    }

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
    /// This method should be executed by <see cref="TransientBinder"/> within a context where event handlers are bypassed,
    /// so that any changes to the target property's value will not result in a binding update if one were to go off.
    /// </remarks>
    protected virtual void WriteTargetValue(object value)
        => SetValue(TargetProperty, value);

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

        if (binder.Binding?.Mode == BindingMode.OneWayToSource)
            return;

        if (binder.IsHandlingBypassed())
            return;

        binder.OnSourceChanged();
    }

    private static void OnTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        TransientBinder binder = (TransientBinder) sender;

        if (binder.Binding?.Mode is not BindingMode.TwoWay and not BindingMode.OneWayToSource)
            return;

        if (binder.IsHandlingBypassed())
            return;

        binder.OnTargetChanged();

    }
        
    private void Clone(Freezable sourceFreezable)
    {
        var binder = (TransientBinder)sourceFreezable;

        binder.Binding = Binding;
    }
}