//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using BadEcho.Presentation.Properties;
using BadEcho.Logging;

namespace BadEcho.Presentation.Markup;

/// <summary>
/// Provides a skeletal implementation of a binding-related markup extension.
/// </summary>
/// <typeparam name="TBinding">
/// A Microsoft-provided binding type that provides the actual binding mechanics being extended.
/// </typeparam>
/// <typeparam name="TValueConverter">The value converter type to be used by the binding.</typeparam>
[MarkupExtensionReturnType(typeof(object))]
[Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable, Readability = Readability.Unreadable)]
public abstract class BindingExtensionSkeleton<TBinding, TValueConverter> : MarkupExtension, IBinding
    where TBinding : BindingBase
{
    private BindingExpressionBase? _bindingExpression;

    /// <inheritdoc/>
    [DefaultValue("")]
    public string BindingGroupName
    {
        get => ActualBinding.BindingGroupName;
        set => ActualBinding.BindingGroupName = value;
    }

    /// <inheritdoc/>
    [DefaultValue(0)]
    public int Delay
    {
        get => ActualBinding.Delay;
        set => ActualBinding.Delay = value;
    }

    /// <inheritdoc/>
    public object FallbackValue
    {
        get => ActualBinding.FallbackValue;
        set => ActualBinding.FallbackValue = value;
    }

    /// <inheritdoc/>
    public object TargetNullValue
    {
        get => ActualBinding.TargetNullValue;
        set => ActualBinding.TargetNullValue = value;
    }

    /// <inheritdoc/>
    [DefaultValue(null)]
    public string? StringFormat
    {
        get => ActualBinding.StringFormat;
        set => ActualBinding.StringFormat = value;
    }

    /// <summary>
    /// Gets or sets the converter to use.
    /// </summary>
    [DefaultValue(null)]
    public abstract TValueConverter? Converter
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(null)]
    [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
    public abstract CultureInfo? ConverterCulture
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(null)]
    public abstract object? ConverterParameter
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(BindingMode.Default)]
    public abstract BindingMode Mode
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(false)]
    public abstract bool NotifyOnSourceUpdated
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(false)]
    public abstract bool NotifyOnTargetUpdated
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(false)]
    public abstract bool NotifyOnValidationError
    { get; set; }

    /// <inheritdoc/>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public abstract UpdateSourceExceptionFilterCallback? UpdateSourceExceptionFilter
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(UpdateSourceTrigger.Default)]
    public abstract UpdateSourceTrigger UpdateSourceTrigger
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(false)]
    public abstract bool ValidatesOnDataErrors
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(false)]
    public abstract bool ValidatesOnExceptions
    { get; set; }

    /// <inheritdoc/>
    [DefaultValue(true)]
    public abstract bool ValidatesOnNotifyDataErrors
    { get; set; }

    /// <inheritdoc/>
    public abstract Collection<ValidationRule> ValidationRules
    { get; }

    /// <summary>
    /// Gets a value that indicates if a binding's extension requires that a valid target dependency object be present.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Data binding mechanisms will only be extended by this type if a target <see cref="DependencyObject"/> is found. Execution
    /// will be deferred to the built-in data binding routines of WPF for proper handling if no target object has been made available.
    /// If, however, a derived class overrides this property to return true, the binding extension is allowed to continue, even if
    /// no <see cref="DependencyObject"/> is present. This can be most useful when there is a need to fulfill an auxiliary type of
    /// function such as further configuration of the binder.
    /// </para>
    /// <para>
    /// Regardless of what further actions are taken, basic WPF data binding design guidelines must be adhered to. If a binding
    /// is not being applied to a reachable <see cref="DependencyObject"/>, such as when the binding is being applied to a
    /// template, the binding itself should be returned by <see cref="ProvideValue(IServiceProvider)"/>. This will allow for
    /// WPF to execute the binding again at a later point in time when a <see cref="DependencyObject"/> might be available,
    /// such as when the aforementioned template gets applied.
    /// </para>
    /// </remarks>
    protected virtual bool IsTargetRequired
        => true;

    /// <summary>
    /// Gets the binding that actually provides the binding mechanics being extended by this markup extension.
    /// </summary>
    protected abstract TBinding ActualBinding
    { get; }

    /// <inheritdoc/>
    public override object ProvideValue(IServiceProvider? serviceProvider)
    {
        if (serviceProvider == null)
        {  
            // WPF passes a null service provider when it wishes to defer execution of the binding until later.
            // For example, when it is cloning dynamic resources and other markup extensions to another object.
            // Returning the binding itself here allows for WPF to call into it again once some data actually needs
            // to be transferred. The more you know!
            return this;
        }
            
        IProvideValueTarget? valueProvider
            = (IProvideValueTarget?) serviceProvider.GetService(typeof(IProvideValueTarget));
            
        if (valueProvider == null)
        {
            // While it is not entirely clear from the WPF source as to when or why the IProvideValueTarget service might not
            // be locatable, it is an eventuality that WPF handles by returning the binding itself to allow for its future execution.
            return this;
        }

        DependencyObject? targetObject = valueProvider.TargetObject as DependencyObject;
        DependencyProperty? targetProperty = valueProvider.TargetProperty as DependencyProperty;

        return CanExtendBinding(targetObject, targetProperty)
            ? ExtendBinding(serviceProvider, targetObject, targetProperty)
            : ActualBinding.ProvideValue(serviceProvider);
    }

    /// <inheritdoc/>
    public void ClearConverter() 
        => Converter = default;

    /// <inheritdoc/>
    public bool DoBindingAction(Func<bool> bindingAction)
    {
        Require.NotNull(bindingAction, nameof(bindingAction));

        try
        {
            return bindingAction();
        }
        catch (Exception ex)
        {
            ProcessExceptionValidation(ex);

            Logger.Error(Strings.BindingActionError, ex);
            return false;
        }
    }

    /// <inheritdoc/>
    public void DoBindingAction(Action bindingAction)
    {
        Require.NotNull(bindingAction, nameof(bindingAction));

        try
        {
            bindingAction();
        }
        catch (Exception ex)
        {
            ProcessExceptionValidation(ex);

            Logger.Error(Strings.BindingActionError, ex);
        }
    }

    /// <inheritdoc/>
    public BindingExpressionBase SetBinding(DependencyObject targetObject, DependencyProperty targetProperty) 
        => _bindingExpression = BindingOperations.SetBinding(targetObject, targetProperty, ActualBinding);

    /// <summary>
    /// Creates and returns a binding with extended mechanics for the provided target object and property.
    /// </summary>
    /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
    /// <param name="targetObject">The target dependency object containing the property to bind.</param>
    /// <param name="targetProperty">The target dependency property to bind.</param>
    /// <returns>An extended binding for <c>targetProperty</c> found on <c>targetObject</c>.</returns>
    protected abstract object ExtendBinding(IServiceProvider serviceProvider,
                                            DependencyObject? targetObject,
                                            DependencyProperty targetProperty);

    private void ProcessExceptionValidation(Exception ex)
    {
        ValidationError? validationError = null;
        object? filteredException = null;

        // Interestingly, WPF seems to ignore ValidatesOnExceptions and adds a validation error anyway if it has managed to retrieve
        // a validation error from an exception filter. For ourselves, we'll only proceed with adding a validation error for a thrown
        // exception if the ValidatesOnExceptions property is set to true, or if we're part of a binding group.
        if (_bindingExpression == null || !ValidatesOnExceptions && string.IsNullOrEmpty(BindingGroupName))
            return;

        if (UpdateSourceExceptionFilter != null)
        {
            filteredException = UpdateSourceExceptionFilter(ActualBinding, ex);

            if (filteredException == null)
                return;

            validationError = filteredException as ValidationError;
        }

        if (validationError == null)
        {
            var validationRule = new ExceptionValidationRule();

            validationError = filteredException == null
                ? new ValidationError(validationRule, ActualBinding, ex.Message, ex)
                : new ValidationError(validationRule, ActualBinding, filteredException, ex);
        }

        Validation.MarkInvalid(_bindingExpression, validationError);
    }

    private bool CanExtendBinding(DependencyObject? targetObject, [NotNullWhen(true)]DependencyProperty? targetProperty)
    {
        if (targetProperty == null)
            return false;

        if (targetObject == null && IsTargetRequired)
            return false;

        return targetObject == null || !DesignerProperties.GetIsInDesignMode(targetObject);
    }
}