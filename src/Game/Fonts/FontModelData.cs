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

using Microsoft.Xna.Framework;

namespace BadEcho.Game.Fonts;

/// <summary>
/// Provides the vertex data required to render a 3D model of flat signed distance field font text.
/// </summary>
public sealed class FontModelData : QuadModelData<VertexPositionOutlinedColorTexture>
{
    private readonly DistanceFieldFont _font;
    private readonly Color _fillColor;
    private readonly Color _strokeColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontModelData"/> class.
    /// </summary>
    /// <param name="font">The multi-channel signed distance font to vertex data for.</param>
    /// <param name="color">The color of the text.</param>
    public FontModelData(DistanceFieldFont font, Color color)
        : this(font, color, default)
    {
        FillOnly = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontModelData"/> class.
    /// </summary>
    /// <param name="font">The multi-channel signed distance font to vertex data for.</param>
    /// <param name="fillColor">The inner color of the text.</param>
    /// <param name="strokeColor">The outer color of the text.</param>
    public FontModelData(DistanceFieldFont font, Color fillColor, Color strokeColor)
        : base(VertexPositionOutlinedColorTexture.VertexDeclaration)
    {
        Require.NotNull(font, nameof(font));

        _font = font;
        _fillColor = fillColor;
        _strokeColor = strokeColor;
    }

    /// <summary>
    /// Gets a value indicating if the coloring of the text consists of only a single fill color, without any outlining.
    /// </summary>
    public bool FillOnly
    { get; }

    /// <summary>
    /// Adds 3D modeling data for quadrilateral surfaces that can be mapped to the specified text using glyphs found in a font
    /// atlas texture during rendering.
    /// </summary>
    /// <param name="text">The text to prepare modeling data for.</param>
    /// <param name="position">The position of the top-left corner of the text.</param>
    /// <param name="scale">The amount of scaling to apply to the text.</param>
    /// <remarks>
    /// <para>
    /// In order to model a glyph, we create <see cref="VertexPositionOutlinedColorTexture"/> values whose texture coordinates are
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
        PointF? offset = position;

        for (int i = 0; i < text.Length; i++)
        { 
            char character = text[i];
            FontGlyph glyph = _font.FindGlyph(character);

            if (!char.IsWhiteSpace(character)) 
                AddGlyph(glyph, _font.Characteristics, cursor, scaledAdvance, offset);

            char? nextCharacter = i < text.Length - 1 ? text[i + 1] : null;

            cursor += _font.GetNextAdvance(character, nextCharacter, advanceDirection, scale);

            // Only apply an offset the starting quad corner of the text. Another offset may be needed if a line break is inserted.
            offset = null;
        }
    }

    /// <inheritdoc/>
    protected override Vector3 GetVertexPosition(VertexPositionOutlinedColorTexture vertex)
        => vertex.Position;

    private void AddGlyph(FontGlyph glyph, FontCharacteristics characteristics, Vector2 cursor, Vector2 scaledAdvance, PointF? offset)
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
            = new VertexPositionOutlinedColorTexture(new Vector3(cursor + positionTop + positionLeft, 0),
                                                     _fillColor,
                                                     _strokeColor,
                                                     new Vector2(texelLeft, texelTop));
        var vertexTopRight
            = new VertexPositionOutlinedColorTexture(new Vector3(cursor + positionTop + positionRight, 0),
                                                     _fillColor,
                                                     _strokeColor,
                                                     new Vector2(texelRight, texelTop));
        var vertexBottomLeft
            = new VertexPositionOutlinedColorTexture(new Vector3(cursor + positionBottom + positionLeft, 0),
                                                     _fillColor,
                                                     _strokeColor,
                                                     new Vector2(texelLeft, texelBottom));
        var vertexBottomRight
            = new VertexPositionOutlinedColorTexture(new Vector3(cursor + positionBottom + positionRight, 0),
                                                     _fillColor,
                                                     _strokeColor,
                                                     new Vector2(texelRight, texelBottom));

        AddVertices(vertexTopLeft, vertexTopRight, vertexBottomLeft, vertexBottomRight);

        if (offset != null)
            AddQuadCornerOffset(vertexTopLeft, offset.Value);
    }
}
