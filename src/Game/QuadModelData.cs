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

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides the vertex data required to render a 3D model of flat quadrilateral polygons.
/// </summary>
/// <typeparam name="TVertex">The vertex type structure to compose the 3D model with.</typeparam>
public abstract class QuadModelData<TVertex> : ModelData<TVertex>
    where TVertex : struct, IVertexType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuadModelData{TVertex}"/> class.
    /// </summary>
    /// <param name="vertexDeclaration">Value defining the shader declarations used by the vertices within the vertex data.</param>
    protected QuadModelData(VertexDeclaration vertexDeclaration)
        : base(vertexDeclaration)
    { }

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
