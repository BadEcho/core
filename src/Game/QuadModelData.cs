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
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides the vertex data required to render a 3D model of flat quadrilateral polygons.
/// </summary>
/// <typeparam name="TVertex">The vertex type structure to compose the 3D model with.</typeparam>
public abstract class QuadModelData<TVertex> : ModelData<TVertex>
    where TVertex : struct, IVertexType
{
    private readonly Dictionary<TVertex, PointF> _quadCornerOffsetMap = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="QuadModelData{TVertex}"/> class.
    /// </summary>
    /// <param name="vertexDeclaration">Value defining the shader declarations used by the vertices within the vertex data.</param>
    protected QuadModelData(VertexDeclaration vertexDeclaration)
        : base(vertexDeclaration)
    { }

    /// <inheritdoc/>
    public override SizeF MeasureSize()
    {
        float left = 0;
        float top = 0;
        float right = 0;
        float bottom = 0;

        for (int i = 0; i < IndexCount / 6; i++)
        {
            TVertex topLeftVertex = Vertices[i * 4 + 0];
            
            Vector3 topLeft = GetVertexPosition(topLeftVertex);
            Vector3 topRight = GetVertexPosition(Vertices[i * 4 + 1]);
            Vector3 bottomLeft = GetVertexPosition(Vertices[i * 4 + 2]);
            Vector3 bottomRight = GetVertexPosition(Vertices[i * 4 + 3]);

            float topY = Math.Min(topLeft.Y, topRight.Y);
            float bottomY = Math.Max(bottomLeft.Y, bottomRight.Y);
            float leftX = Math.Min(topLeft.X, bottomLeft.X);
            float rightX = Math.Max(topRight.X, bottomRight.X);

            if (_quadCornerOffsetMap.TryGetValue(topLeftVertex, out PointF offset))
            {
                left = offset.X;
                top = offset.Y;           
            }

            left = Math.Min(leftX, left);
            top = Math.Min(topY, top);
            right = Math.Max(rightX, right);
            bottom = Math.Max(bottomY, bottom);
        }

        var area = new RectangleF(left, top, right - left, bottom - top);

        return area.Size;
    }

    /// <summary>
    /// Adds 3D modeling data for a quadrilateral surface defined by the specified vertices.
    /// </summary>
    /// <param name="topLeft">The top-left vertex of the quadrilateral.</param>
    /// <param name="topRight">The top-right vertex of the quadrilateral.</param>
    /// <param name="bottomLeft">The bottom-left vertex of the quadrilateral.</param>
    /// <param name="bottomRight">The bottom-right vertex of the quadrilateral.</param>
    protected void AddVertices(TVertex topLeft, TVertex topRight, TVertex bottomLeft, TVertex bottomRight)
    {
        AddIndices(VertexCount);

        Vertices.Add(topLeft);
        Vertices.Add(topRight);
        Vertices.Add(bottomLeft);
        Vertices.Add(bottomRight);
    }

    /// <summary>
    /// Adds an adjustment to pre-rendered size measurements involving the specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex to apply the offset to.</param>
    /// <param name="offset">The amount to offset the vertex's position.</param>
    protected void AddQuadCornerOffset(TVertex vertex, PointF offset) 
        => _quadCornerOffsetMap.Add(vertex, offset);

    /// <summary>
    /// Gets the drawing location of the specified vertex.
    /// </summary>
    /// <param name="vertex">The vertex to get the position for.</param>
    /// <returns>The position of <c>vertex</c>.</returns>
    protected abstract Vector3 GetVertexPosition(TVertex vertex);

    private void AddIndices(int offset)
    {   // The order the indices are defined is important as it defines the winding direction of the rendered triangles.
        // The triangles must be clockwise winding or else they will be automatically culled by MonoGame.
        // Therefore, the second triangle needs to have an entry for the vertex opposite of its hypotenuse (3) added before its leftmost
        // vertex (2), which is shared by both triangles, in order to establish a clockwise winding direction.
        Indices.Add((ushort) (0 + offset));
        Indices.Add((ushort) (1 + offset));
        Indices.Add((ushort) (2 + offset));
        Indices.Add((ushort) (1 + offset));
        Indices.Add((ushort) (3 + offset));
        Indices.Add((ushort) (2 + offset));
    }
}
