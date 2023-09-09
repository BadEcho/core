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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Fonts;

/// <summary>
/// Provides the vertex data required to render a 3D model of flat signed distance field font text.
/// </summary>
public sealed class FontModelData : QuadModelData<VertexPositionColorTexture>
{
    private readonly DistanceFieldFont _font;
    private readonly Color _color;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontModelData"/> class.
    /// </summary>
    /// <param name="font">The multi-channel signed distance font to vertex data for.</param>
    /// <param name="color">The color of the text.</param>
    public FontModelData(DistanceFieldFont font,  Color color)
        : base(VertexPositionColorTexture.VertexDeclaration)
    {
        Require.NotNull(font, nameof(font));

        _font = font;
        _color = color;
    }

    /// <summary>
    /// Adds 3D modeling data for quadrilateral surfaces that can be mapped to the specified text using glyphs found in a font
    /// atlas texture during rendering.
    /// </summary>
    /// <param name="text">The text to prepare modeling data for.</param>
    /// <param name="position">The position of the top-left corner of the text.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    /// <remarks>
    /// <para>
    /// In order to model a glyph, we create <see cref="VertexPositionColorTexture"/> values whose texture coordinates are
    /// based on the generated atlas coordinates for the glyph. These coordinates must be normalized such that they are in a range
    /// from 0 to 1 where (0, 0) is the top-left of the texture and (1, 1) is the bottom-right of the texture.
    /// </para>
    /// <para>
    /// Much like what we do with <see cref="QuadTextureModelData"/>, we divide the atlas (source) rectangle's individual vertex
    /// coordinates by the appropriate font atlas texture dimension, based on the axis that the particular vertex rests on.
    /// </para>
    /// <para>
    /// Unlike what we do with <see cref="QuadTextureModelData"/>, the source rectangle has no impact on the position of the vertices;
    /// rather, we make base the position off of the generated plane coordinates for the glyph. These coordinates are relative to the
    /// baseline cursor, and allow us to position the glyphs appropriately.
    /// </para>
    /// </remarks>
    public void AddText(string text, Vector2 position, float scale)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var advanceDirection = new Vector2(1, 0);
        // The vertical direction is the vector that's perpendicular to our advance direction.
        var verticalDirection = -1 * new Vector2(advanceDirection.Y, -advanceDirection.X);

        // The provided position vector value specifies where the top-left corner of the text should be placed.
        // The generated signed distance glyph plane coordinates, however, are meant to be applied relative to the baseline cursor.
        // Therefore, we subtract the distance between the baseline and ascender line from our initial cursor, giving us a cursor now
        // positioned at the baseline.
        Vector2 cursorStart = position + verticalDirection * scale * _font.Characteristics.Ascender * -1;
        Vector2 cursor = cursorStart;
        Vector2 scaledAdvance = advanceDirection * scale;

        for (int i = 0; i < text.Length; i++)
        {
            char character = text[i];
            FontGlyph glyph = _font.FindGlyph(character);

            if (!char.IsWhiteSpace(character)) 
                AddGlyph(glyph, _font.Characteristics, cursor, scaledAdvance);

            char? nextCharacter = i < text.Length - 1 ? text[i + 1] : null;

            cursor += _font.GetNextAdvance(character, nextCharacter, advanceDirection, scale);
        }
    }

    private void AddGlyph(FontGlyph glyph, FontCharacteristics characteristics, Vector2 cursor, Vector2 scaledAdvance)
    {
        Require.NotNull(glyph, nameof(glyph));
        Require.NotNull(characteristics, nameof(characteristics));

        RectangleF atlasBounds = glyph.AtlasBounds;
        float texelLeft = atlasBounds.X / characteristics.Width;
        float texelRight = (atlasBounds.X + atlasBounds.Width) / characteristics.Width;
        float texelTop = atlasBounds.Y / characteristics.Height;
        float texelBottom = (atlasBounds.Y + atlasBounds.Height) / characteristics.Height;

        Vector2 verticalAdvance = -1 * new Vector2(scaledAdvance.Y, -scaledAdvance.X);
        RectangleF planeBounds = glyph.PlaneBounds;
        Vector2 positionLeft = scaledAdvance * planeBounds.Left;
        Vector2 positionRight = scaledAdvance * planeBounds.Right;
        Vector2 positionTop = verticalAdvance * planeBounds.Top;
        Vector2 positionBottom = verticalAdvance * planeBounds.Bottom;

        var vertexTopLeft
            = new VertexPositionColorTexture(new Vector3(cursor + positionTop + positionLeft, 1),
                                             _color,
                                             new Vector2(texelLeft, texelTop));
        var vertexTopRight
            = new VertexPositionColorTexture(new Vector3(cursor + positionTop + positionRight, 1),
                                             _color,
                                             new Vector2(texelRight, texelTop));
        var vertexBottomLeft
            = new VertexPositionColorTexture(new Vector3(cursor + positionBottom + positionLeft, 1),
                                             _color,
                                             new Vector2(texelLeft, texelBottom));
        var vertexBottomRight
            = new VertexPositionColorTexture(new Vector3(cursor + positionBottom + positionRight, 1),
                                             _color,
                                             new Vector2(texelRight, texelBottom));

        AddVertices(vertexTopLeft, vertexTopRight, vertexBottomLeft, vertexBottomRight);
    }
}
