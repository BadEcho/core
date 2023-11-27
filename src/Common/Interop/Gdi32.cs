//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with the Graphics Device Interface functionality of Windows.
/// </summary>
internal static partial class Gdi32
{
    private const string LIBRARY_NAME = "gdi32";

    /// <summary>
    /// Gets the stock object type for a null brush.
    /// </summary>
    public const int StockObjectBrushNull = 5;

    /// <summary>
    /// Creates a logical brush that has the specified solid color.
    /// </summary>
    /// <param name="color">The color of the brush.</param>
    /// <returns>The logical brush if successful; otherwise, null.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr CreateSolidBrush(int color);
    
    /// <summary>
    /// Retrieves device-specific information for the specified device.
    /// </summary>
    /// <param name="hdc">A handle to the device context.</param>
    /// <param name="nIndex">An enumeration value that specifies the item to be returned.</param>
    /// <returns>The value for the requested information type.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int GetDeviceCaps(DeviceContextHandle hdc, DeviceInformation nIndex);

    /// <summary>
    /// Retrieves a handle to one of the stock pens, brushes, fonts, or palettes.
    /// </summary>
    /// <param name="stockObject">The type of stock object.</param>
    /// <returns>A handle to the requested logical object if successful; otherwise, null.</returns>
    [LibraryImport(LIBRARY_NAME, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial IntPtr GetStockObject(int stockObject);

    /// <summary>
    /// Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources
    /// associated with the obj et.
    /// </summary>
    /// <param name="hObject">A handle to the object.</param>
    /// <returns>A nonzero value if the function succeeds; otherwise, zero.</returns>
    [LibraryImport(LIBRARY_NAME)]
    [return: MarshalAs(UnmanagedType.Bool)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial bool DeleteObject(IntPtr hObject);
}