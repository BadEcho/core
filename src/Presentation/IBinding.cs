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

using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BadEcho.Presentation;

/// <summary>
/// Defines a connection formed between a data source and the properties of a target object.
/// </summary>
public interface IBinding
{
    /// <summary>
    /// Gets or sets the name of the <see cref="BindingGroup"/> to which this binding belongs.
    /// </summary>
    string BindingGroupName { get; set; }

    /// <summary>
    /// Gets or sets the amount of time, in milliseconds, to wait before updating the binding source after the value on the
    /// target changes.
    /// </summary>
    /// <remarks>
    /// This property only affects <see cref="BindingMode.TwoWay"/> bindings that use property change triggered updates.
    /// </remarks>
    int Delay { get; set; }

    /// <summary>
    /// Gets or sets the value to use when the binding is unable to return a value.
    /// </summary>
    object FallbackValue { get; set; }

    /// <summary>
    /// Gets or sets the value that is used in the target when the value of the source is null.
    /// </summary>
    object TargetNullValue { get; set; }

    /// <summary>
    /// Gets or sets a string that specifies how to format the binding if it displays the bound value as a string.
    /// </summary>
    string? StringFormat { get; set; }

    /// <summary>
    /// Gets or sets the culture in which to evaluate the converter.
    /// </summary>
    CultureInfo? ConverterCulture { get; set; }

    /// <summary>
    /// Gets or sets the parameter to pass to any value converter in use.
    /// </summary>
    object? ConverterParameter { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the direction of the data flow in the binding.
    /// </summary>
    BindingMode Mode { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to raise a source updated event when a value is transferred from
    /// the binding target to the binding source.
    /// </summary>
    bool NotifyOnSourceUpdated { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to raise a target updated event when a value is transferred from the
    /// binding source to the binding target.
    /// </summary>
    bool NotifyOnTargetUpdated { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to raise the <see cref="Validation.ErrorEvent"/> attached event on the bound object.
    /// </summary>
    bool NotifyOnValidationError { get; set; }

    /// <summary>
    /// Gets or sets a handler you can use to provide custom logic for handling exceptions that the binding engine encounters during the
    /// update of the binding source value. This is only applicable if you have an associated <see cref="ExceptionValidationRule"/> with
    /// your binding.
    /// </summary>
    UpdateSourceExceptionFilterCallback? UpdateSourceExceptionFilter { get; set; }

    /// <summary>
    /// Gets or sets a value that determines the timing of binding source updates.
    /// </summary>
    UpdateSourceTrigger UpdateSourceTrigger { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to include the <see cref="DataErrorValidationRule"/>.
    /// </summary>
    bool ValidatesOnDataErrors { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to include the <see cref="ExceptionValidationRule"/>.
    /// </summary>
    bool ValidatesOnExceptions { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates whether to include the <see cref="NotifyDataErrorValidationRule"/>.
    /// </summary>
    bool ValidatesOnNotifyDataErrors { get; set; }

    /// <summary>
    /// Gets a collection of rules that check the validity of the user input.
    /// </summary>
    Collection<ValidationRule> ValidationRules { get; }

    /// <summary>
    /// Clears any value converter associated with this binding.
    /// </summary>
    /// <remarks>
    /// A method is required for resetting associated converters as there is no converter-related property exposed
    /// at the interface level. This is because value converters either implement <see cref="IValueConverter"/> or
    /// <see cref="IMultiValueConverter"/>, two interfaces which are unrelated to each other.
    /// </remarks>
    void ClearConverter();

    /// <summary>
    /// Performs a binding-related action, which is an action that concerns the propagation of an input value to
    /// a target property, in a safe manner.
    /// </summary>
    /// <param name="bindingAction">An action that concerns the propagation of an input value to a target property.</param>
    /// <returns>Value indicating if the binding action succeeded.</returns>
    /// <remarks>
    /// <para>
    /// If any code must interface with the data being inputted for binding, they must do so through this method. It is important
    /// for us to adopt the same approach that Microsoft uses in their own data binding logic when dealing with user input;
    /// specifically: <c>all</c> exceptions must be caught. There is no application code on the stack when actual binding code is ran, so
    /// any exceptions thrown are not actionable by the application.
    /// </para>
    /// <para>
    /// This means that unless we catch the exceptions here, the only possible outcome is for the application to crash; therefore,
    /// this becomes one of those rare times when the absolute best course of action is to "swallow" exceptions. We don't simply ignore
    /// them, however. Instead, we make use of the data binding exception handling system that WPF makes available, and log all
    /// occurrences.
    /// </para>
    /// </remarks>
    bool DoBindingAction(Func<bool> bindingAction);

    /// <summary>
    /// Performs a binding-related action, which is an action that concerns the propagation of an input value to
    /// a target property, in a safe manner.
    /// </summary>
    /// <param name="bindingAction">An action that concerns the propagation of an input value to a target property.</param>
    /// <remarks>
    /// <para>
    /// If any code must interface with the data being inputted for binding, they must do so through this method. It is important
    /// for us to adopt the same approach that Microsoft uses in their own data binding logic when dealing with user input;
    /// specifically: <c>all</c> exceptions must be caught. There is no application code on the stack when actual binding code is ran, so
    /// any exceptions thrown are not actionable by the application.
    /// </para>
    /// <para>
    /// This means that unless we catch the exceptions here, the only possible outcome is for the application to crash; therefore,
    /// this becomes one of those rare times when the absolute best course of action is to "swallow" exceptions. We don't simply ignore
    /// them, however. Instead, we make use of the data binding exception handling system that WPF makes available, and log all
    /// occurrences.
    /// </para>
    /// </remarks>
    void DoBindingAction(Action bindingAction);

    /// <summary>
    /// Creates and associates a new instance of <see cref="BindingExpressionBase"/> with the specified binding target property.
    /// </summary>
    /// <param name="targetObject">The target dependency object containing the property to bind.</param>
    /// <param name="targetProperty">The target dependency property to bind.</param>
    /// <returns>The created <see cref="BindingExpressionBase"/> instance.</returns>
    BindingExpressionBase SetBinding(DependencyObject targetObject, DependencyProperty targetProperty);
}