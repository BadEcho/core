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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides an animation timing sequence for a tile on a tile map.
/// </summary>
public sealed class TileAnimation
{
    private readonly IReadOnlyList<TileAnimationFrame> _frames;
    private readonly List<QuadTextureModelData> _frameData = [];

    private float _elapsedTime;
    private int _currentFrameIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileAnimation"/> class.
    /// </summary>
    /// <param name="tileSet">The tile set the animated tile belongs to.</param>
    /// <param name="tileId">The local identifier of the animated tile within its tile set.</param>
    public TileAnimation(TileSet tileSet, int tileId)
    {
        Require.NotNull(tileSet, nameof(tileSet));

        _frames = tileSet.GetTileAnimationFrames(tileId);
        Texture2D texture = tileSet.Texture;

        foreach (TileAnimationFrame frame in _frames)
        {
            var frameData = new QuadTextureModelData();
            int frameTileId = frame.TileId;
            
            Rectangle sourceArea = tileSet.GetTileSourceArea(frameTileId);

            frameData.AddTexture(texture.Bounds, sourceArea, Vector2.Zero);

            _frameData.Add(frameData);
        }
    }

    /// <summary>
    /// Gets the model data for the current frame in the animation.
    /// </summary>
    public QuadTextureModelData CurrentFrameData
        => _frameData[_currentFrameIndex];

    /// <summary>
    /// Updates the active frame in the animation.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public void Update(GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        _elapsedTime += (int) time.ElapsedGameTime.TotalMilliseconds;

        if (_elapsedTime <= _frames[_currentFrameIndex].Duration)
            return;

        _elapsedTime -= _frames[_currentFrameIndex].Duration;
        _currentFrameIndex = (_currentFrameIndex + 1) % _frames.Count;
    }
}
