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

using BadEcho.Game.Fonts;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BadEcho.Game.Pipeline.Fonts;

/// <summary>
/// Provides a writer of raw multi-channel signed distance field font content to the content pipeline.
/// </summary>
[ContentTypeWriter]
public sealed class DistanceFieldFontWriter : ContentTypeWriter<DistanceFieldFontContent>
{
    /// <inheritdoc/>
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
        => typeof(DistanceFieldFontReader).AssemblyQualifiedName ?? string.Empty;

    /// <inheritdoc/>
    protected override void Write(ContentWriter output, DistanceFieldFontContent value)
    {
        Require.NotNull(output, nameof(output));
        Require.NotNull(value, nameof(value));

        ExternalReference<Texture2DContent> fontAtlasReference
            = value.GetReference<Texture2DContent>(value.FontAtlasPath);

        output.WriteExternalReference(fontAtlasReference);
        output.WriteObject(value.Characteristics);
        output.WriteObject(value.Glyphs);
        output.WriteObject(value.Kernings);
    }
}
