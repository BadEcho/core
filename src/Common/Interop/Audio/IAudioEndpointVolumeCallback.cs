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
/// Defines a mechanism for receiving notifications of changes in the volume level and muting state of an audio
/// endpoint device.
/// </summary>
[GeneratedComInterface]
[Guid("657804FA-D6AD-4496-8A60-352752AF4F89")]
internal partial interface IAudioEndpointVolumeCallback
{
    /// <summary>
    /// Notifies the client that the volume level or muting state of the audio endpoint device has changed.
    /// </summary>
    /// <param name="pNotifyData">Pointer to the volume-notification data.</param>
    void OnNotify(nint pNotifyData);
}
