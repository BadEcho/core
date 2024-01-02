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

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a generated model of static 3D triangle primitives for rendering.
/// </summary>
public sealed class StaticModel : PrimitiveModel<VertexBuffer, IndexBuffer>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StaticModel"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use when rendering the model.</param>
    /// <param name="texture">The texture to map onto the model.</param>
    /// <param name="modelData">The vertex data required to render the model.</param>
    public StaticModel(GraphicsDevice device, Texture2D texture, IModelData modelData) 
        : base(device, texture, modelData)
    { }

    /// <inheritdoc />
    protected override VertexBuffer CreateVertexBuffer(IModelData modelData)
    {
        var vertexBuffer
            = new VertexBuffer(Device, modelData.VertexDeclaration, modelData.VertexCount, BufferUsage.WriteOnly);

        modelData.LoadVertices(vertexBuffer);
        
        return vertexBuffer;
    }

    /// <inheritdoc />
    protected override IndexBuffer CreateIndexBuffer(IModelData modelData)
    {
        var indexBuffer
            = new IndexBuffer(Device, IndexElementSize.SixteenBits, modelData.IndexCount, BufferUsage.WriteOnly);

        modelData.LoadIndices(indexBuffer);

        return indexBuffer;
    }
}
