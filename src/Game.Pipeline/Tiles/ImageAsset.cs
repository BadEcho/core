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
/// Provides configuration data for a tile-related image asset.
/// </summary>
public sealed class ImageAsset
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageAsset"/> class.
    /// </summary>
    public ImageAsset(XElement root)
    {
        Require.NotNull(root, nameof(root));

        Source = (string?) root.Attribute(XmlConstants.SourceAttribute) ?? string.Empty;
        Width = (int?) root.Attribute(XmlConstants.WidthAttribute) ?? default;
        Height = (int?) root.Attribute(XmlConstants.HeightAttribute) ?? default;
    }

    /// <summary>
    /// Gets the path to an image file if the data is stored externally; otherwise, if the image is embedded, an empty string.
    /// </summary>
    /// <remarks>
    /// While the TMX map format supports images with embedded image data, the most important editors of TMX files (at least the ones
    /// to be used making Bad Echo game products), do not support this. Until such a feature becomes more widely available, tile-related
    /// image assets will only be considered valid if they have valid source paths set.
    /// </remarks>
    public string Source
    { get; }

    /// <summary>
    /// Gets the width of the image.
    /// </summary>
    public int Width
    { get; }

    /// <summary>
    /// Gets the height of the image.
    /// </summary>
    public int Height
    { get; }
}
