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
/// Provides the vertex data required to render a 3D model of flat quadrilateral polygons using rectangular regions of
/// a texture.
/// </summary>
public class QuadTextureModelData : QuadModelData<VertexPositionTexture>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuadTextureModelData"/> class.
    /// </summary>
    public QuadTextureModelData()
        : base(VertexPositionTexture.VertexDeclaration)
    { }

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

        AddVertices(vertexTopLeft, vertexTopRight, vertexBottomLeft, vertexBottomRight);
    }

    /// <inheritdoc/>
    protected override Vector3 GetVertexPosition(VertexPositionTexture vertex)
        => vertex.Position;
}