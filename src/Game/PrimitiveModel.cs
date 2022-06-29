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
/// Provides a generated model of 3D triangle primitives for rendering.
/// </summary>
public class PrimitiveModel : IDisposable   
{
    private readonly VertexBuffer _vertexBuffer;
    private readonly IndexBuffer? _indexBuffer;
    private readonly GraphicsDevice _device;
    private readonly int _primitiveCount;

    private bool _disposed;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveModel"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use when rendering the model.</param>
    /// <param name="vertices">The vertex buffer data for the model.</param>
    /// <param name="indices">The index buffer data for the model.</param>
    public PrimitiveModel(GraphicsDevice device, VertexPositionTexture[] vertices, ushort[] indices)
        : this(device, vertices)
    {
        Require.NotNull(indices, nameof(indices));
        // We base the number of primitives on the index buffer data if present as opposed to the vertex buffer data.
        _primitiveCount = indices.Length / 3;

        _indexBuffer 
            = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);

        _indexBuffer.SetData(indices, 0, indices.Length);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrimitiveModel"/> class.
    /// </summary>
    /// <param name="device">The graphics device to use when rendering the model.</param>
    /// <param name="vertices">The vertex buffer data for the model.</param>
    public PrimitiveModel(GraphicsDevice device, VertexPositionTexture[] vertices)
    {
        Require.NotNull(device, nameof(device));
        Require.NotNull(vertices, nameof(vertices));
        
        _device = device;
        _primitiveCount = vertices.Length / 3;

        _vertexBuffer 
            = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);

        _vertexBuffer.SetData(vertices, 0, vertices.Length);
    }

    /// <summary>
    /// Draws the model to the screen.
    /// </summary>
    /// <param name="effect">The shaders to be used during the rendering of this model.</param>
    public void Draw(BasicEffect effect)
    {
        Require.NotNull(effect, nameof(effect));

        ConfigureEffect(effect);

        _device.SetVertexBuffer(_vertexBuffer);
        _device.Indices = _indexBuffer;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();

            _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _primitiveCount);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Configures the provided shaders in preparation for drawing the model.
    /// </summary>
    /// <param name="effect">The shaders to be used during the rendering of this model.</param>
    protected virtual void ConfigureEffect(BasicEffect effect)
    { }

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
            _vertexBuffer.Dispose();
            _indexBuffer?.Dispose();
        }

        _disposed = true;
    }
}
