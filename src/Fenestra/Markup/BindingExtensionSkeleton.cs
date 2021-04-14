//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace BadEcho.Fenestra.Markup
{
    /// <summary>
    /// Provides a skeletal implementation of a binding-related markup extension.
    /// </summary>
    /// <typeparam name="TBinding">
    /// A Microsoft-provided binding type that provides the actual binding mechanics being extended.
    /// </typeparam>
    /// <typeparam name="TValueConverter">The value converter type to be used by the binding.</typeparam>
    [MarkupExtensionReturnType(typeof(object))]
    [Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable, Readability = Readability.Unreadable)]
    public abstract class BindingExtensionSkeleton<TBinding, TValueConverter> : MarkupExtension
        where TBinding : BindingBase
    {
        /// <summary>
        /// Gets or sets the name of the <see cref="BindingGroup"/> to which this binding belongs.
        /// </summary>
        [DefaultValue("")]
        public string BindingGroupName
        {
            get => ActualBinding.BindingGroupName;
            set => ActualBinding.BindingGroupName = value;
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, to wait before updating the binding source after the value on the
        /// target changes.
        /// </summary>
        /// <remarks>
        /// This property only affects <see cref="BindingMode.TwoWay"/> bindings that use property change triggered updates.
        /// </remarks>
        [DefaultValue(0)]
        public int Delay
        {
            get => ActualBinding.Delay;
            set => ActualBinding.Delay = value;
        }

        /// <summary>
        /// Gets or sets the value to use when the binding is unable to return a value.
        /// </summary>
        public object FallbackValue
        {
            get => ActualBinding.FallbackValue;
            set => ActualBinding.FallbackValue = value;
        }

        /// <summary>
        /// Gets or sets the value that is used in the target when the value of the source is null.
        /// </summary>
        public object TargetNullValue
        {
            get => ActualBinding.TargetNullValue;
            set => ActualBinding.TargetNullValue = value;
        }

        /// <summary>
        /// Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.
        /// </summary>
        [DefaultValue(null)]
        public string StringFormat
        {
            get => ActualBinding.StringFormat;
            set => ActualBinding.StringFormat = value;
        }

        /// <summary>
        /// Gets or sets the converter to use.
        /// </summary>
        [DefaultValue(null)]
        public abstract TValueConverter Converter
        { get; set; }

        /// <summary>
        /// Gets or sets the culture in which to evaluate the converter.
        /// </summary>
        [DefaultValue(null)]
        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        public abstract CultureInfo ConverterCulture
        { get; set; }

        /// <summary>
        /// Gets or sets the parameter to pass to the <see cref="Converter"/>.
        /// </summary>
        [DefaultValue(null)]
        public abstract object ConverterParameter
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the direction of the data flow in the binding.
        /// </summary>
        [DefaultValue(BindingMode.Default)]
        public abstract BindingMode Mode
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to raise a source updated event when a value is transferred from
        /// the binding target to the binding source.
        /// </summary>
        [DefaultValue(false)]
        public abstract bool NotifyOnSourceUpdated
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to raise a target updated event when a value is transferred from the
        /// binding source to the binding target.
        /// </summary>
        [DefaultValue(false)]
        public abstract bool NotifyOnTargetUpdated
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to raise the <see cref="Validation.ErrorEvent"/> attached event on the bound object.
        /// </summary>
        [DefaultValue(false)]
        public abstract bool NotifyOnValidationError
        { get; set; }

        /// <summary>
        /// Gets or sets a handler you can use to provide custom logic for handling exceptions that the binding engine encounters during the
        /// update of the binding source value. This is only applicable if you have an associated <see cref="ExceptionValidationRule"/> with
        /// your binding.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public abstract UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
        { get; set; }

        /// <summary>
        /// Gets or sets a value that determines the timing of binding source updates.
        /// </summary>
        [DefaultValue(UpdateSourceTrigger.Default)]
        public abstract UpdateSourceTrigger UpdateSourceTrigger
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the <see cref="DataErrorValidationRule"/>.
        /// </summary>
        [DefaultValue(false)]
        public abstract bool ValidatesOnDataErrors
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the <see cref="ExceptionValidationRule"/>.
        /// </summary>
        [DefaultValue(false)]
        public abstract bool ValidatesOnExceptions
        { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the <see cref="NotifyDataErrorValidationRule"/>.
        /// </summary>
        [DefaultValue(true)]
        public abstract bool ValidatesOnNotifyDataErrors
        { get; set; }

        /// <summary>
        /// Gets a collection of rules that check the validity of the user input.
        /// </summary>
        public abstract Collection<ValidationRule> ValidationRules
        { get; }

        /// <summary>
        /// Gets a value that indicates if a binding's extension requires that a valid target dependency object be present.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Data binding mechanisms will only be extended by this type if a target <see cref="DependencyObject"/> is found. Execution
        /// will be deferred to WPF's built-in data binding routines for proper handling if no target object has been made available.
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
        public override object ProvideValue([AllowNull]IServiceProvider serviceProvider)
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

        private bool CanExtendBinding(DependencyObject? targetObject, [NotNullWhen(true)]DependencyProperty? targetProperty)
        {
            if (targetProperty == null)
                return false;

            if (targetObject == null && IsTargetRequired)
                return false;

            return targetObject == null || !DesignerProperties.GetIsInDesignMode(targetObject);
        }
    }
}