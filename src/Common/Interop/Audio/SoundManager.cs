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

namespace BadEcho.Interop.Audio;

/// <summary>
/// Provides a management interface for the system's active audio devices.
/// </summary>
public sealed class SoundManager
{
    private static readonly Guid _CLSID = new("BCDE0395-E52F-467C-8E3D-C4579291692E");
    
    private readonly List<AudioDevice> _outputDevices = [];
    private readonly List<AudioDevice> _inputDevices = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundManager"/> class.
    /// </summary>
    public SoundManager()
    {
        var deviceEnumerator = Ole32.Activate<IMMDeviceEnumerator>(_CLSID,
                                                                   typeof(IMMDeviceEnumerator).GUID);

        _outputDevices.AddRange(ReadDevices(deviceEnumerator, DataFlow.Render));
        _inputDevices.AddRange(ReadDevices(deviceEnumerator, DataFlow.Capture));

        ResultHandle hResult =
            deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console, out IMMDevice defaultOutputDevice);

        if (hResult != ResultHandle.ElementNotFound)
            DefaultOutputDevice = new AudioDevice(defaultOutputDevice);

        hResult = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console, out IMMDevice defaultInputDevice);

        if (hResult != ResultHandle.ElementNotFound)
            DefaultInputDevice = new AudioDevice(defaultInputDevice);
    }

    /// <summary>
    /// Gets the output audio devices currently active.
    /// </summary>
    public IEnumerable<AudioDevice> OutputDevices
        => _outputDevices;

    /// <summary>
    /// Gets the input audio devices currently active.
    /// </summary>
    public IEnumerable<AudioDevice> InputDevices
        => _inputDevices;

    /// <summary>
    /// Gets the default output audio device, if there is one.
    /// </summary>
    public AudioDevice? DefaultOutputDevice
    { get; }

    /// <summary>
    /// Gets the default input audio device, if there is one.
    /// </summary>
    public AudioDevice? DefaultInputDevice
    { get; }

    private static IEnumerable<AudioDevice> ReadDevices(IMMDeviceEnumerator deviceEnumerator, DataFlow dataFlow)
    {
        IMMDeviceCollection devices = deviceEnumerator.EnumAudioEndpoints(dataFlow, DeviceState.Active);
        int count = devices.GetCount();

        for (int i = 0; i < count; i++)
        {
            IMMDevice device = devices.Item(i);

            yield return new AudioDevice(device);
        }
    }
}
