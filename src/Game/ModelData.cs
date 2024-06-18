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

using BadEcho.Game.Properties;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides the vertex data required to render a 3D model.
/// </summary>
/// <typeparam name="TVertex">The vertex type structure to compose the 3D model with.</typeparam>
public abstract class ModelData<TVertex> : IModelData
    where TVertex : struct, IVertexType
{
    private readonly List<TVertex> _vertices = [];
    private readonly List<ushort> _indices = [];

    private TVertex[]? _vertexData;
    private ushort[]? _indexData;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelData{TVertex}"/> class.
    /// </summary>
    /// <param name="vertexDeclaration">Value defining the shader declarations used by the vertices within the vertex data.</param>
    protected ModelData(VertexDeclaration vertexDeclaration)
    {
        VertexDeclaration = vertexDeclaration;
    }

    /// <inheritdoc/>
    public int VertexCount
        => _vertices.Count;

    /// <inheritdoc/>
    public int IndexCount
        => _indices.Count;

    /// <inheritdoc/>
    public VertexDeclaration VertexDeclaration 
    { get; }

    /// <inheritdoc/>
    public void LoadVertices(VertexBuffer vertexBuffer)
    {
        Require.NotNull(vertexBuffer, nameof(vertexBuffer));
        
        vertexBuffer.SetData(GetVertexData(), 0, VertexCount);
    }

    /// <inheritdoc/>
    public void LoadIndices(IndexBuffer indexBuffer)
    {
        Require.NotNull(indexBuffer, nameof(indexBuffer));

        if (0 == VertexCount)
            throw new InvalidOperationException(Strings.ModelNoVertexIndices);

        indexBuffer.SetData(GetIndexData(), 0, IndexCount);
    }

    /// <inheritdoc/>
    public abstract SizeF MeasureSize();

    /// <summary>
    /// Adds a vertex to the model's vertex data.
    /// </summary>
    /// <param name="vertex">The <typeparam name="TVertex"/> value to add to the model's vertex data.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="GetVertexData"/> has been invoked and no further vertex data can be added.
    /// </exception>
    protected void AddVertex(TVertex vertex)
    {
        if (_vertexData != null)
            throw new InvalidOperationException(Strings.VertexDataAlreadyLoaded);

        _vertices.Add(vertex);
    }

    /// <summary>
    /// Adds an index pointing to a distinct vertex within the vertex data.
    /// </summary>
    /// <param name="index">An index to a distinct vertex within the vertex data.</param>
    /// <exception cref="InvalidOperationException">
    /// <see cref="GetIndexData"/> has been invoked and no further index data can be added.
    /// </exception>
    protected void AddIndex(ushort index)
    {
        if (_indexData != null)
            throw new InvalidOperationException(Strings.IndexDataAlreadyLoaded);

        _indices.Add(index);
    }

    /// <summary>
    /// Loads the raw array of vertex data that will be provided to a vertex buffer.
    /// </summary>
    /// <returns>An array of <typeparamref name="TVertex"/> values to be loaded into a vertex buffer.</returns>
    protected TVertex[] GetVertexData() 
        => _vertexData ??= [.._vertices];

    /// <summary>
    /// Loads the raw array of index data that will be provided to an index buffer.
    /// </summary>
    /// <returns>An array of <see cref="ushort"/> values to be loaded into an index buffer.</returns>
    protected ushort[] GetIndexData()
        => _indexData ??= [.._indices];
}
