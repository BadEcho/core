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

using BadEcho.Game.Tiles;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides a writer of raw tile set content into the content pipeline.
/// </summary>
[ContentTypeWriter]
public sealed class TileSetWriter : ContentTypeWriter<TileSetContent>
{
    /// <inheritdoc />
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
        => typeof(TileSetReader).AssemblyQualifiedName ?? string.Empty;

    /// <inheritdoc />
    protected override void Write(ContentWriter output, TileSetContent value)
    {
        Require.NotNull(output, nameof(output));
        Require.NotNull(value, nameof(value));

        TileSetAsset asset = value.Asset;

        if (asset.Image != null)
        {
            ExternalReference<Texture2DContent> imageReference
                = value.GetReference<Texture2DContent>(asset.Image.Source);

            output.WriteExternalReference(imageReference);
        }

        output.Write(asset.TileWidth);
        output.Write(asset.TileHeight);
        output.Write(asset.TileCount);
        output.Write(asset.Columns);
        output.Write(asset.Spacing);
        output.Write(asset.Margin);
    }
}
