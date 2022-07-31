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

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides a writer of raw sprite sheet content into the content pipeline.
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

        ExternalReference<Texture2DContent> externalReference 
            = value.GetReference<Texture2DContent>(value.Asset.TexturePath);

        output.WriteExternalReference(externalReference);
        output.Write(value.Asset.Rows);
        output.Write(value.Asset.Columns);
        output.Write(value.Asset.RowUp);
        output.Write(value.Asset.RowDown);
        output.Write(value.Asset.RowLeft);
        output.Write(value.Asset.RowRight);
        output.Write(value.Asset.RowInitial);
    }
}
