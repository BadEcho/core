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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides the vertex data required to render an animated 3D model of flat tiles belonging to a tile map.
/// </summary>
public sealed class AnimatedTileModelData : QuadTextureModelData
{
    /// <summary>
    /// Gets the collection of tile animations that played out when rendering this model data.
    /// </summary>
    public ICollection<TileAnimation> Animations
    { get; } = [];

    /// <summary>
    /// Advances the animations belonging to this model data.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public void Update(GameUpdateTime time)
    {
        int vertexIndex = 0;

        foreach (TileAnimation animation in Animations)
        {
            animation.Update(time);

            QuadTextureModelData currentFrameData = animation.CurrentFrameData;

            Vertices[vertexIndex++] = currentFrameData.Vertices[0];
            Vertices[vertexIndex++] = currentFrameData.Vertices[1];
            Vertices[vertexIndex++] = currentFrameData.Vertices[2];
            Vertices[vertexIndex++] = currentFrameData.Vertices[3];
        }
    }
}
