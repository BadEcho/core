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

using System.Runtime.InteropServices.Marshalling;

namespace BadEcho.Interop.Audio;

/// <summary>
/// Provides an audio endpoint device.
/// </summary>
public sealed class AudioDevice
{
    private readonly IMMDevice _device;
    private readonly IAudioEndpointVolume _endpointVolume;

    private static Guid _AudioEndpointVolumeIID = typeof(IAudioEndpointVolume).GUID;
    private static Guid _Empty = Guid.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioDevice"/> class.
    /// </summary>
    /// <param name="device">The underlying audio device resource.</param>
    internal AudioDevice(IMMDevice device)
    {
        Require.NotNull(device, nameof(device));

        _device = device;
        
        unsafe
        {
            void* pEndpointVolume = _device.Activate(ref _AudioEndpointVolumeIID, ClassContext.All, nint.Zero);

            _endpointVolume = ComInterfaceMarshaller<IAudioEndpointVolume>.ConvertToManaged(pEndpointVolume)!;

            _endpointVolume.GetVolumeRange(out float minimumValue, out float _, out float _);

            MinimumVolume = minimumValue;
        }
    }

    /// <summary>
    /// Gets the endpoint ID for the device.
    /// </summary>
    public string Id 
        => _device.GetId();

    /// <summary>
    /// Gets the current state of the device.
    /// </summary>
    public DeviceState State
        => _device.GetState();

    /// <summary>
    /// Gets or sets a value indicating if the device is muted.
    /// </summary>
    public bool Mute
    {
        get => _endpointVolume.GetMute();
        set => _endpointVolume.SetMute(value, ref _Empty);
    }

    /// <summary>
    /// Gets or sets the volume of the device, in decibels.
    /// </summary>
    public float Volume
    {
        get => _endpointVolume.GetMasterVolumeLevel();
        set => _endpointVolume.SetMasterVolumeLevel(value, ref _Empty);
    }

    /// <summary>
    /// Gets the minimum volume level of the device, in decibels.
    /// </summary>
    public float MinimumVolume
    { get; }
}
