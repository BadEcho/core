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

using BadEcho.Game.Tiles;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides a writer of raw tile set content to the content pipeline.
/// </summary>
[ContentTypeWriter]
public sealed class TileSetWriter : ContentTypeWriter<TileSetContent>
{
    /// <inheritdoc />
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
        => typeof(TileSetReader).AssemblyQualifiedName ?? string.Empty;

    /// <summary>
    /// Writes the provided tile set asset to the content pipeline, using the provided content item as a reference source.
    /// </summary>
    /// <param name="output">The writer to the content pipeline.</param>
    /// <param name="asset">The tile set asset to write to the content pipeline.</param>
    /// <param name="referenceSource">A content item to use as a reference source.</param>
    public static void Write(ContentWriter output, TileSetAsset asset, IContentItem referenceSource)
    {
        Require.NotNull(output, nameof(output));
        Require.NotNull(asset, nameof(asset));
        Require.NotNull(referenceSource, nameof(referenceSource));

        if (asset.Image == null)
            throw new PipelineException();

        ExternalReference<Texture2DContent> imageReference
            = referenceSource.GetReference<Texture2DContent>(asset.Image.Source);

        output.WriteExternalReference(imageReference);

        output.Write(asset.TileWidth);
        output.Write(asset.TileHeight);
        output.Write(asset.TileCount);
        output.Write(asset.Columns);
        output.Write(asset.Spacing);
        output.Write(asset.Margin);
        output.WriteProperties(asset);

        WriteTiles(output, asset);
    }

    /// <inheritdoc />
    protected override void Write(ContentWriter output, TileSetContent value)
    {
        Require.NotNull(value, nameof(value));

        Write(output, value.Asset, value);
    }
    
    private static void WriteTiles(ContentWriter output, TileSetAsset asset)
    {
        // Record the number of tiles to guide the reader.
        output.Write(asset.Tiles.Count);

        foreach (TileAsset tile in asset.Tiles)
        {
            output.Write(tile.Id);
            output.WriteObject(tile.SourceArea);

            // The animation frame count, to guide the reader.
            output.Write(tile.AnimationFrames.Count);

            foreach (TileAnimationFrame frame in tile.AnimationFrames)
            {
                output.Write(frame.TileId);
                output.WriteObject(frame.Duration);
            }

            output.WriteProperties(tile);
        }
    }
}
