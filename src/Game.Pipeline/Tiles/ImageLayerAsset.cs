//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Xml.Linq;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Game.Tiles;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for a tile map image layer asset.
/// </summary>
public sealed class ImageLayerAsset : LayerAsset
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageLayerAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the image layer's configuration.</param>
    public ImageLayerAsset(XElement root) 
        : base(root, LayerType.Image)
    {
        XElement? imageElement = root.Element(XmlConstants.ImageElement);

        if (imageElement == null)
            throw new InvalidOperationException(Strings.ImageLayerNoImage);

        Image = new ImageAsset(imageElement);
    }

    /// <summary>
    /// Gets the data for the layer's image.
    /// </summary>
    public ImageAsset Image
    { get; }
}
