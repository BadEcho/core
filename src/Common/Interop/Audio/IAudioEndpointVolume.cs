// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace BadEcho.Interop.Audio;

/// <summary>
/// Defines volume controls on the audio stream to or from an audio endpoint device.
/// </summary>
[GeneratedComInterface]
[Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
internal partial interface IAudioEndpointVolume
{
    /// <summary>
    /// Registers a client's notification callback interface.
    /// </summary>
    /// <param name="client">The interface that the client is registering for notification callbacks.</param>
    void RegisterControlChangeNotify(IAudioEndpointVolumeCallback client);

    /// <summary>
    /// Deletes the registration of a notification callback interface.
    /// </summary>
    /// <param name="client">The interface that the client is unregistering.</param>
    void UnregisterControlChangeNotify(IAudioEndpointVolumeCallback client);

    /// <summary>
    /// Gets a count of the channels in the audio stream.
    /// </summary>
    /// <returns>The number of channels in the audio stream.</returns>
    int GetChannelCount();

    /// <summary>
    /// Sets the master volume level.
    /// </summary>
    /// <param name="fLevelDb">The new master volume level expressed in decibels.</param>
    /// <param name="pEventContext">Context value for the notification method.</param>
    void SetMasterVolumeLevel(float fLevelDb, ref Guid pEventContext);

    /// <summary>
    /// Sets the master volume level of the audio stream.
    /// </summary>
    /// <param name="fLevel">The new master volume level expressed as a normalized value in the range from 0.0 to 1.0.</param>
    /// <param name="pEventContext">Context value for the notification method.</param>
    void SetMasterVolumeLevelScalar(float fLevel, ref Guid pEventContext);

    /// <summary>
    /// Gets the master volume level.
    /// </summary>
    /// <returns>The master volume level expressed in decibels.</returns>
    float GetMasterVolumeLevel();

    /// <summary>
    /// Gets the master volume level.
    /// </summary>
    /// <returns>The master volume level expressed as a normalized value in the range from 0.0 to 1.0.</returns>
    float GetMasterVolumeLevelScalar();

    /// <summary>
    /// Sets the volume level of a channel.
    /// </summary>
    /// <param name="nChannel">The channel number.</param>
    /// <param name="fLevelDb">The new volume level expressed in decibels.</param>
    /// <param name="pEventContext">Context value for the notification method.</param>
    void SetChannelVolumeLevel(uint nChannel, float fLevelDb, ref Guid pEventContext);

    /// <summary>
    /// Sets the volume level of a channel.
    /// </summary>
    /// <param name="nChannel">The channel number.</param>
    /// <param name="fLevel">The new master volume level expressed as a normalized value in the range from 0.0 to 1.0.</param>
    /// <param name="pEventContext">Context value for the notification method.</param>
    void SetChannelVolumeLevelScalar(uint nChannel, float fLevel, ref Guid pEventContext);

    /// <summary>
    /// Gets the volume level of a channel.
    /// </summary>
    /// <param name="nChannel">The channel number.</param>
    /// <returns>The volume level of the channel expressed in decibels.</returns>
    float GetChannelVolumeLevel(uint nChannel);

    /// <summary>
    /// Gets the volume level of a channel.
    /// </summary>
    /// <param name="nChannel">The channel number.</param>
    /// <returns>The volume level of the channel expressed as a normalized value in the range from 0.0 to 1.0.</returns>
    float GetChannelVolumeLevelScalar(uint nChannel);

    /// <summary>
    /// Mutes or unmutes the device.
    /// </summary>
    /// <param name="bMute">True to mute, false to unmute.</param>
    /// <param name="pEventContext">Context value for the notification method.</param>
    void SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, ref Guid pEventContext);
    
    /// <summary>
    /// Gets a value indicating if the device is muted.
    /// </summary>
    /// <returns>True if muted; otherwise, false.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetMute();

    /// <summary>
    /// Gets information about the current step in the volume range.
    /// </summary>
    /// <param name="pnStep">Pointer that will receive the step index.</param>
    /// <returns>The number of steps in the volume range.</returns>
    uint GetVolumeStepInfo(out uint pnStep);

    /// <summary>
    /// Increments the volume level by one step.
    /// </summary>
    /// <param name="pEventContext">Context value for the notification method.</param>
    void VolumeStepUp(ref Guid pEventContext);

    /// <summary>
    /// Decrements the volume level by one step.
    /// </summary>
    /// <param name="pEventContext">Context value for the notification method.</param>
    void VolumeStepDown(ref Guid pEventContext);

    /// <summary>
    /// Queries the audio endpoint device for its hardware-supported functions.
    /// </summary>
    /// <returns>The hardware support mask.</returns>
    uint QueryHardwareSupport();

    /// <summary>
    /// Gets the volume range, in decibels, of the audio stream.
    /// </summary>
    /// <param name="pflVolumeMindB">Pointer to minimum volume level.</param>
    /// <param name="pFlVolumeMaxDb">Pointer to maximum volume level.</param>
    /// <param name="pFlVolumeIncrementDb">Pointer to the volume increment.</param>
    void GetVolumeRange(out float pflVolumeMindB, out float pFlVolumeMaxDb, out float pFlVolumeIncrementDb);
}
