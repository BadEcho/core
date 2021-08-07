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
    /// Specifies extended styling configurations for windows that can be set using
    /// <see cref="User32.SetWindowLongPtr(IntPtr, WindowAttribute, IntPtr)"/>.
    /// </summary>
    [Flags]
    public enum ExtendedWindowStyles
    {
        /// <summary>
        /// The window should not be painted until siblings beneath the window created by the same thread have been painted.
        /// </summary>
        Transparent = 0x00000020
    }
}
