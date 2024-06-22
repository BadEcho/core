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

using System.Reflection;
using BadEcho.Drawing;
using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Interop;
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
    /// <summary>
    /// Initializes the <see cref="TileSetProcessor"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This code, as with all other code in this assembly, is loaded and executed by MonoGame's
    /// content builder (MGCB) as a pipeline extension at compile time.
    /// </para>
    /// <para>
    /// The MGCB is an executable that is installed as a .NET tool, which means it is a NuGet package
    /// that lives in the global packages folder along with all of its other brethren.
    /// When executing, the <see cref="AppContext.BaseDirectory"/> will be the MGCB package folder.
    /// </para>
    /// <para>
    /// Unfortunately, because of this, any platform-specific native libraries depended on by pipeline extensions
    /// will fail to load, as MGCB's assembly resolver will be probing its own local runtime folder.
    /// This extension uses SkiaSharp which relies on its own native library, so we help it out with a
    /// custom native library resolver. 
    /// </para>
    /// </remarks>
    static TileSetProcessor()
    {
        string extensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) 
                                    ?? Path.DirectorySeparatorChar.ToString();

        NativeLibraryTransposer.Create(typeof(SkiaSharp.SKBitmap).Assembly, extensionDirectory);
    }

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
