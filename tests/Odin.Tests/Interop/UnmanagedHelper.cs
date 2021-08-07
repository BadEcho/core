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
using System.Collections.Generic;
using BadEcho.Odin.Interop;

namespace BadEcho.Odin.Tests.Interop
{
    /// <summary>
    /// Provides some helper functions for testing-related purposes.
    /// </summary>
    public class UnmanagedHelper
    {
        public static IEnumerable<IntPtr> EnumerateMonitors()
        {
            var closure = new MonitorCallbackClosure();

            User32.EnumDisplayMonitors(DeviceContextHandle.Null, IntPtr.Zero, closure.Callback, IntPtr.Zero);

            return closure.Monitors;
        }
    }
}
