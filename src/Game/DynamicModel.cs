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
/// Provides a generated model of dynamically changing (i.e., animated) 3D triangle primitives for rendering.
/// </summary>
public sealed class DynamicModel : PrimitiveModel<DynamicVertexBuffer, DynamicIndexBuffer>
{
    private readonly IModelData _modelData;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicModel"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use when rendering the model.</param>
    /// <param name="texture">The texture to map onto the model.</param>
    /// <param name="modelData">The vertex data required to render the model.</param>
    public DynamicModel(GraphicsDevice device, Texture2D texture, IModelData modelData)
        : base(device, texture, modelData)
    {
        _modelData = modelData;
    }

    public void UpdateVertices() 
        => _modelData.LoadVertices(VertexBuffer);

    /// <inheritdoc />
    protected override DynamicVertexBuffer CreateVertexBuffer(IModelData modelData)
    {
        var vertexBuffer
            = new DynamicVertexBuffer(Device, modelData.VertexDeclaration, modelData.VertexCount, BufferUsage.WriteOnly);

        modelData.LoadVertices(vertexBuffer);
        
        return vertexBuffer;
    }

    /// <inheritdoc />
    protected override DynamicIndexBuffer CreateIndexBuffer(IModelData modelData)
    {
        var indexBuffer
            = new DynamicIndexBuffer(Device, IndexElementSize.SixteenBits, modelData.IndexCount, BufferUsage.WriteOnly);

        modelData.LoadIndices(indexBuffer);

        return indexBuffer;
    }
}
