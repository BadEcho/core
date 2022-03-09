//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using BadEcho.Presentation.Properties;

namespace BadEcho.Presentation.Markup;

/// <summary>
/// Provides a base class for a markup extension that defines a collection of binding objects attached to a single binding
/// target property.
/// </summary>
/// <remarks>
/// Deriving from this class allows one to extend the mechanics of an applied <see cref="MultiBinding"/>.
/// </remarks>
[ContentProperty(nameof(Bindings))]
public abstract class MultiBindingExtension : BindingExtensionSkeleton<MultiBinding, IMultiValueConverter>, IAddChild
{
    void IAddChild.AddChild(object value)
    {
        if (value is not BindingBase binding)
            throw new ArgumentException(Strings.CannotAddNonBindingToMultiBinding, nameof(value));

        Bindings.Add(binding);
    }

    void IAddChild.AddText(string text)
    {
        IAddChild actualAddChild = ActualBinding;

        actualAddChild.AddText(text);
    }

    /// <inheritdoc/>
    public override IMultiValueConverter? Converter
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
    /// Gets the collection of <see cref="Binding"/> objects within this <see cref="MultiBinding"/> instance.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Collection<BindingBase> Bindings
        => ActualBinding.Bindings;

    /// <inheritdoc/>
    protected override MultiBinding ActualBinding
    { get; } = new();
}