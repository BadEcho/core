//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Xml.Linq;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for a custom property attributed to a particular tile-related asset.
/// </summary>
public sealed class CustomPropertyAsset
{
    private const string VALUE_ATTRIBUTE = "value";

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomPropertyAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the custom property's configuration.</param>
    public CustomPropertyAsset(XElement root)
    {
        Require.NotNull(root, nameof(root));

        Name = (string?) root.Attribute(XmlConstants.NameAttribute) ?? string.Empty;
        Value = (string?) root.Attribute(VALUE_ATTRIBUTE) ?? string.Empty;
    }

    /// <summary>
    /// Gets the name of the custom property.
    /// </summary>
    public string Name
    { get; }

    /// <summary>
    /// Gets the value of the custom property.
    /// </summary>
    /// <remarks>
    /// While the TMX map format supports expression of the property's value type, the value itself is inevitably saved as string
    /// an asset  file, and is therefore read as one.
    /// </remarks>
    public string Value
    { get; }
}
