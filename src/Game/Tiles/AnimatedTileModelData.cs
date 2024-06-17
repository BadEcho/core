using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadEcho.Game.Tiles;

public sealed class AnimatedTileModelData : QuadTextureModelData
{

    public ICollection<TileAnimation> Animations
    { get; } = [];

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
