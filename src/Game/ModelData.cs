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
        => Vertices.Count;

    /// <inheritdoc/>
    public int IndexCount
        => Indices.Count;

    /// <inheritdoc/>
    public VertexDeclaration VertexDeclaration 
    { get; }

    /// <summary>
    /// Gets the vertices composing the 3D model.
    /// </summary>
    public IList<TVertex> Vertices
    { get; } = new List<TVertex>();

    /// <summary>
    /// Gets indices pointing to distinct vertices within the vertex data.
    /// </summary>
    public IList<ushort> Indices
    { get; } = new List<ushort>();

    /// <inheritdoc/>
    public void LoadVertices(VertexBuffer vertexBuffer)
    {
        Require.NotNull(vertexBuffer, nameof(vertexBuffer));
        
        vertexBuffer.SetData(Vertices.ToArray(), 0, VertexCount);
    }

    /// <inheritdoc/>
    public void LoadIndices(IndexBuffer indexBuffer)
    {
        Require.NotNull(indexBuffer, nameof(indexBuffer));

        if (0 == VertexCount)
            throw new InvalidOperationException(Strings.ModelNoVertexIndices);

        indexBuffer.SetData(Indices.ToArray(), 0, IndexCount);
    }

    /// <inheritdoc/>
    public abstract SizeF MeasureSize();
}
