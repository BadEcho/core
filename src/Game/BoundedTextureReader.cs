//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under a
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;

namespace BadEcho.Game;

/// <summary>
/// Provides a reader of raw bounded texture content from the content pipeline.
/// </summary>
public sealed class BoundedTextureReader : ContentTypeReader<BoundedTexture>
{
    /// <inheritdoc />
    protected override BoundedTexture Read(ContentReader input, BoundedTexture existingInstance)
    {
        Require.NotNull(input, nameof(input));

        var shapeType = (ShapeType) input.ReadInt32();
        var size = input.ReadVector2();
        var texture = input.ReadObject<Texture2D>();

        IShape bounds = ReadBounds(shapeType, size);

        return new BoundedTexture(bounds, texture);
    }

    private static IShape ReadBounds(ShapeType shapeType, SizeF size)
        => shapeType switch
        {
            ShapeType.RectangleCustom or ShapeType.RectangleSource => new RectangleF(PointF.Empty, size),
            ShapeType.CircleCustom => new Circle(PointF.Empty, size.Width),
            _ => throw new InvalidEnumArgumentException(nameof(shapeType), (int) shapeType, typeof(ShapeType))
        };
}