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
/// Provides configuration data for a tile set asset.
/// </summary>
public sealed class TileSetAsset
{
    private const string COLUMNS_ATTRIBUTE = "columns";
    private const string SPACING_ATTRIBUTE = "spacing";
    private const string MARGIN_ATTRIBUTE = "margin";
    private const string FIRST_ID_ATTRIBUTE = "firstgid";
    private const string TILE_COUNT_ATTRIBUTE = "tilecount";

    /// <summary>
    /// Initializes a new instance of the <see cref="TileSetAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the tile set's configuration.</param>
    public TileSetAsset(XElement root)
    {
        Require.NotNull(root, nameof(root));

        FirstId = (int?) root.Attribute(FIRST_ID_ATTRIBUTE) ?? default;
        Source = (string?) root.Attribute(XmlConstants.SourceAttribute) ?? string.Empty;
        Name = (string?) root.Attribute(XmlConstants.NameAttribute) ?? string.Empty;
        TileWidth = (int?) root.Attribute(XmlConstants.TileWidthAttribute) ?? default;
        TileHeight = (int?) root.Attribute(XmlConstants.TileHeightAttribute) ?? default;
        TileCount = (int?) root.Attribute(TILE_COUNT_ATTRIBUTE) ?? default;
        Columns = (int?) root.Attribute(COLUMNS_ATTRIBUTE) ?? default;
        Spacing = (int?) root.Attribute(SPACING_ATTRIBUTE) ?? default;
        Margin = (int?) root.Attribute(MARGIN_ATTRIBUTE) ?? default;

        XElement? imageElement = root.Element(XmlConstants.ImageElement);

        if (imageElement != null)
            Image = new ImageAsset(imageElement);
    }

    /// <summary>
    /// Gets the starting value to use when mapping global identifiers to the tiles in this tile set.
    /// </summary>
    public int FirstId
    { get; }

    /// <summary>
    /// Gets the path to a TSX file if this tile set's data is stored externally; otherwise, if the data is defined inline,
    /// an empty string.
    /// </summary>
    public string Source
    { get; }

    /// <summary>
    /// Gets the name of this tile set.
    /// </summary>
    public string Name
    { get; }

    /// <summary>
    /// Gets the width of an individual tile in the tile set.
    /// </summary>
    public int TileWidth
    { get; }

    /// <summary>
    /// Gets the height of an individual tile in this tile set.
    /// </summary>
    public int TileHeight
    { get; }

    /// <summary>
    /// Gets the number of tiles in this tile set.
    /// </summary>
    public int TileCount
    { get; }

    /// <summary>
    /// Gets the number of columns of tiles in this tile set.
    /// </summary>
    public int Columns
    { get; }

    /// <summary>
    /// Gets the spacing between the individual tiles that compose this tile set.
    /// </summary>
    public int Spacing
    { get; }

    /// <summary>
    /// Gets the space between the perimeter of the tiles composing this tile set and the edge of the texture.
    /// </summary>
    public int Margin
    { get; }

    /// <summary>
    /// Gets the image data for the texture comprising all the tiles in this tile set.
    /// </summary>
    public ImageAsset? Image
    { get; }
}
