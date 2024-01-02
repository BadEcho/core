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
using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides an importer of tile map asset data for the content pipeline.
/// </summary>
[ContentImporter(".tmx", DisplayName = "Tile Map Importer - Bad Echo", DefaultProcessor = nameof(TileMapProcessor))]
public sealed class TileMapImporter : ContentImporter<TileMapContent>
{
    /// <inheritdoc />
    public override TileMapContent Import(string filename, ContentImporterContext context)
    {
        Require.NotNull(filename, nameof(filename));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ImportingTileMap.InvariantFormat(filename));

        XElement assetRoot = XElement.Load(filename);
        var asset = new TileMapAsset(assetRoot);
        string mapDirectory = Path.GetDirectoryName(filename) ?? string.Empty;

        ImportTileSets(asset, mapDirectory, context);
        ImportLayers(asset, mapDirectory, context);

        context.Log(Strings.ImportingFinished.InvariantFormat(filename));

        return new TileMapContent(asset) { Identity = new ContentIdentity(filename) };
    }

    private static void ImportTileSets(TileMapAsset asset, string mapDirectory, ContentImporterContext context)
    {
        foreach (TileSetAsset tileSet in asset.TileSets)
        {
            if (!string.IsNullOrEmpty(tileSet.Source))
            {   // This is a referenced tile set that is defined in its own file.
                context.Log(Strings.ImportingDependency.InvariantFormat(tileSet.Source));

                tileSet.Source = Path.Combine(mapDirectory, tileSet.Source);

                context.AddDependency(tileSet.Source);
            }
            else if (tileSet.Image != null)
            {   // The tile set is embedded inside the map.
                context.Log(Strings.ImportingDependency.InvariantFormat(tileSet.Image.Source));

                tileSet.Image.Source = Path.Combine(mapDirectory, tileSet.Image.Source);

                context.AddDependency(tileSet.Image.Source);
            }
        }
    }

    private static void ImportLayers(TileMapAsset asset, string mapDirectory, ContentImporterContext context)
    {
        foreach (LayerAsset layer in asset.Layers)
        {
            switch (layer)
            {
                case ImageLayerAsset imageLayer:
                    context.Log(Strings.ImportingDependency.InvariantFormat(imageLayer.Image.Source));

                    imageLayer.Image.Source = Path.Combine(mapDirectory, imageLayer.Image.Source);

                    context.AddDependency(imageLayer.Image.Source);
                    break;
            }
        }
    }
}
