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
using BadEcho.Game.Tiles;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for a tile map layer asset.
/// </summary>
public abstract class LayerAsset 
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LayerAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the layer's configuration.</param>
    /// <param name="type">An enumeration value that specifies the type of layer and the kind of content it contains.</param>
    protected LayerAsset(XElement root, LayerType type)
    {
        Require.NotNull(root, nameof(root));

        Name = (string?) root.Attribute(XmlConstants.NameAttribute) ?? string.Empty;
        IsVisible = (bool?) root.Attribute(XmlConstants.VisibleAttribute) ?? true;
        Opacity = (float?) root.Attribute(XmlConstants.OpacityAttribute) ?? 1.0f;
        OffsetX = (float?) root.Attribute(XmlConstants.OffsetXAttribute) ?? default;
        OffsetY = (float?) root.Attribute(XmlConstants.OffsetYAttribute) ?? default;
        Type = type;
    }

    /// <summary>
    /// Gets the name of this layer.
    /// </summary>
    public string Name
    { get; }

    /// <summary>
    /// Gets a value indicating if this layer's data should actually be rendered.
    /// </summary>
    public bool IsVisible
    { get; }

    /// <summary>
    /// Gets the opacity of this layer and all of its contents.
    /// </summary>
    public float Opacity
    { get; }

    /// <summary>
    /// Gets the horizontal offset, in terms of this layer's position, from the tile map's origin.
    /// </summary>
    public float OffsetX
    { get; }

    /// <summary>
    /// Gets the vertical offset, in terms of this layer's position, from the tile map's origin.
    /// </summary>
    public float OffsetY
    { get; }

    /// <summary>
    /// Gets a <see cref="LayerType"/> value that specifies this layer's type and the kind of content it contains.
    /// </summary>
    public LayerType Type
    { get; }
}
