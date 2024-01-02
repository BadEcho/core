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
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace BadEcho.Game.Pipeline.BoundedTextures;

/// <summary>
/// Provides a writer of raw bounded texture content to the content pipeline.
/// </summary>
[ContentTypeWriter]
public sealed class BoundedTextureWriter : ContentTypeWriter<BoundedTextureContent>
{
    /// <inheritdoc/>
    public override string GetRuntimeReader(TargetPlatform targetPlatform)
        => typeof(BoundedTextureReader).AssemblyQualifiedName ?? string.Empty;

    /// <inheritdoc/>
    protected override void Write(ContentWriter output, BoundedTextureContent value)
    {
        Require.NotNull(output, nameof(output));
        Require.NotNull(value, nameof(value));

        output.Write((int) value.ShapeType);
        output.Write(value.Size);
        output.WriteObject(value.Texture);
    }
}
