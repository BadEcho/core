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
/// Provides a mechanism for animating sprites.
/// </summary>
public sealed class SpriteAnimator
{
    private readonly List<TimeSpan> _frames;

    private TimeSpan _elapsedTime;
    private bool _isPaused = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteAnimator"/> class.
    /// </summary>
    /// <param name="frames">The timing sequence for the animation's frames.</param>
    public SpriteAnimator(IEnumerable<TimeSpan> frames)
    {
        Require.NotNull(frames, nameof(frames));

        _frames = [..frames];
    }

    /// <summary>
    /// Gets the current frame of the animation.
    /// </summary>
    public int CurrentFrame
    { get; private set; }
    
    /// <summary>
    /// Plays the animation.
    /// </summary>
    public void Play() 
        => _isPaused = false;

    /// <summary>
    /// Pauses the animation on its initial frame.
    /// </summary>
    public void Pause()
        => Pause(true);

    /// <summary>
    /// Pauses the animation, optionally on its initial frame.
    /// </summary>
    /// <param name="reset">
    /// Value indicating if animation should be paused on its initial frame, as opposed to the current frame.
    /// </param>
    public void Pause(bool reset)
    {
        if (reset)
            CurrentFrame = 0;

        _isPaused = true;
    }

    /// <summary>
    /// Stops the animation from playing, resetting it to its default state.
    /// </summary>
    public void Stop()
    {
        CurrentFrame = 0;
        _isPaused = true;
        _elapsedTime = TimeSpan.Zero;
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

        _elapsedTime += time.ElapsedGameTime;

        if (_elapsedTime <= _frames[CurrentFrame])
            return;

        // Prevent lag accumulation.
        _elapsedTime -= _frames[CurrentFrame];
        CurrentFrame = (CurrentFrame + 1) % _frames.Count;
    }
}
