//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides the vertex data required to render a 3D model of flat quadrilateral polygons.
/// </summary>
/// <remarks>
/// Quadrilateral polygons defined by this type of model data are expressed and subsequently rendered as pairs of adjacent
/// 3D triangle primitives.
/// </remarks>
public sealed class QuadModelData : ModelData<VertexPositionTexture>
{
    /// <summary>
    /// Adds 3D modeling data for a quadrilateral surface that can be mapped to a particular region of a texture during rendering.
    /// </summary>
    /// <param name="textureBounds">
    /// The dimensions of the texture the mapped region is being sourced from, required for purposes of texture coordinate normalization.
    /// </param>
    /// <param name="sourceArea">The bounding rectangle of the region of the texture to create modeling data for.</param>
    /// <param name="position">The drawing location of the model.</param>
    /// <remarks>
    /// <para>
    /// In order to source a particular region of a texture, we need to create <see cref="VertexPositionTexture"/> values whose
    /// texture coordinates are normalized such that they are in a range from 0 to 1 where (0, 0) is the top-left of the texture
    /// and (1, 1) is the bottom-right of the texture.
    /// </para>
    /// <para>
    /// In order to normalize the specified source region's coordinates, all we need do is simply divide the source rectangle's
    /// individual vertex coordinates by the appropriate texture dimension, based on the axis that the particular vertex rests on. 
    /// </para>
    /// </remarks>
    public void AddTexture(Rectangle textureBounds, Rectangle sourceArea, Vector2 position)
    {
        float texelLeft = (float) sourceArea.X / textureBounds.Width;
        float texelRight = (float) (sourceArea.X + sourceArea.Width) / textureBounds.Width;
        float texelTop = (float) sourceArea.Y / textureBounds.Height;
        float texelBottom = (float) (sourceArea.Y + sourceArea.Height) / textureBounds.Height;

        var vertexTopLeft
            = new VertexPositionTexture(new Vector3(position, 0),
                                        new Vector2(texelLeft, texelTop));
        var vertexTopRight
            = new VertexPositionTexture(new Vector3(position + new Vector2(sourceArea.Width, 0), 0),
                                        new Vector2(texelRight, texelTop));
        var vertexBottomLeft
            = new VertexPositionTexture(new Vector3(position + new Vector2(0, sourceArea.Height), 0),
                                        new Vector2(texelLeft, texelBottom));
        var vertexBottomRight
            = new VertexPositionTexture(new Vector3(position + new Vector2(sourceArea.Width, sourceArea.Height), 0),
                                        new Vector2(texelRight, texelBottom));

        AddIndices(VertexCount);

        Vertices.Add(vertexTopLeft);
        Vertices.Add(vertexTopRight);
        Vertices.Add(vertexBottomLeft);
        Vertices.Add(vertexBottomRight);
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