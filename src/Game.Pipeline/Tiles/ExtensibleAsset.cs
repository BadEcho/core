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

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for tile-related assets that support custom property attribution.
/// </summary>
public abstract class ExtensibleAsset
{
    private const string PROPERTIES_ELEMENT = "properties";
    private const string PROPERTY_ELEMENT = "property";
    private const string VALUE_ATTRIBUTE = "value";

    private readonly Dictionary<string, string> _customProperties = new();

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

                _customProperties.Add(name, value);
            }
        }
    }

    /// <summary>
    /// Gets a mapping between the names of the asset's custom properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, string> CustomProperties
        => _customProperties;
}
