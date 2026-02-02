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
/// Defines the generic features of a multimedia device resource.
/// </summary>
[GeneratedComInterface(StringMarshalling = StringMarshalling.Utf16)]
[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
internal partial interface IMMDevice
{
    /// <summary>
    /// Creates a COM object with the specified interface.
    /// </summary>
    /// <param name="iid">The interface identifier.</param>
    /// <param name="dwClsCtx">The execution context in which the code that manages the newly created object will run.</param>
    /// <param name="pActivationParams">Activation parameters that depend on the interface being specified.</param>
    /// <returns>Pointer to the interface specified by the parameter <c>iid</c>.</returns>
    unsafe void* Activate(ref Guid iid,
                          ClassContext dwClsCtx,
                          nint pActivationParams);
    /// <summary>
    /// Retrieves an interface to the device's property store.
    /// </summary>
    /// <param name="sAccessMode">The storage-access mode.</param>
    /// <returns>The property store for the device.</returns>
    IPropertyStore OpenPropertyStore(StorageAccessMode sAccessMode);

    /// <summary>
    /// Retrieves an endpoint ID string that identifies the device.
    /// </summary>
    /// <returns>The endpoint ID string for the device.</returns>
    string GetId();

    /// <summary>
    /// Retrieves the current device state.
    /// </summary>
    /// <returns>The current device state.</returns>
    DeviceState GetState();
}
