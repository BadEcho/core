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

using BadEcho.Drawing;
using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides a processor of tile set asset data for the content pipeline.
/// </summary>
[ContentProcessor(DisplayName = "Tile Set Processor - Bad Echo")]
public sealed class TileSetProcessor : ContentProcessor<TileSetContent, TileSetContent>
{
    /// <inheritdoc />
    public override TileSetContent Process(TileSetContent input, ContentProcessorContext context)
    {
        Require.NotNull(input, nameof(input));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ProcessingTileSet.InvariantFormat(input.Identity.SourceFilename));

        TileSetAsset asset = input.Asset;
        bool imageGenerated = false;

        if (asset.Image == null)
        {
            imageGenerated = true;
            asset.Image = GeneratePackedTexture(input, context);
        }

        var processorParameters = new OpaqueDataDictionary
                                  {
                                      { nameof(TextureProcessor.ColorKeyColor), asset.Image.ColorKey },
                                      { nameof(TextureProcessor.ColorKeyEnabled), true }
                                  };

        input.AddReference<Texture2DContent>(
            context,
            asset.Image.Source,
            processorParameters,
            imageGenerated ? context.ResolveOutputPath(asset.Image.Source) : string.Empty);

        ProcessTiles(input);

        context.Log(Strings.ProcessingFinished.InvariantFormat(input.Identity.SourceFilename));

        return input;
    }

    private static void ProcessTiles(TileSetContent input)
    {
        TileSetAsset asset = input.Asset;

        foreach (TileAsset tile in asset.Tiles)
        {
            if (tile.Image != null) 
                tile.SourceArea = input.PackedSourceAreas[tile.Image.Source];
        }
    }

    private static ImageAsset GeneratePackedTexture(TileSetContent input, ContentProcessorContext context)
    {
        TileSetAsset asset = input.Asset;
        IEnumerable<string> tileImagePaths = asset.Tiles.Select(t => t.Image?.Source).WhereNotNull();
        
        string intermediatePath = context.ResolveIntermediatePath(input.Identity.SourceFilename);
        
        if (!Directory.Exists(intermediatePath))
            Directory.CreateDirectory(intermediatePath);

        string packedTexturePath = Path.Combine(intermediatePath, $"{asset.Name}-packed.png");

        var imagePacker = new ImagePacker();
        
        imagePacker.Pack(tileImagePaths, 4096, 4096);
        imagePacker.Save(packedTexturePath);
        
        foreach (var outputPosition in imagePacker.PackedAreas)
        {
            input.PackedSourceAreas.Add(outputPosition.Key,
                                        new Rectangle(outputPosition.Value.X,
                                                      outputPosition.Value.Y,
                                                      outputPosition.Value.Width,
                                                      outputPosition.Value.Height));
        }

        return new ImageAsset(packedTexturePath,
                              Color.Transparent,
                              imagePacker.OutputSize.Width,
                              imagePacker.OutputSize.Height);
    }
}
