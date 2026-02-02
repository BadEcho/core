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
/// Defines a collection of multimedia device resources.
/// </summary>
[GeneratedComInterface]
[Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E")]
internal partial interface IMMDeviceCollection
{
    /// <summary>
    /// Retrieves a count of the devices in the device collection.
    /// </summary>
    /// <returns>The number of devices in the collection.</returns>
    int GetCount();
    
    /// <summary>
    /// Retrieves the specified item from the device collection.
    /// </summary>
    /// <param name="nDevice">The device number to retrieve.</param>
    /// <returns>The device whose number is <c>nDevice</c>.</returns>
    IMMDevice Item(int nDevice);
}
