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

using System.Runtime.InteropServices;

namespace BadEcho.Odin.Interop
{
    /// <summary>
    /// Provides interoperability with the Graphics Device Interface functionality of Windows.
    /// </summary>
    internal static class Gdi32
    {
        private const string LIBRARY_NAME = "gdi32";

        /// <summary>
        /// Retrieves device-specific information for the specified device.
        /// </summary>
        /// <param name="hdc">A handle to the device context.</param>
        /// <param name="nIndex">An enumeration value that specifies the item to be returned.</param>
        /// <returns>The value for the requested information type.</returns>
        [DllImport(LIBRARY_NAME, ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern int GetDeviceCaps(DeviceContextHandle hdc, DeviceInformation nIndex);
    }
}