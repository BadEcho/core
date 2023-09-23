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

using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Defines the vertex data required to render a 3D model.
/// </summary>
public interface IModelData
{
    /// <summary>
    /// Gets the number of vertices composing the 3D model.
    /// </summary>
    int VertexCount { get; }

    /// <summary>
    /// Gets the number of indices pointing to distinct vertices within the vertex data.
    /// </summary>
    /// <remarks>
    /// A nonzero value is indicative of indexed vertex data requiring the use of an index buffer in order to be rendered properly.
    /// </remarks>
    int IndexCount { get; }

    /// <summary>
    /// Gets a <see cref="Microsoft.Xna.Framework.Graphics.VertexDeclaration"/> value defining the shader declarations used by the
    /// vertices within the vertex data.
    /// </summary>
    VertexDeclaration VertexDeclaration { get; }

    /// <summary>
    /// Loads the provided vertex buffer with the vertices for the model.
    /// </summary>
    /// <param name="vertexBuffer">The vertex buffer to load the model's vertices into.</param>
    void LoadVertices(VertexBuffer vertexBuffer);

    /// <summary>
    /// Loads the provided index buffer with indices pointing to distinct vertices within the vertex data.
    /// </summary>
    /// <param name="indexBuffer">The index buffer to load the model's vertex indices into.</param>
    /// <exception cref="InvalidOperationException">No indices were provided for the vertex data.</exception>
    void LoadIndices(IndexBuffer indexBuffer);

    /// <summary>
    /// Calculates the size of the described model's contents when rendered.
    /// </summary>
    /// <returns>A <see cref="SizeF"/> value representing the size of the described model's contents when rendered.</returns>
    SizeF MeasureSize();
}
