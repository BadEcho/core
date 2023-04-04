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

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using BadEcho.Presentation.Properties;

namespace BadEcho.Presentation.Markup;

/// <summary>
/// Provides a type converter for Bad Echo Presentation framework resource keys.
/// </summary>
/// <remarks>
/// <para>
/// A custom <see cref="TypeConverter"/> is required for the proper operation of tools that extract control parts containing
/// resource keys and converting them into resources. A number of compromises had to be made in order for us to be able to play
/// ball in this normally closed off process.
/// </para>
/// <para>
/// In order to be able to convert from resource keys to markup and vice versa, the markup this converter outputs differs from
/// the usual format. Resource keys should normally be referenced using static markup extensions, however efforts made to produce
/// static extension conversion outputs for Bad Echo Presentation framework key instances for the tools requiring said conversion
/// have not been successful. In order to achieve success, strings containing type information is output in the following format:
/// <c>FullTypeName!KeyName</c>.
/// </para>
/// <para>
/// This special format only comes into play in instances where XAML containing Bad Echo Presentation framework keys is exported from
/// an external assembly with a tool such as Blend. For all other uses, like during the normal runtime of an application, a normal
/// static markup extension will be used instead and this converter will never be called.
/// </para>
/// </remarks>
internal sealed class BadEchoKeyConverter : TypeConverter
{
    private const char KEY_DELIMITER = '!';

    /// <inheritdoc/>
    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (CanConvertTo(context, destinationType))
        {
            if (value is not BadEchoKey key)
                throw new ArgumentException(Strings.CanOnlyConvertBadEchoKeys, nameof(value));

            return $"{key.ProviderType.FullName}{KEY_DELIMITER}{key.Name}";
        }

        // This may look like a mistake (returning the output of CanConvertTo in the conversion function), but it is not.
        // It is required by internal Microsoft processes, and is exactly how the ComponentResourceKeyConverter does it.
        return CanConvertTo(context, destinationType);
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        var stringValue = (string) value;

        string[] descriptorParts = stringValue.Split(KEY_DELIMITER);

        if (descriptorParts.Length != 2)
            throw new ArgumentException(Strings.BadEchoKeyIsInvalid, nameof(value));

        string providerTypeName = descriptorParts[0];
        string keyName = descriptorParts[1];

        Type? providerType = Type.GetType(providerTypeName);

        if (null == providerType)
            throw new ArgumentException(Strings.BadEchoKeyCannotFindType, nameof(value));

        FieldInfo? providerField = providerType.GetField(keyName);

        if (null == providerField)
            throw new ArgumentException(Strings.BadEchoKeyCannotFindField, nameof(value));

        return providerField.GetValue(null);
    }
}