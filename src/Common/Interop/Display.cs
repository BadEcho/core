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

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using BadEcho.Logging;
using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides a representation of a display device connected to the computer.
/// </summary>
public sealed class Display
{
    /// <summary>
    /// Apparently the only defined flag that can be set in a <see cref="MONITORINFO"/> structure, which indicates that
    /// the monitor is the primary display.
    /// </summary>
    private const int MONITORINFOF_PRIMARY = 0x1;

    private static readonly Lazy<IEnumerable<Display>> _Displays
        = new(LoadDisplays, LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly bool _isPrimary;
    private readonly IntPtr _monitor;

    /// <summary>
    /// Initializes a new instance of the <see cref="Display"/> class.
    /// </summary>
    /// <param name="monitor">A handle to the display device this instance will encapsulate.</param>
    internal Display(IntPtr monitor)
    {
        _monitor = monitor;

        var info = MONITORINFO.CreateWritable();
            
        if (!User32.GetMonitorInfo(monitor, ref info))
            throw ((ResultHandle) Marshal.GetHRForLastWin32Error()).GetException();

        _isPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;

        MonitorDpi = LoadMonitorDpi();
        WorkingArea = LoadWorkingArea();
    }

    /// <summary>
    /// Gets a value indicating if the current operating system supports having a different DPI per monitor, as opposed to only
    /// a single system-wide DPI being in effect.
    /// </summary>
    /// <remarks>This feature was added with the release of Windows 8.1.</remarks>
    public static bool IsDpiPerMonitor
    {
        get
        {
            Version osVersion = Environment.OSVersion.Version;

            return (osVersion.Major == 6 && osVersion.Minor >= 3) || osVersion.Major >= 7;
        }
    }

    /// <summary>
    /// Gets the display devices in use by the system.
    /// </summary>
    /// <remarks>
    /// The display devices are ordered based on where each lives in the arrangement defined in the user's display settings, with
    /// the first item being the leftmost device, and the last item being the right most device.
    /// </remarks>
    public static IEnumerable<Display> Devices
        => _Displays.Value;

    /// <summary>
    /// Gets the display device designated as the primary monitor.
    /// </summary>
    public static Display Primary
        => Devices.First(d => d._isPrimary);

    /// <summary>
    /// Gets the system-wide DPI.
    /// </summary>
    public static int SystemDpi
    { get; } = LoadSystemDpi();

    /// <summary>
    /// Gets the DPI specific to this display device.
    /// </summary>
    /// <remarks>
    /// Unless the OS version for Windows is 8.1 or later (which is when per-monitor DPI support was added), the value returned
    /// by this property will always be the same value returned by <see cref="SystemDpi"/>.
    /// </remarks>
    public int MonitorDpi 
    { get; }

    /// <summary>
    /// Gets a representation of the working area (the area available for use by applications) of the display.
    /// </summary>
    public Rectangle WorkingArea
    { get; }

    private static IEnumerable<Display> LoadDisplays()
    {
        var displays = new List<Display>();

        var closure = new CallbackClosure();

        // EnumDisplayMonitors doesn't support GetLastError, so there is unfortunately no way to get more information if this fails.
        if (!User32.EnumDisplayMonitors(DeviceContextHandle.Null, IntPtr.Zero, closure.Callback, IntPtr.Zero))
            throw new Win32Exception(Strings.DisplayEnumDisplayMonitorsFailed);

        var displaysByArrangement = closure.RetrievedDisplays
                                           .OrderBy(d => d.WorkingArea.Left);

        displays.AddRange(displaysByArrangement);

        return displays;
    }

    private static int LoadSystemDpi()
    {
        using (DeviceContextHandle deviceContext = User32.GetDC(IntPtr.Zero))
        {
            return !deviceContext.IsInvalid
                ? Gdi32.GetDeviceCaps(deviceContext, DeviceInformation.PpiHeight)
                : throw ((ResultHandle) Marshal.GetHRForLastWin32Error()).GetException();
        }
    }

    private int LoadMonitorDpi()
    {
        if (!IsDpiPerMonitor)
            return SystemDpi;

        ResultHandle result = ShellScaling.GetDpiForMonitor(_monitor, MonitorDpiType.Effective, out uint dpiX, out _);

        if (result != ResultHandle.Success)
        {
            Logger.Warning(Strings.DisplayGetDpiForMonitorFailed.InvariantFormat((int) result));

            return SystemDpi;
        }

        return (int) dpiX;
    }

    private Rectangle LoadWorkingArea()
    {
        var info = MONITORINFO.CreateWritable();

        return User32.GetMonitorInfo(_monitor, ref info)
            ? Rectangle.FromLTRB(info.rcWork.Left, info.rcWork.Top, info.rcWork.Right, info.rcWork.Bottom)
            : throw ((ResultHandle) Marshal.GetHRForLastWin32Error()).GetException();
    }

    /// <summary>
    /// Provides a referencing environment for a <see cref="MonitorEnumProc"/> callback.
    /// </summary>
    private sealed class CallbackClosure
    {
        private readonly List<Display> _retrievedDisplays = new();

        /// <summary>
        /// Gets the <see cref="Display"/> instances provided to the callback.
        /// </summary>
        public IEnumerable<Display> RetrievedDisplays
            => _retrievedDisplays;

        /// <summary>
        /// Processes <see cref="User32.EnumDisplayMonitors"/> results by storing the returned display information.
        /// </summary>
        /// <returns>Value indicating whether enumeration should continue.</returns>
        /// <remarks>
        /// The parameters consisting of a discard symbol followed by an integer are named so in order to tell Microsoft's various
        /// code analyzers that this signature is required in order to be able to use this method as a delegate.
        /// </remarks>
        /// <seealso cref="MonitorEnumProc"/>
        public bool Callback(IntPtr hMonitor, IntPtr _0, IntPtr _1, IntPtr _2)
        {
            var display = new Display(hMonitor);

            _retrievedDisplays.Add(display);

            return true;
        }
    }
}