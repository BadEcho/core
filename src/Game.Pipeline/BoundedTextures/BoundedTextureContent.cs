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

using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace BadEcho.Game.Pipeline.BoundedTextures;

/// <summary>
/// Provides the raw data for a texture with spatial boundaries defined.
/// </summary>
public sealed class BoundedTextureContent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedTextureContent"/> class.
    /// </summary>
    /// <param name="shapeType">An enumeration value specifying this texture's spatial shape type.</param>
    /// <param name="size">The size of the texture's spatial shape.</param>
    /// <param name="texture">The underlying texture content, processed by the built-in texture processor.</param>
    public BoundedTextureContent(ShapeType shapeType, SizeF size, TextureContent texture)
    {
        ShapeType = shapeType;
        Size = size;
        Texture = texture;
    }

    /// <summary>
    /// Gets a <see cref="ShapeType"/> value that specifies this texture's spatial shape type.
    /// </summary>
    public ShapeType ShapeType
    { get; }

    /// <summary>
    /// Gets the size of the texture's spatial shape.
    /// </summary>
    public SizeF Size
    { get; }

    /// <summary>
    /// Gets the underlying texture content, processed by the built-in texture processor.
    /// </summary>
    public TextureContent Texture 
    { get; }
}
