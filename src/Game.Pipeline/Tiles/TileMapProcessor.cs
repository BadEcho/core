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

using System.IO.Compression;
using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Game.Tiles;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides a processor of tile map asset data for the content pipeline.
/// </summary>
[ContentProcessor(DisplayName = "Tile Map Processor - Bad Echo")]
public sealed class TileMapProcessor : ContentProcessor<TileMapContent, TileMapContent>
{
    private const string COMPRESSION_GZIP = "gzip";
    private const string COMPRESSION_ZLIB = "zlib";
    private const string ENCODING_BASE64 = "base64";
    private const string ENCODING_CSV = "csv";

    /// <inheritdoc />
    public override TileMapContent Process(TileMapContent input, ContentProcessorContext context)
    {
        Require.NotNull(input, nameof(input));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ProcessingTileMap.InvariantFormat(input.Identity.SourceFilename));

        ProcessTileSets(input, context);
        ProcessLayers(input, context);

        context.Log(Strings.ProcessingFinished.InvariantFormat(input.Identity.SourceFilename));

        return input;
    }

    private static void ProcessTileSets(TileMapContent input, ContentProcessorContext context)
    {
        TileMapAsset asset = input.Asset;

        foreach (TileSetAsset tileSet in asset.TileSets)
        {
            if (!string.IsNullOrEmpty(tileSet.Source))
            {   // Leverage our tile set content loader to load this external tile set.
                input.AddReference<TileSetContent>(context, tileSet.Source, []);
            }
            else if (tileSet.Image != null)
            {   // If the tile set is embedded in the map, then we have to perform the content loading work here.
                var processorParameters = new OpaqueDataDictionary
                                          {
                                              { nameof(TextureProcessor.ColorKeyColor), tileSet.Image.ColorKey },
                                              { nameof(TextureProcessor.ColorKeyEnabled), true }
                                          };

                input.AddReference<Texture2DContent>(context, tileSet.Image.Source, processorParameters);
            }
        }
    }

    private static void ProcessLayers(TileMapContent input, ContentProcessorContext context)
    {
        TileMapAsset asset = input.Asset;

        foreach (LayerAsset layer in asset.Layers)
        {
            switch (layer)
            {
                case ImageLayerAsset imageLayer:
                    var processorParameters = new OpaqueDataDictionary
                                              {
                                                  { nameof(TextureProcessor.ColorKeyColor), imageLayer.Image.ColorKey },
                                                  { nameof(TextureProcessor.ColorKeyEnabled), true }
                                              };

                    input.AddReference<Texture2DContent>(context, imageLayer.Image.Source, processorParameters);
                    break;

                case TileLayerAsset tileLayer:
                    IList<uint> tileData = DecodeTileData(tileLayer.Data, tileLayer.Width * tileLayer.Height);

                    foreach (Tile tile in CreateTiles(asset.RenderOrder, tileLayer.Width, tileLayer.Height, tileData))
                    {
                        tileLayer.Tiles.Add(tile);
                    }

                    break;

                default:
                    throw new NotSupportedException(Strings.TileMapUnsupportedLayerType.InvariantFormat(layer.Type));
            }
        }
    }

    private static IEnumerable<Tile> CreateTiles(TileRenderOrder renderOrder, int mapWidth, int mapHeight, IList<uint> tileData)
        => renderOrder switch
        {
            TileRenderOrder.RightDown => CreateTilesRightDown(mapWidth, mapHeight, tileData),
            TileRenderOrder.RightUp => CreateTilesRightUp(mapWidth, mapHeight, tileData),
            TileRenderOrder.LeftDown => CreateTilesLeftDown(mapWidth, mapHeight, tileData),
            _ => CreateTilesLeftUp(mapWidth, mapHeight, tileData)
        };

    private static IEnumerable<Tile> CreateTilesRightDown(int mapWidth, int mapHeight, IList<uint> tileData)
    {
        for (int row = 0; row < mapHeight; row++)
        {
            for (int column = 0; column < mapWidth; column++)
            {
                yield return CreateTile(column, row, mapWidth, tileData);
            }
        }
    }

    private static IEnumerable<Tile> CreateTilesRightUp(int mapWidth, int mapHeight, IList<uint> tileData)
    {
        for (int row = mapHeight - 1; row >= 0; row--)
        {
            for (int column = 0; column < mapWidth; column++)
            {
                yield return CreateTile(column, row, mapWidth, tileData);
            }
        }
    }

    private static IEnumerable<Tile> CreateTilesLeftDown(int mapWidth, int mapHeight, IList<uint> tileData)
    {
        for (int row = 0; row < mapHeight; row++)
        {
            for (int column = mapWidth - 1; column >= 0; column--)
            {
                yield return CreateTile(column, row, mapWidth, tileData);
            }
        }
    }

    private static IEnumerable<Tile> CreateTilesLeftUp(int mapWidth, int mapHeight, IList<uint> tileData)
    {
        for (int row = mapHeight - 1; row >= mapHeight; row--)
        {
            for (int column = mapWidth - 1; column >= 0; column--)
            {
                yield return CreateTile(column, row, mapWidth, tileData);
            }
        }
    }

    private static Tile CreateTile(int column, int row, int mapWidth, IList<uint> tileData)
    {
        int tileIndex = column + row * mapWidth;
        uint tileId = tileData[tileIndex];

        return tileId == 0 ? default : new Tile(tileId, column, row);
    }

    private static List<uint> DecodeTileData(DataAsset tileData, int tilesToDecode)
        => tileData.Encoding switch
        {
            ENCODING_BASE64 => DecodeBase64TileData(tileData, tilesToDecode),
            ENCODING_CSV => DecodeCsvTileData(tileData),
            _ => throw new NotSupportedException(Strings.TileLayerEncodingUnsupported.InvariantFormat(tileData.Encoding))
        };

    private static List<uint> DecodeCsvTileData(DataAsset tileData)
        => tileData.Payload
                   .Split(',')
                   .Select(uint.Parse)
                   .ToList();

    private static List<uint> DecodeBase64TileData(DataAsset tileData, int tilesToDecode)
    {
        var tiles = new List<uint>();
        byte[] decodedData = Convert.FromBase64String(tileData.Payload.Trim());

        using (var stream = GetDataStream(decodedData, tileData.Compression))
        {
            using (var reader = new BinaryReader(stream))
            {
                while (tilesToDecode > 0)
                {
                    tiles.Add(reader.ReadUInt32());
                    tilesToDecode--;
                }
            }
        }

        return tiles;
    }

    private static Stream GetDataStream(byte[] decodedData, string compression)
    {   // Will be disposed by an outer decompression stream, if one wraps it.
        var memoryStream = new MemoryStream(decodedData, false);

        return compression switch
        {
            COMPRESSION_GZIP => new GZipStream(memoryStream, CompressionMode.Decompress),
            COMPRESSION_ZLIB => new ZLibStream(memoryStream, CompressionMode.Decompress),
            _ => memoryStream
        };
    }
}
