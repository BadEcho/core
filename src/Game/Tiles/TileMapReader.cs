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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a reader of raw tile map content from the content pipeline.
/// </summary>
public sealed class TileMapReader : ContentTypeReader<TileMap>
{
    /// <inheritdoc />
    protected override TileMap Read(ContentReader input, TileMap existingInstance)
    {
        Require.NotNull(input, nameof(input));

        var orientation = (MapOrientation) input.ReadInt32();
        var renderOrder = (TileRenderOrder) input.ReadInt32();
        var backgroundColor = input.ReadColor();
        var width = input.ReadInt32();
        var height = input.ReadInt32();
        var tileWidth = input.ReadInt32();
        var tileHeight = input.ReadInt32();

        var size = new Point(width, height);
        var tileSize = new Point(tileWidth, tileHeight);
        var map = new TileMap(input.GetGraphicsDevice(),
                              input.AssetName,
                              size,
                              tileSize,
                              orientation,
                              renderOrder,
                              backgroundColor);

        ReadTileSets(input, map);
        ReadLayers(input, map);

        return map;
    }

    private static void ReadTileSets(ContentReader input, TileMap map)
    {
        var tileSetsToRead = input.ReadInt32();

        while (tileSetsToRead > 0)
        {
            var firstId = input.ReadInt32();
            var isExternal = input.ReadBoolean();
            var tileSet = isExternal
                ? input.ReadExternalReference<TileSet>()
                : TileSetReader.Read(input);

            map.AddTileSet(tileSet, firstId);

            tileSetsToRead--;
        }
    }

    private static void ReadLayers(ContentReader input, TileMap map)
    {
        var layersToRead = input.ReadInt32();

        while (layersToRead > 0)
        {
            var layerType = (LayerType) input.ReadInt32();
            var name = input.ReadString();
            var isVisible = input.ReadBoolean();
            var opacity = input.ReadSingle();
            var offsetX = input.ReadSingle();
            var offsetY = input.ReadSingle();
            var customProperties = new Dictionary<string, string>();
            var customPropertiesToRead = input.ReadInt32();

            while (customPropertiesToRead > 0)
            {
                var propertyName = input.ReadString();
                var propertyValue = input.ReadString();

                customProperties.Add(propertyName, propertyValue);
                customPropertiesToRead--;
            }

            switch (layerType)
            {
                case LayerType.Image:
                    var imageTexture = input.ReadExternalReference<Texture2D>();

                    var imageLayer = new ImageLayer(name,
                                                    isVisible,
                                                    opacity,
                                                    new Vector2(offsetX, offsetY),
                                                    imageTexture,
                                                    Vector2.Zero,
                                                    customProperties);
                    map.AddLayer(imageLayer);
                    break;

                case LayerType.Tile:
                    var width = input.ReadInt32();
                    var height = input.ReadInt32();

                    var tilesToRead = input.ReadInt32();

                    var tileLayer = new TileLayer(name,
                                                  isVisible,
                                                  opacity,
                                                  new Vector2(offsetX, offsetY),
                                                  new Point(width, height),
                                                  customProperties);
                    while (tilesToRead > 0)
                    {
                        var idWithFlags = input.ReadUInt32();
                        var columnIndex = input.ReadInt32();
                        var rowIndex = input.ReadInt32();

                        tileLayer.LoadTile(idWithFlags, columnIndex, rowIndex);
                        tilesToRead--;
                    }

                    map.AddLayer(tileLayer);
                    break;
            }


            layersToRead--;
        }
    }
}
