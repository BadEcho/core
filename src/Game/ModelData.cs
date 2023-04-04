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

using BadEcho.Game.Properties;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides the vertex data required to render a 3D model.
/// </summary>
public abstract class ModelData<TVertex>
    where TVertex : struct, IVertexType
{
    /// <summary>
    /// Gets the number of vertices composing the 3D model.
    /// </summary>
    public int VertexCount
        => Vertices.Count;

    /// <summary>
    /// Gets the number of indices pointing to distinct vertices within the vertex data.
    /// </summary>
    /// <remarks>
    /// A nonzero value is indicative of indexed vertex data requiring the use of an index buffer in order to be rendered properly.
    /// </remarks>
    public int IndexCount
        => Indices.Count;
    
    /// <summary>
    /// Gets the vertices composing the 3D model.
    /// </summary>
    protected IList<TVertex> Vertices
    { get; } = new List<TVertex>();

    /// <summary>
    /// Gets indices pointing to distinct vertices within the vertex data.
    /// </summary>
    protected IList<ushort> Indices
    { get; } = new List<ushort>();

    /// <summary>
    /// Loads the provided vertex buffer with the vertices for the model.
    /// </summary>
    /// <param name="vertexBuffer">The vertex buffer to load the model's vertices into.</param>
    public void LoadVertices(VertexBuffer vertexBuffer)
    {
        Require.NotNull(vertexBuffer, nameof(vertexBuffer));
        
        vertexBuffer.SetData(Vertices.ToArray(), 0, VertexCount);
    }

    /// <summary>
    /// Loads the provided index buffer with indices pointing to distinct vertices within the vertex data.
    /// </summary>
    /// <param name="indexBuffer">The index buffer to load the model's vertex indices into.</param>
    /// <exception cref="InvalidOperationException">No indices were provided for the vertex data.</exception>
    public void LoadIndices(IndexBuffer indexBuffer)
    {
        Require.NotNull(indexBuffer, nameof(indexBuffer));

        if (0 == VertexCount)
            throw new InvalidOperationException(Strings.ModelNoVertexIndices);

        indexBuffer.SetData(Indices.ToArray(), 0, IndexCount);
    }
}
