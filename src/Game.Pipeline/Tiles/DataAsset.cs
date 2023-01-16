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

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for a blob of tiles, optionally compressed and/or encoded, that are assigned to a tile layer.
/// </summary>
public sealed class DataAsset
{
    private const string ENCODING_ATTRIBUTE = "encoding";
    private const string COMPRESSION_ATTRIBUTE = "compression";

    /// <summary>
    /// Initializes a new instance of the <see cref="DataAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the tile blob's configuration.</param>
    public DataAsset(XElement root)
    {
        Require.NotNull(root, nameof(root));

        Encoding = (string?) root.Attribute(ENCODING_ATTRIBUTE) ?? string.Empty;
        Compression = (string?) root.Attribute(COMPRESSION_ATTRIBUTE) ?? string.Empty;
        Payload = root.Value;
    }

    /// <summary>
    /// Gets the encoding used to encode the tile layer data.
    /// </summary>
    /// <remarks>
    /// Tile data, if encoding is used, is encoded as either a Base64 string ("base64") or as comma separated values ("csv").
    /// Unencoded data, indicated by this property returning an empty string, appears as a collection of <c>tile</c> XML elements,
    /// however this inefficient approach is deprecated within the TMX map format specification.
    /// </remarks>
    public string Encoding 
    { get; }

    /// <summary>
    /// Gets the type of compression used to compress the tile layer data.
    /// </summary>
    /// <remarks>TMX maps support "gzip", "zlib", and "zstd" (ZStandard) compression formats.</remarks>
    public string Compression
    { get; }

    /// <summary>
    /// Gets the blob of tiles assigned to a tile layer.
    /// </summary>
    public string Payload
    { get; }
}
