using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

public sealed class TileAnimation
{
    private readonly List<QuadTextureModelData> _frameData = [];
    private readonly List<TileAnimationFrame> _frames;

    private float _elapsedTime;
    private int _currentFrameIndex;

    public TileAnimation(TileSet tileSet, int tileId)
    {
        Require.NotNull(tileSet, nameof(tileSet));

        _frames = [..tileSet.GetTileAnimationFrames(tileId)];

        for (int i = 0; i < _frames.Count; i++)
        {
            var frameData = new QuadTextureModelData();

            Texture2D texture = tileSet.GetTileTexture(tileId);
            Rectangle sourceArea = tileSet.GetTileSourceArea(tileId);

            frameData.AddTexture(texture.Bounds, sourceArea, Vector2.Zero);

            _frameData.Add(frameData);
        }
    }

    public QuadTextureModelData CurrentFrameData
        => _frameData[_currentFrameIndex];

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
