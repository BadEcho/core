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

using System.Runtime.InteropServices;
using BadEcho.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Represents a custom vertex format structure that contains position, fill color, stroke color, and one set of
/// texture coordinates.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct VertexPositionOutlinedColorTexture : IVertexType, IEquatable<VertexPositionOutlinedColorTexture>
{
    /// <summary>
    /// A <see cref="Microsoft.Xna.Framework.Graphics.VertexDeclaration"/> value defining shader declarations used by
    /// vertices of this type.
    /// </summary>
    public static readonly VertexDeclaration VertexDeclaration
        = CreateDeclaration();

    /// <summary>
    /// Initializes a new instance of the <see cref="VertexPositionOutlinedColorTexture"/> class.
    /// </summary>
    /// <param name="position">The position of the vertex.</param>
    /// <param name="fillColor">The inner color of the vertex.</param>
    /// <param name="strokeColor">The outer color of the vertex.</param>
    /// <param name="textureCoordinate">The texture coordinate of the vertex.</param>
    public VertexPositionOutlinedColorTexture(Vector3 position, 
                                              Color fillColor, 
                                              Color strokeColor,
                                              Vector2 textureCoordinate)
    {
        Position = position;
        FillColor = fillColor;
        StrokeColor = strokeColor;
        TextureCoordinate = textureCoordinate;
    }

    VertexDeclaration IVertexType.VertexDeclaration
        => VertexDeclaration;
    
    /// <summary>
    /// Gets the position of the vertex.
    /// </summary>
    public Vector3 Position
    { get; }

    /// <summary>
    /// Gets the inner color of the vertex.
    /// </summary>
    public Color FillColor
    { get; }
    
    /// <summary>
    /// Gets the outer color of the vertex.
    /// </summary>
    public Color StrokeColor
    { get; }

    /// <summary>
    /// Gets the texture coordinate of the vertex.
    /// </summary>
    public Vector2 TextureCoordinate
    { get; }

    /// <summary>
    /// Determines whether two <see cref="VertexPositionOutlinedColorTexture"/> values have the same positions, colors,
    /// and texture coordinates.
    /// </summary>
    /// <param name="left">The first vertex to compare.</param>
    /// <param name="right">The second vertex to compare.</param>
    /// <returns>
    /// True if <c>left</c> represents the same vertex as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator ==(VertexPositionOutlinedColorTexture left, VertexPositionOutlinedColorTexture right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="VertexPositionOutlinedColorTexture"/> values have different positions, colors,
    /// or texture coordinates.
    /// </summary>
    /// <param name="left">The first vertex to compare.</param>
    /// <param name="right">The second vertex to compare.</param>
    /// <returns>
    /// True if <c>left</c> does not represent the same vertex as <c>right</c>; otherwise, false.
    /// </returns>
    public static bool operator !=(VertexPositionOutlinedColorTexture left, VertexPositionOutlinedColorTexture right)
        => !left.Equals(right);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is VertexPositionOutlinedColorTexture other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(Position, FillColor, StrokeColor, TextureCoordinate);

    /// <inheritdoc/>
    public bool Equals(VertexPositionOutlinedColorTexture other)
        => Position == other.Position
           && FillColor == other.FillColor
           && StrokeColor == other.StrokeColor
           && TextureCoordinate == other.TextureCoordinate;

    private static VertexDeclaration CreateDeclaration()
    {
        VertexElement[] elements =
        {
            new(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new(12, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new(16, VertexElementFormat.Color, VertexElementUsage.Color, 1),
            new(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        };

        return new VertexDeclaration(elements);
    }
}
