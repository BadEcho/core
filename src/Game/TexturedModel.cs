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

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a generated texture-mapped model of 3D triangle primitives for rendering.
/// </summary>
public sealed class TexturedModel : PrimitiveModel
{
    private readonly Texture2D _texture;
    
    /// <inheritdoc />
    public TexturedModel(GraphicsDevice device, Texture2D texture, VertexPositionTexture[] vertices, ushort[] indices) 
        : base(device, vertices, indices)
    {
        Require.NotNull(texture, nameof(texture));

        _texture = texture;
    }

    /// <inheritdoc />
    public TexturedModel(GraphicsDevice device, Texture2D texture, VertexPositionTexture[] vertices) 
        : base(device, vertices)
    {
        Require.NotNull(texture, nameof(texture));

        _texture = texture;
    }

    /// <inheritdoc/>
    protected override void ConfigureEffect(BasicEffect effect)
    {
        base.ConfigureEffect(effect);

        effect.TextureEnabled = true;
        effect.Texture = _texture;
    }
}
