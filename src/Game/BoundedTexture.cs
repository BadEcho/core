//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under a
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a texture with spatial boundaries defined.
/// </summary>
public sealed class BoundedTexture
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedTexture"/> class.
    /// </summary>
    /// <param name="bounds">The spatial shape of the texture.</param>
    /// <param name="texture">The underlying texture data.</param>
    public BoundedTexture(IShape bounds, Texture2D texture)
    {
        Require.NotNull(bounds, nameof(bounds));
        Require.NotNull(texture, nameof(texture));

        Bounds = bounds;
        Texture = texture;
    }

    /// <summary>
    /// Gets the spatial shape of the texture.
    /// </summary>
    public IShape Bounds
    { get; }

    /// <summary>
    /// Gets the underlying texture data.
    /// </summary>
    public Texture2D Texture
    { get; }
}
