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
/// Provides a writer of raw tile map content to the content pipeline.
/// </summary>
[ContentTypeWriter]
public sealed class TileMapWriter : ContentTypeWriter<TileMapContent>
{
    /// <inheritdoc />
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
        => typeof(TileMapReader).AssemblyQualifiedName ?? string.Empty;

    /// <inheritdoc />
    protected override void Write(ContentWriter output, TileMapContent value)
    {
        Require.NotNull(output, nameof(output));
        Require.NotNull(value, nameof(value));

        TileMapAsset asset = value.Asset;

        output.Write((int) asset.Orientation);
        output.Write((int) asset.RenderOrder);
        output.Write(asset.BackgroundColor);
        output.Write(asset.Width);
        output.Write(asset.Height);
        output.Write(asset.TileWidth);
        output.Write(asset.TileHeight);
        output.WriteProperties(asset);

        WriteTileSets(output, value);
        WriteLayers(output, value);
    }

    private static void WriteTileSets(ContentWriter output, TileMapContent value)
    {
        TileMapAsset asset = value.Asset;
        
        // Need to record how many tile sets in order to properly direct the reader.
        output.Write(asset.TileSets.Count);

        foreach (TileSetAsset tileSet in asset.TileSets)
        {
            output.Write(tileSet.FirstId);
            // The next value written indicates if the tile set is externally defined.
            if (!string.IsNullOrEmpty(tileSet.Source))
            {
                output.Write(true);
                output.WriteExternalReference(value.GetReference<TileSetContent>(tileSet.Source));
            }
            else
            {
                output.Write(false);
                TileSetWriter.Write(output, tileSet, value);
            }
        }
    }

    private static void WriteLayers(ContentWriter output, TileMapContent value)
    {
        TileMapAsset asset = value.Asset;

        // Need to record how many layers in order to properly direct the reader.
        output.Write(asset.Layers.Count);

        foreach (LayerAsset layer in asset.Layers)
        {
            output.Write((int) layer.Type);
            output.Write(layer.Name);
            output.Write(layer.IsVisible);
            output.Write(layer.Opacity);
            output.Write(layer.OffsetX);
            output.Write(layer.OffsetY);
            output.WriteProperties(layer);

            switch (layer)
            {
                case ImageLayerAsset imageLayer:
                    WriteImageLayer(output, imageLayer, value);
                    break;

                case TileLayerAsset tileLayer:
                    WriteTileLayer(output, tileLayer);
                    break;
            }
        }
    }

    private static void WriteImageLayer(ContentWriter output, ImageLayerAsset imageLayer, TileMapContent value)
    {
        ExternalReference<Texture2DContent> imageReference
            = value.GetReference<Texture2DContent>(imageLayer.Image.Source);

        output.WriteExternalReference(imageReference);
    }

    private static void WriteTileLayer(ContentWriter output, TileLayerAsset tileLayer)
    {
        output.Write(tileLayer.Width);
        output.Write(tileLayer.Height);

        // Need to record how many tiles in order to properly direct the reader.
        output.Write(tileLayer.Tiles.Count);

        foreach (Tile tile in tileLayer.Tiles)
        {
            output.Write(tile.IdWithFlags);
            output.Write(tile.Column);
            output.Write(tile.Row);
        }
    }
}
