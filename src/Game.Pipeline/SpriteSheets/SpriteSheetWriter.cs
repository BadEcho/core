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

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BadEcho.Game.Pipeline.SpriteSheets;

/// <summary>
/// Provides a writer of raw sprite sheet content to the content pipeline.
/// </summary>
[ContentTypeWriter]
public sealed class SpriteSheetWriter : ContentTypeWriter<SpriteSheetContent>
{
    /// <inheritdoc />
    public override string GetRuntimeReader(TargetPlatform targetPlatform) 
        => typeof(SpriteSheetReader).AssemblyQualifiedName ?? string.Empty;

    /// <inheritdoc />
    protected override void Write(ContentWriter output, SpriteSheetContent value)
    {
        Require.NotNull(output, nameof(output));
        Require.NotNull(value, nameof(value));

        SpriteSheetAsset asset = value.Asset;

        ExternalReference<Texture2DContent> textureReference 
            = value.GetReference<Texture2DContent>(asset.TexturePath);

        output.WriteExternalReference(textureReference);
        output.Write(asset.ColumnCount);
        output.Write(asset.RowCount);
        output.Write(asset.InitialFrame);

        // Record the count to guide the reader.
        output.Write(asset.Animations.Count);

        foreach (SpriteAnimationAsset animation in asset.Animations)
        {
            output.Write(animation.Name);
            output.Write(animation.StartFrame);
            output.Write(animation.EndFrame);
        }
    }
}
