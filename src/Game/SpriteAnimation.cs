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

namespace BadEcho.Game;

/// <summary>
/// Provides an animation timing sequence for a sprite.
/// </summary>
public sealed class SpriteAnimation
{
    private readonly float _framesPerSecond;
    private float _elapsedTime;
    private bool _isPaused;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimation"/> class.
    /// </summary>
    /// <param name="name">The name of the animation.</param>
    /// <param name="framesPerSecond">The number of sprite frames to draw each second.</param>
    public SpriteAnimation(string name, float framesPerSecond)
    {
        Name = name;
        _framesPerSecond = framesPerSecond;
    }

    /// <summary>
    /// Gets the name of the animation.
    /// </summary>
    public string Name
    { get; }

    /// <summary>
    /// Gets the current frame of the animation.
    /// </summary>
    public int CurrentFrame
    { get; private set; }

    /// <summary>
    /// Pauses the animation, resetting it to its initial state.
    /// </summary>
    public void Pause()
    {
        CurrentFrame = 0;
        _isPaused = true;
    }

    /// <summary>
    /// Updates the active frame in the animation.
    /// </summary>
    /// <param name="time">The game timing configuration and state for this update.</param>
    public void Update(GameUpdateTime time)
    {
        Require.NotNull(time, nameof(time));

        if (_isPaused)
            return;

        _elapsedTime += (float) time.ElapsedGameTime.TotalSeconds;
        CurrentFrame = (int) Math.Floor(_elapsedTime * _framesPerSecond);
    }
}
