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

using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using BadEcho.Game.Atlases;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace BadEcho.Game.Pipeline.Atlases;

/// <summary>
/// Provides a writer of raw texture atlas content to the content pipeline.
/// </summary>
[ContentTypeWriter]
public sealed class TextureAtlasWriter : ContentTypeWriter<TextureAtlasContent>
{
    /// <inheritdoc />
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
        => typeof(TextureAtlasReader).AssemblyQualifiedName ?? string.Empty;

    /// <inheritdoc />
    protected override void Write(ContentWriter output, TextureAtlasContent value)
    {
        Require.NotNull(output, nameof(output));
        Require.NotNull(value, nameof(value));

        TextureAtlasAsset asset = value.Asset;

        ExternalReference<Texture2DContent> textureReference
            = value.GetReference<Texture2DContent>(asset.TexturePath);

        output.WriteExternalReference(textureReference);
        
        // Need to record how many regions in order to properly direct the reader.
        output.Write(asset.Regions.Count);

        foreach (TextureRegionAsset region in asset.Regions)
        {   // We need to inform the reader whether this region uses 9-slice scaling or not.
            bool isNineSliced = !region.NineSliceArea.IsEmpty;
            output.Write(isNineSliced);
            output.Write(region.Name);
            output.Write(region.SourceArea);

            if (isNineSliced)
                output.Write(region.NineSliceArea);
        }
    }
}
