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

using System.IO;
using System.Media;
using System.Windows;

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides an action that, when executed, will play a sound described by a <see cref="byte"/> array bound to
/// the dependency object this action is attached to.
/// </summary>
public sealed class PlaySoundAction : BehaviorAction<DependencyObject>
{
    /// <summary>
    /// Identifies the <see cref="Sound"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SoundProperty
        = DependencyProperty.Register(nameof(Sound),
                                      typeof(byte[]),
                                      typeof(PlaySoundAction));
    /// <summary>
    /// Identifies the <see cref="IsUninterruptible"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsUninterruptibleProperty
        = DependencyProperty.Register(nameof(IsUninterruptible),
                                      typeof(bool),
                                      typeof(PlaySoundAction));
    /// <summary>
    /// Gets or sets the array of bytes that compose the sound to play when this action is executed.
    /// </summary>
    public IEnumerable<byte>? Sound
    {
        get => (IEnumerable<byte>?) GetValue(SoundProperty);
        set => SetValue(SoundProperty, value);
    }

    /// <summary>
    /// Gets or sets a value that, when true, indicates that the sound played by this action must be played for its entire duration
    /// before another <see cref="PlaySoundAction"/> is allowed to begin its audio playback.
    /// </summary>
    /// <remarks>
    /// If this is not set to true, the execution of any subsequent <see cref="PlaySoundAction"/> will result in this action's sound being
    /// immediately interrupted. Got to make room for the new, baby!
    /// </remarks>
    public bool IsUninterruptible
    {
        get => (bool) GetValue(IsUninterruptibleProperty);
        set => SetValue(IsUninterruptibleProperty, value);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The actual method that plays the sound is fired off in an asynchronous task, as we don't want the playback
    /// to block the UI thread, which this action will most definitely be running on.
    /// </remarks>
    public override bool Execute()
    {
        // The runtime will throw a runtime error due to thread ownership issues if we don't copy the byte array from the
        // dependency property here.
        var sound = Sound?.ToList();

        if (sound == null || sound.Count == 0)
            return false;

        // The task thread won't have access to this dependency property.
        bool isUninterruptible = IsUninterruptible;
            
        Task.Run(() => PlaySound(sound, isUninterruptible));

        return true;
    }

    private static void PlaySound(IEnumerable<byte> sound, bool isUninterruptible)
    {
        using (var stream = new MemoryStream(sound.ToArray()))
        {
            using (var soundPlayer = new SoundPlayer(stream))
            {   // We call Load() first so that the stream data doesn't end up being loaded on separate worker thread, which is exactly what happens
                // if we just immediately call Play(). Once the stream data is loaded, the providing memory stream is safe to dispose.
                // On the same note, it is safe to dispose the SoundPlayer immediately after Play() returns, as it will offload the actual playing
                // of the already loaded stream data onto a worker thread, which is responsible for cleaning up itself.
                soundPlayer.Load();
                
                if (isUninterruptible)
                    soundPlayer.PlaySync();
                else
                    soundPlayer.Play();
            }
        }
    }

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
        => new PlaySoundAction();
}