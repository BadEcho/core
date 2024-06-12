//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
/// Provides configuration data for a tile set asset.
/// </summary>
public sealed class TileSetAsset : ExtensibleAsset
{
    private const string COLUMNS_ATTRIBUTE = "columns";
    private const string SPACING_ATTRIBUTE = "spacing";
    private const string MARGIN_ATTRIBUTE = "margin";
    private const string FIRST_ID_ATTRIBUTE = "firstgid";
    private const string TILE_COUNT_ATTRIBUTE = "tilecount";
    private const string TILE_ELEMENT = "tile";

    private readonly List<TileAsset> _tiles = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="TileSetAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the tile set's configuration.</param>
    public TileSetAsset(XElement root)
        : base(root)
    {
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

        foreach (XElement tile in root.Elements(TILE_ELEMENT))
        {
            _tiles.Add(new TileAsset(tile));
        }
    }

    /// <summary>
    /// Gets the starting value to use when mapping global identifiers to the tiles in this tile set.
    /// </summary>
    public int FirstId
    { get; }

    /// <summary>
    /// Gets or sets the path to a TSX file if this tile set's data is stored externally; otherwise, if the data is defined
    /// inline, an empty string.
    /// </summary>
    public string Source
    { get; set; }

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
    /// <remarks>
    /// This will be present if the tile set is based on a single image. If the tile set is based on a collection
    /// of images, then the tile set's constituent tile assets will contain the image data.
    /// </remarks>
    public ImageAsset? Image
    { get; }

    /// <summary>
    /// Gets the collection of explicitly configured tiles belonging to this tile set.
    /// </summary>
    /// <remarks>
    /// Tile sets based on a single image do not require explicit configuration for their tiles, save for any
    /// that have animation frames.
    /// </remarks>
    public IReadOnlyCollection<TileAsset> Tiles
        => _tiles;
}
