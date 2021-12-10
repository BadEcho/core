//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Media;
using System.Windows;

namespace BadEcho.Fenestra.Behaviors;

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
    /// Gets or sets the array of bytes that compose the sound to play when this action is executed.
    /// </summary>
    public IEnumerable<byte>? Sound
    {
        get => (IEnumerable<byte>?) GetValue(SoundProperty);
        set => SetValue(SoundProperty, value);
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

        if (sound == null || !sound.Any())
            return false;
            
        Task.Run(() => PlaySound(sound));

        return true;
    }

    private static void PlaySound(IEnumerable<byte> sound)
    {
        using (var stream = new MemoryStream(sound.ToArray()))
        {
            using (var soundPlayer = new SoundPlayer(stream))
            {   // A synchronous playback call is required here so we don't end up disposing the sound player prior to the sound
                // playback completing.
                soundPlayer.PlaySync();
            }
        }
    }

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
        => new PlaySoundAction();
}