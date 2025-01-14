//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace BadEcho.Interop;

/// <summary>
/// Provides a custom marshaller for window class information.
/// </summary>
[CustomMarshaller(typeof(WindowClass), MarshalMode.ManagedToUnmanagedRef, typeof(WindowClassMarshaller))]
internal static unsafe class WindowClassMarshaller
{
    /// <summary>
    /// Converts a managed <see cref="WindowClass"/> instance to its unmanaged counterpart.
    /// </summary>
    /// <param name="managed">A managed instance of window class information.</param>
    /// <returns>A <see cref="WNDCLASSEX"/> equivalent of <c>managed</c>.</returns>
    public static WNDCLASSEX ConvertToUnmanaged(WindowClass managed)
    {
        Require.NotNull(managed, nameof(managed));

        return new WNDCLASSEX
               {
                   cbSize = Marshal.SizeOf<WNDCLASSEX>(),
                   style = managed.Style,
                   lpfnWndProc = Marshal.GetFunctionPointerForDelegate(managed.WndProc),
                   cbClsExtra = managed.ClassExtraBytes,
                   cbWndExtra = managed.WindowExtraBytes,
                   hInstance = managed.Instance,
                   hIcon = managed.Icon,
                   hCursor = managed.Cursor,
                   hbrBackground = managed.BackgroundBrush,
                   lpszMenuName = Utf16StringMarshaller.ConvertToUnmanaged(managed.MenuName),
                   lpszClassName = Utf16StringMarshaller.ConvertToUnmanaged(managed.ClassName),
                   hIconSm = managed.SmallIcon
               };
    }

    /// <summary>
    /// Converts an unmanaged <see cref="WNDCLASSEX"/> instance to its managed counterpart.
    /// </summary>
    /// <param name="unmanaged">An unmanaged instance of window class information.</param>
    /// <returns>A <see cref="WindowClass"/> equivalent of <c>unmanaged</c>.</returns>
    public static WindowClass ConvertToManaged(WNDCLASSEX unmanaged)
    {
        var wndProc = Marshal.GetDelegateForFunctionPointer<WNDPROC>(unmanaged.lpfnWndProc);

        return new WindowClass(wndProc)
               {
                   Style = unmanaged.style,
                   ClassExtraBytes = unmanaged.cbClsExtra,
                   WindowExtraBytes = unmanaged.cbWndExtra,
                   Instance = unmanaged.hInstance,
                   Icon = unmanaged.hIcon,
                   Cursor = unmanaged.hCursor,
                   BackgroundBrush = unmanaged.hbrBackground,
                   MenuName = Utf16StringMarshaller.ConvertToManaged(unmanaged.lpszMenuName),
                   ClassName = Utf16StringMarshaller.ConvertToManaged(unmanaged.lpszClassName),
                   SmallIcon = unmanaged.hIconSm
               };
    }

    /// <summary>
    /// Represents a window class.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WNDCLASSEX
    {
        /// <summary>
        /// The size, in bytes, of this structure.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;
        /// <summary>
        /// The class style(s).
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public int style;
        /// <summary>
        /// A pointer to the window procedure.
        /// </summary>
        public IntPtr lpfnWndProc;
        /// <summary>
        /// The number of extra bytes to allocate following the window-class structure.
        /// </summary>
        public int cbClsExtra;
        /// <summary>
        /// The number of extra bytes to allocate following the window instance.
        /// </summary>
        public int cbWndExtra;
        /// <summary>
        /// A handle to the instance that contains the window procedure for the class.
        /// </summary>
        public IntPtr hInstance;
        /// <summary>
        /// A handle to the class icon.
        /// </summary>
        public IntPtr hIcon;
        /// <summary>
        /// A handle to the class cursor.
        /// </summary>
        public IntPtr hCursor;
        /// <summary>
        /// A handle to the class background brush.
        /// </summary>
        public IntPtr hbrBackground;
        /// <summary>
        /// Specifies the resource name of the class menu.
        /// </summary>
        public ushort* lpszMenuName;
        /// <summary>
        /// Either specifies the window class name or is a unique atom.
        /// </summary>
        public ushort* lpszClassName;
        /// <summary>
        /// A handle to a small icon that is associated with the window class.
        /// </summary>
        public IntPtr hIconSm;
    }
}
