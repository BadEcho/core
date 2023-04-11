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

using System.Xml.Linq;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Extensions;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for tile-related assets that support custom property attribution.
/// </summary>
public abstract class ExtensibleAsset
{
    private const string PROPERTIES_ELEMENT = "properties";
    private const string PROPERTY_ELEMENT = "property";
    private const string VALUE_ATTRIBUTE = "value";
    private const string TYPE_ATTRIBUTE = "type";

    private readonly Dictionary<string, string> _customStringProperties = new();
    private readonly Dictionary<string, bool> _customBoolProperties = new();
    private readonly Dictionary<string, Color> _customColorProperties = new();
    private readonly Dictionary<string, int> _customIntProperties = new();
    private readonly Dictionary<string, float> _customFloatProperties = new();
    private readonly Dictionary<string, string> _customFileProperties = new();
 
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensibleAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the extensible asset's configuration.</param>
    protected ExtensibleAsset(XElement root)
    {
        Require.NotNull(root, nameof(root));

        XElement? propertiesElement = root.Element(PROPERTIES_ELEMENT);

        if (propertiesElement != null)
        {
            foreach (XElement propertyElement in propertiesElement.Elements(PROPERTY_ELEMENT))
            {
                string name = (string?) propertyElement.Attribute(XmlConstants.NameAttribute) ?? string.Empty;
                string value = (string?) propertyElement.Attribute(VALUE_ATTRIBUTE) ?? string.Empty;
                string? typeValue = (string?) propertyElement.Attribute(TYPE_ATTRIBUTE);
                CustomPropertyType type;

                if (string.IsNullOrEmpty(typeValue))    // String custom properties have no 'type' attribute.
                    type = CustomPropertyType.String;

                else if (!Enum.TryParse(typeValue, true, out type))
                    throw new NotSupportedException(Strings.ExtensibleUnsupportedType.InvariantFormat(typeValue));

                AddCustomProperty(name, value, type);
            }
        }
    }

    /// <summary>
    /// Gets a mapping between the names of the asset's custom string properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, string> CustomStringProperties
        => _customStringProperties;

    /// <summary>
    /// Gets a mapping between the names of the asset's custom bool properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, bool> CustomBoolProperties
        => _customBoolProperties;

    /// <summary>
    /// Gets a mapping between the names of the asset's custom color properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, Color> CustomColorProperties
        => _customColorProperties;

    /// <summary>
    /// Gets a mapping between the names of the asset's custom integer properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, int> CustomIntProperties
        => _customIntProperties;

    /// <summary>
    /// Gets a mapping between the names of the asset's custom floating-point number properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, float> CustomFloatProperties
        => _customFloatProperties;

    /// <summary>
    /// Gets a mapping between the names of the asset's custom file properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, string> CustomFileProperties
        => _customFileProperties;

    private static bool TryParseFileInfo(string? value, out string? result)
    {   // We can't write a FileInfo instance to the content pipeline, so we just perform validation here to make sure the string path
        // has no chance of causing an exception to be thrown when the reader uses it to initialize the FileInfo.
        result = default;

        // MSDN documentation as of .NET 7.0 on FileInfo exceptions is outdated. Only the following conditions will cause an exception:
        // 1) If the path is null or empty.
        if (string.IsNullOrEmpty(value))
            return false;

        // 2) If the path contains a null character. Surprisingly, and contrary to documentation, characters returned by
        // Path.GetInvalidPathChars() do not cause an exception; only a null character will.
        if (value.Contains('\0', StringComparison.OrdinalIgnoreCase))
            return false;
        
        // That appears to be it! Nothing else results in an exception.
        result = value;

        return true;
    }

    private static T ParsePropertyValue<T>(string name, string value, CustomPropertyType type, PropertyParser<T> parser)
    {
        if (!parser(value, out T? parsedValue))
        {
            throw new FormatException(
                Strings.ExtensibleUnparseablePropertyValue.InvariantFormat(name, type, value));
        }

        if (parsedValue == null)
        {
            throw new InvalidOperationException(
                Strings.ExtensibleParsedNullPropertyValue.InvariantFormat(type, name, value));
        }

        return parsedValue;
    }

    private void AddCustomProperty(string name, string value, CustomPropertyType type)
    {
        switch (type)
        {
            case CustomPropertyType.Bool:
                bool boolValue = ParsePropertyValue<bool>(name, value, type, bool.TryParse);
                
                _customBoolProperties.Add(name, boolValue);
                break;

            case CustomPropertyType.Color:
                Color colorValue = ParsePropertyValue<Color>(name, value, type, Coloring.TryParse);

                _customColorProperties.Add(name, colorValue);
                break;

            case CustomPropertyType.File:
                string fileValue = ParsePropertyValue<string>(name, value, type, TryParseFileInfo);

                _customFileProperties.Add(name, fileValue);
                break;

            case CustomPropertyType.Float:
                float floatValue = ParsePropertyValue<float>(name, value, type, float.TryParse);

                _customFloatProperties.Add(name, floatValue);
                break;

            case CustomPropertyType.Int:
                int intValue = ParsePropertyValue<int>(name, value, type, int.TryParse);

                _customIntProperties.Add(name, intValue);
                break;

            case CustomPropertyType.String:
                _customStringProperties.Add(name, value);
                break;
        }
    }

    /// <summary>
    /// A callback to use for parsing custom property values.
    /// </summary>
    /// <typeparam name="T">The type of value returned by the parser.</typeparam>
    /// <param name="value">The string representation of the value to be parsed.</param>
    /// <param name="parsedValue">
    /// When this callback returns, a <typeparamref name="T"/> representation of <c>value</c>, or a default value
    /// if the conversion failed.
    /// </param>
    /// <returns>True if <c>value</c> was converted successfully; otherwise, false.</returns>
    private delegate bool PropertyParser<T>(string? value, out T? parsedValue);
}
