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

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a base generated model of 3D triangle primitives for rendering.
/// </summary>
/// <typeparam name="TVertexBuffer">
/// The type of <see cref="VertexBuffer"/> that holds the vertex buffer data for the model.
/// </typeparam>
/// <typeparam name="TIndexBuffer">
/// The type of <see cref="IndexBuffer"/> that holds the index buffer data for the model.
/// </typeparam>
public abstract class PrimitiveModel<TVertexBuffer, TIndexBuffer> : IPrimitiveModel
    where TVertexBuffer : VertexBuffer
    where TIndexBuffer : IndexBuffer
{
    private readonly Lazy<TVertexBuffer> _vertexBuffer;
    private readonly Lazy<TIndexBuffer>? _indexBuffer;
    private readonly int _primitiveCount;
    private readonly Texture2D? _texture;

    private bool _disposed;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveModel{TVertexBuffer,TIndexBuffer}"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use when rendering the model.</param>
    /// <param name="texture">The texture to map onto the model.</param>
    /// <param name="modelData">The vertex data required to render the model.</param>
    protected PrimitiveModel(GraphicsDevice device, Texture2D texture, ModelData<VertexPositionTexture> modelData)
    {
        Require.NotNull(device, nameof(device));
        Require.NotNull(texture, nameof(texture));
        Require.NotNull(modelData, nameof(modelData));
        
        Device = device;
        _texture = texture;

        if (modelData.IndexCount != 0)
        {
            _primitiveCount = modelData.IndexCount / 3;
            
            _indexBuffer = new Lazy<TIndexBuffer>(() => CreateIndexBuffer(modelData),
                                                  LazyThreadSafetyMode.ExecutionAndPublication);
        }
        else
            _primitiveCount = modelData.VertexCount / 3;

        _vertexBuffer = new Lazy<TVertexBuffer>(() => CreateVertexBuffer(modelData),
                                                LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Gets the graphics device to use when rendering the model.
    /// </summary>
    protected GraphicsDevice Device 
    { get; }

    /// <summary>
    /// Gets the loaded vertex buffer for the model.
    /// </summary>
    protected TVertexBuffer VertexBuffer
        => _vertexBuffer.Value;

    /// <summary>
    /// Gets the loaded index buffer for the model if index buffer data was provided at initialization; otherwise, null.
    /// </summary>
    protected TIndexBuffer? IndexBuffer
        => _indexBuffer?.Value;

    /// <inheritdoc/>
    public void Draw(BasicEffect effect)
    {
        Require.NotNull(effect, nameof(effect));

        if (_texture != null)
        {
            effect.TextureEnabled = true;
            effect.Texture = _texture;
        }
        
        Device.SetVertexBuffer(VertexBuffer);

        if (IndexBuffer != null)
            Device.Indices = IndexBuffer;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            if (IndexBuffer == null)
                Device.DrawPrimitives(PrimitiveType.TriangleList, 0, _primitiveCount);
            else
                Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primitiveCount);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and (optionally) managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            if (_vertexBuffer.IsValueCreated)
                VertexBuffer.Dispose();

            if (_indexBuffer is {IsValueCreated: true})
                IndexBuffer?.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    /// Creates and loads the model's vertex buffer with vertices from the the provided model data.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertices to load into the vertex buffer.</typeparam>
    /// <param name="modelData">The model data to load into the created vertex buffer.</param>
    /// <returns>
    /// A <typeparamref name="TVertexBuffer"/> instance loaded with vertices from <c>modelData</c>.
    /// </returns>
    protected abstract TVertexBuffer CreateVertexBuffer<TVertex>(ModelData<TVertex> modelData)
        where TVertex : struct, IVertexType;

    /// <summary>
    /// Creates and loads the model's index buffer with vertex indices from the provided model data.
    /// </summary>
    /// <typeparam name="TVertex">The type of vertices used by the provided model data.</typeparam>
    /// <param name="modelData">The model data to load into the created index buffer.</param>
    /// <returns>
    /// A <typeparamref name="TIndexBuffer"/> instance loaded with vertex indices from <c>modelData</c>.
    /// </returns>
    protected abstract TIndexBuffer CreateIndexBuffer<TVertex>(ModelData<TVertex> modelData)
        where TVertex : struct, IVertexType;
}
