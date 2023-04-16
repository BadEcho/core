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
        var customProperties = input.ReadProperties();

        var size = new Size(width, height);
        var tileSize = new Size(tileWidth, tileHeight);
        var map = new TileMap(input.GetGraphicsDevice(),
                              input.AssetName,
                              size,
                              tileSize,
                              customProperties)
                  {
                      BackgroundColor = backgroundColor,
                      Orientation = orientation,
                      RenderOrder = renderOrder
                  };

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
            var customProperties = input.ReadProperties();

            switch (layerType)
            {
                case LayerType.Image:
                    var imageTexture = input.ReadExternalReference<Texture2D>();

                    var imageLayer = new ImageLayer(name, imageTexture, customProperties)
                                     {
                                         IsVisible = isVisible,
                                         Opacity = opacity,
                                         Offset = new Vector2(offsetX, offsetY),
                                         Position = Vector2.Zero
                                     };

                    map.AddLayer(imageLayer);
                    break;

                case LayerType.Tile:
                    var width = input.ReadInt32();
                    var height = input.ReadInt32();

                    var tilesToRead = input.ReadInt32();

                    var tileLayer = new TileLayer(name,
                                                  new Size(width, height),
                                                  map.TileSize,
                                                  customProperties)
                                    {
                                        IsVisible = isVisible,
                                        Opacity = opacity,
                                        Offset = new Vector2(offsetX, offsetY)
                                    };

                    while (tilesToRead > 0)
                    {
                        var idWithFlags = input.ReadUInt32();
                        var column = input.ReadInt32();
                        var row = input.ReadInt32();

                        tileLayer.LoadTile(idWithFlags, column, row);
                        tilesToRead--;
                    }

                    map.AddLayer(tileLayer);
                    break;
            }


            layersToRead--;
        }
    }
}
