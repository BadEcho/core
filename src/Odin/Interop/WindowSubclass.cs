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

using System;

namespace BadEcho.Odin.Interop
{
    /// <summary>
    /// Provides a way to subclass a window in a managed environment.
    /// </summary>
    /// <remarks>
    /// This class gives managed objects the ability to subclass an unmanaged window or control. If you are unfamiliar with
    /// subclassing, it is a Microsoft term for how one can go about changing or adding additional features to an existing
    /// control or window.
    /// </remarks>
    internal sealed class WindowSubclass : IDisposable
    {
        private static readonly IntPtr _DefWindowProc = GetDefaultWindowProc();

        private WeakReference? _hook;

        public void Dispose()
        {
            _hook = null;
        }
        
        private static IntPtr GetDefaultWindowProc()
        {
            IntPtr hModule = User32.GetModuleHandle();

            return Kernel32.GetProcAddress(hModule, User32.ExportDefWindowProcW);
        }
    }
}