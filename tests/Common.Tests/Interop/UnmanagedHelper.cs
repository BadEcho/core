//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Interop;

namespace BadEcho.Tests.Interop;

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