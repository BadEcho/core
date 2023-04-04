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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BadEcho.Presentation.Markup;

/// <summary>
/// Provides a base class for a markup extension that defines a binding, connecting the properties of target objects and
/// any data source.
/// </summary>
/// <remarks>
/// Deriving from this class allows one to extend the mechanics of an applied <see cref="Binding"/>.
/// </remarks>
public abstract class BindingExtension : BindingExtensionSkeleton<Binding, IValueConverter>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BindingExtension"/> class.
    /// </summary>
    /// <param name="path">The path to the binding source property.</param>
    protected BindingExtension(PropertyPath path) 
        => Path = path;

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingExtension"/> class.
    /// </summary>
    protected BindingExtension()
    { }

    /// <inheritdoc/>
    public override IValueConverter? Converter
    {
        get => ActualBinding.Converter;
        set => ActualBinding.Converter = value;
    }

    /// <inheritdoc/>
    public override CultureInfo? ConverterCulture
    {
        get => ActualBinding.ConverterCulture;
        set => ActualBinding.ConverterCulture = value;
    }

    /// <inheritdoc/>
    public override object? ConverterParameter
    {
        get => ActualBinding.ConverterParameter;
        set => ActualBinding.ConverterParameter = value;
    }

    /// <inheritdoc/>
    public override BindingMode Mode
    {
        get => ActualBinding.Mode;
        set => ActualBinding.Mode = value;
    }

    /// <inheritdoc/>
    public override bool NotifyOnSourceUpdated
    {
        get => ActualBinding.NotifyOnSourceUpdated;
        set => ActualBinding.NotifyOnSourceUpdated = value;
    }

    /// <inheritdoc/>
    public override bool NotifyOnTargetUpdated
    {
        get => ActualBinding.NotifyOnTargetUpdated;
        set => ActualBinding.NotifyOnTargetUpdated = value;
    }

    /// <inheritdoc/>
    public override bool NotifyOnValidationError
    {
        get => ActualBinding.NotifyOnValidationError;
        set => ActualBinding.NotifyOnValidationError = value;
    }

    /// <inheritdoc/>
    public override UpdateSourceExceptionFilterCallback? UpdateSourceExceptionFilter
    {
        get => ActualBinding.UpdateSourceExceptionFilter;
        set => ActualBinding.UpdateSourceExceptionFilter = value;
    }

    /// <inheritdoc/>
    public override UpdateSourceTrigger UpdateSourceTrigger
    {
        get => ActualBinding.UpdateSourceTrigger;
        set => ActualBinding.UpdateSourceTrigger = value;
    }

    /// <inheritdoc/>
    public override bool ValidatesOnDataErrors
    {
        get => ActualBinding.ValidatesOnDataErrors;
        set => ActualBinding.ValidatesOnDataErrors = value;
    }

    /// <inheritdoc/>
    public override bool ValidatesOnExceptions
    {
        get => ActualBinding.ValidatesOnExceptions;
        set => ActualBinding.ValidatesOnExceptions = value;
    }

    /// <inheritdoc/>
    public override bool ValidatesOnNotifyDataErrors
    {
        get => ActualBinding.ValidatesOnNotifyDataErrors;
        set => ActualBinding.ValidatesOnNotifyDataErrors = value;
    }

    /// <inheritdoc/>
    public override Collection<ValidationRule> ValidationRules
        => ActualBinding.ValidationRules;

    /// <summary>
    /// Gets or sets the opaque data passed to the asynchronous data dispatcher.
    /// </summary>
    [DefaultValue(null)]
    public object? AsyncState
    {
        get => ActualBinding.AsyncState;
        set => ActualBinding.AsyncState = value;
    }

    /// <summary>
    /// Gets or sets a value that indicates whether to evaluate the <see cref="Path"/> relative to the data item or the
    /// <see cref="DataSourceProvider"/> object.
    /// </summary>
    [DefaultValue(false)]
    public bool BindsDirectlyToSource
    {
        get => ActualBinding.BindsDirectlyToSource;
        set => ActualBinding.BindsDirectlyToSource = value;
    }

    /// <summary>
    /// Gets or sets the name of the element to use as the binding source object.
    /// </summary>
    [DefaultValue(null)]
    public string? ElementName
    {
        get => ActualBinding.ElementName;
        set => ActualBinding.ElementName = value;
    }

    /// <summary>
    /// Gets or sets a value that indicates whether the binding should get and set values asynchronously.
    /// </summary>
    public bool IsAsync
    {
        get => ActualBinding.IsAsync;
        set => ActualBinding.IsAsync = value;
    }

    /// <summary>
    /// Gets or sets the path to the binding source property.
    /// </summary>
    public PropertyPath Path
    {
        get => ActualBinding.Path;
        set => ActualBinding.Path = value;
    }

    /// <summary>
    /// Gets or sets the binding source by specifying its location relative to the position of the binding target.
    /// </summary>
    [DefaultValue(null)]
    public RelativeSource? RelativeSource
    {
        get => ActualBinding.RelativeSource;
        set => ActualBinding.RelativeSource = value;
    }

    /// <summary>
    /// Gets or sets the object to use as the binding source.
    /// </summary>
    public object? Source
    {
        get => ActualBinding.Source;
        set => ActualBinding.Source = value;
    }

    /// <summary>
    /// Gets or sets an <c>XPath</c> query that returns the value on the XML binding source to use.
    /// </summary>
    [DefaultValue(null)]
    public string? XPath
    {
        get => ActualBinding.XPath;
        set => ActualBinding.XPath = value;
    }

    /// <inheritdoc/>
    protected override Binding ActualBinding
    { get; } = new();
}