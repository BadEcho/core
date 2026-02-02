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
/// Defines methods for enumerating multimedia device resources.
/// </summary>
[GeneratedComInterface(StringMarshalling = StringMarshalling.Utf16)]
[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6")]
internal partial interface IMMDeviceEnumerator
{
    /// <summary>
    /// Generates a collection of audio endpoint devices that meet the specified criteria.
    /// </summary>
    /// <param name="dataFlow">The data-flow direction for the endpoint devices in the collection.</param>
    /// <param name="dwStateMask">The state or states of the endpoints that are to be included in the collection.</param>
    /// <returns>An <see cref="IMMDeviceCollection"/> instance containing the requested devices.</returns>
    IMMDeviceCollection EnumAudioEndpoints(DataFlow dataFlow, DeviceState dwStateMask);

    /// <summary>
    /// Retrieves the default audio endpoint for the specified data-flow direction and role.
    /// </summary>
    /// <param name="dataFlow">The data-flow direction for the endpoint device.</param>
    /// <param name="role">The role of the endpoint device.</param>
    /// <param name="device">The default audio endpoint device for the specified data-flow direction and role.</param>
    /// <returns>The result of the operation.</returns>
    [PreserveSig]
    ResultHandle GetDefaultAudioEndpoint(DataFlow dataFlow, Role role, out IMMDevice device);

    /// <summary>
    /// Retrieves an audio endpoint device that is identified by an endpoint ID string.
    /// </summary>
    /// <param name="id">The endpoint ID.</param>
    /// <returns>The audio endpoint device whose ID is <c>id</c>.</returns>
    IMMDevice GetDevice(string id);

    /// <summary>
    /// Registers a client's notification callback interface.
    /// </summary>
    /// <param name="client">The interface that the client is registering for notification callbacks.</param>
    void RegisterEndpointNotificationCallback(IMMNotificationClient client);

    /// <summary>
    /// Deletes the registration of a notification callback interface.
    /// </summary>
    /// <param name="client">The interface that the client is unregistering.</param>
    void UnregisterEndpointNotificationCallback(IMMNotificationClient client);
}
