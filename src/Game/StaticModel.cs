//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a generated model of static 3D triangle primitives for rendering.
/// </summary>
public sealed class StaticModel : PrimitiveModel<VertexBuffer, IndexBuffer>
{
    /// <inheritdoc />
    public StaticModel(GraphicsDevice device, Texture2D texture, ModelData<VertexPositionTexture> modelData) 
        : base(device, texture, modelData)
    { }

    /// <inheritdoc />
    protected override VertexBuffer CreateVertexBuffer<TVertex>(ModelData<TVertex> modelData)
    {
        var vertexBuffer
            = new VertexBuffer(Device, VertexPosition.VertexDeclaration, modelData.VertexCount, BufferUsage.WriteOnly);

        modelData.LoadVertices(vertexBuffer);

        return vertexBuffer;
    }

    /// <inheritdoc />
    protected override IndexBuffer CreateIndexBuffer<TVertex>(ModelData<TVertex> modelData)
    {
        var indexBuffer
            = new IndexBuffer(Device, IndexElementSize.SixteenBits, modelData.IndexCount, BufferUsage.WriteOnly);

        modelData.LoadIndices(indexBuffer);

        return indexBuffer;
    }
}
