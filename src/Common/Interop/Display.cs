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

using BadEcho.Extensions;
using BadEcho.Logging;
using BadEcho.Properties;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Provides a representation of a display monitor connected to the computer.
/// </summary>
public sealed class Display
{
    /// <summary>
    /// Apparently the only defined flag that can be set in a <see cref="MONITORINFOEX"/> structure, which indicates that
    /// the monitor is the primary display device.
    /// </summary>
    private const int MONITORINFOF_PRIMARY = 0x1;

    /// <summary>
    /// The assumed screen DPI for displays that have a scale factor of 100 percent (WinUser.h).
    /// </summary>
    private const double USER_DEFAULT_SCREEN_DPI = 96.0;

    private static readonly Lazy<IEnumerable<Display>> _Displays
        = new(LoadDisplays, LazyThreadSafetyMode.ExecutionAndPublication);
    
    private readonly IntPtr _monitor;

    private DeviceMode _currentMode = new();
    private bool _isPrimary;

    /// <summary>
    /// Initializes a new instance of the <see cref="Display"/> class.
    /// </summary>
    /// <param name="monitor">A handle to the display monitor this instance will encapsulate.</param>
    internal Display(IntPtr monitor)
    {
        _monitor = monitor;

        LoadInfo();

        // EnumDisplaySettings doesn't support GetLastError, so we can't get any meaningful error data.
        // Regardless, we want to fail since we may introduce problems trying to commit an uninitialized DEVMODE when changing settings.
        if (!User32.EnumDisplaySettings(Name, User32.EnumCurrentSettings, ref _currentMode))
            throw new Win32Exception(Strings.DisplayEnumDisplaySettingsFailed.InvariantFormat(Name));
    }

    /// <summary>
    /// Gets a value indicating if the current process is aware of each monitor's DPI and is notified of any changes to their DPI
    /// settings.
    /// </summary>
    public static bool IsDpiPerMonitor
        => GetDpiAwareness() == DpiAwareness.PerMonitorAware;

    /// <summary>
    /// Gets the display monitors in use by the system.
    /// </summary>
    /// <remarks>
    /// The display monitors are ordered based on where each lives in the arrangement defined in the user's display settings, with
    /// the first item being the leftmost monitor, and the last item being the right most monitor.
    /// </remarks>
    public static IEnumerable<Display> Devices
        => _Displays.Value;

    /// <summary>
    /// Gets the monitor designated as the primary display device.
    /// </summary>
    public static Display Primary
        => Devices.First(d => d._isPrimary);

    /// <summary>
    /// Gets the system-wide DPI.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is the effective DPI that all displays will be treated as having if the process's DPI awareness is
    /// anything other than <see cref="DpiAwareness.PerMonitorAware"/>.
    /// </para>
    /// <para>
    /// The value for this is never cached because, although <see cref="DpiAwareness.SystemAware"/> specifies that
    /// the application does not receive changes made to the system DPI, the DPI awareness context for the process
    /// itself can change at any time, which can influence the effective DPI.
    /// </para>
    /// </remarks>
    public static int SystemDpi
        => (int) User32.GetDpiForSystem();

    /// <summary>
    /// Gets the name of this display monitor.
    /// </summary>
    public string? Name
    { get; private set; }

    /// <summary>
    /// Gets the DPI specific to this display monitor.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This will only return the individual monitor's DPI if the process is per-monitor DPI aware, indicated by
    /// <see cref="IsDpiPerMonitor"/>. If the process is not per-monitor DPI aware, then the value returned will
    /// always be the same value as <see cref="SystemDpi"/>.
    /// </para>
    /// <para>
    /// This value is never cached, and is queried for each time this property is accessed. If the process is per-monitor
    /// DPI aware, it must adhere to live changes made to a monitor's DPI; additionally, the DPI awareness context of a
    /// thread or process can be changed at any time, which can influence the individual monitor's DPI.
    /// </para>
    /// </remarks>
    public int MonitorDpi
    {
        get
        {
            if (!IsDpiPerMonitor)
                return SystemDpi;
            
            // GetDpiForMonitor only returns a valid DPI for the monitor if we are per-monitor DPI aware.
            ResultHandle result = ShellScaling.GetDpiForMonitor(_monitor, MonitorDpiType.Effective, out uint dpiX, out _);

            if (result != ResultHandle.Success)
            {
                Logger.Warning(Strings.DisplayGetDpiForMonitorFailed.InvariantFormat((int)result));

                return SystemDpi;
            }

            return (int) dpiX;
        }
    }

    /// <summary>
    /// Gets the scale factor applied to the size of text, apps, and other items on the display.
    /// </summary>
    /// <remarks>
    /// This value is not cached because the process's DPI awareness context can change at any time,
    /// which influences the scale factor; additionally, if the process is <see cref="DpiAwareness.PerMonitorAware"/>,
    /// then we need to reflect live changes to the monitor's DPI.
    /// </remarks>
    public double ScaleFactor
    {
        get
        {
            switch (GetDpiAwareness())
            {
                // If the process is DPI unaware, then we'll need to call GetScaleFactorForMonitor to get an accurate scale.
                case DpiAwareness.Unaware:
                    ResultHandle result = ShellScaling.GetScaleFactorForMonitor(_monitor, out int scalePercentage);

                    if (result != ResultHandle.Success)
                    {
                        Logger.Warning(Strings.DisplayGetScaleFactorForMonitorFailed.InvariantFormat((int) result));

                        return 1.0;
                    }

                    return scalePercentage / 100.0;
                // The GetScaleFactorForMonitor will be inaccurate if we have any level of DPI awareness, so we calculate it ourselves.
                default:
                    int dpi = MonitorDpi;

                    return dpi / USER_DEFAULT_SCREEN_DPI;
            }
        }
    }

    /// <summary>
    /// Gets a representation of the working area (the area available for use by applications) of the display.
    /// </summary>
    public Rectangle WorkingArea
    { get; private set; }

    /// <summary>
    /// Retrieves the display monitor containing the largest area of intersection with the bounding rectangle of a specified
    /// window.
    /// </summary>
    /// <param name="window">A handle to the window of interest.</param>
    /// <returns>The <see cref="Display"/> instance containing the largest area of intersection with the <c>window</c>.</returns>
    public static Display FromWindow(WindowHandle window)
    {
        Require.NotNull(window, nameof(window));

        // Default to the nearest monitor if the window is not intersecting any display monitor.
        return new Display(User32.MonitorFromWindow(window, 0x2));
    }

    /// <summary>
    /// Makes this monitor the primary display device.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Setting a monitor as the new primary display device is a more complex process than one might initially assume.
    /// It involves more than just specifying <see cref="ChangeDisplaySettingsFlags.SetPrimary"/> when updating the graphics mode.
    /// </para>
    /// <para>
    /// In addition to this, the new primary display device must be positioned such that its upper-left corner is on the origin
    /// of the coordinate plane. This means all other display devices must be adjusted as well in order to maintain their layout.
    /// </para>
    /// <para>
    /// Since this requires updating the graphics modes of multiple devices, we stage the individual positional changes and then bring
    /// them all into effect at once by calling <c>ChangeDisplaySettingsEx</c> with a null device. None of the changes get committed
    /// to the registry until this final call, so there is no need to worry about an inconsistent registry state should any of these
    /// steps fail.
    /// </para>
    /// </remarks>
    public unsafe void MakePrimary()
    {
        if (_isPrimary)
            return;

        IEnumerable<Display> otherDisplays = Devices.Where(d => d != this);

        int xOffset = 0 - _currentMode.PositionX;
        int yOffset = 0 - _currentMode.PositionY;

        Move(xOffset, yOffset, false);

        var result = User32.ChangeDisplaySettingsEx(Name,
                                                    ref _currentMode,
                                                    IntPtr.Zero,
                                                    ChangeDisplaySettingsFlags.SetPrimary |
                                                    ChangeDisplaySettingsFlags.UpdateRegistry |
                                                    ChangeDisplaySettingsFlags.NoReset,
                                                    null);

        if (result != ChangeDisplaySettingsResult.Successful)
            throw new Win32Exception(Strings.DisplayChangePrimaryFailed.InvariantFormat(result));
        
        foreach (var otherDisplay in otherDisplays)
        {
            otherDisplay.Move(xOffset, yOffset, true);
        }

        ApplyChanges(true);
    }

    private static List<Display> LoadDisplays()
    {
        var displays = new List<Display>();

        var closure = new CallbackClosure();

        // EnumDisplayMonitors doesn't support GetLastError, so there is unfortunately no way to get more information if this fails.
        if (!User32.EnumDisplayMonitors(DeviceContextHandle.InvalidHandle, IntPtr.Zero, closure.Callback, IntPtr.Zero))
            throw new Win32Exception(Strings.DisplayEnumDisplayMonitorsFailed);

        var displaysByArrangement = closure.RetrievedDisplays
                                           .OrderBy(d => d.WorkingArea.Left);

        displays.AddRange(displaysByArrangement);

        return displays;
    }

    /// <summary>
    /// Gets the DPI awareness of the process.
    /// </summary>
    /// <returns>
    /// A <see cref="DpiAwareness"/> value that specifies the current DPI awareness of the process.
    /// </returns>
    /// <remarks>
    /// The returned DPI awareness value should never be cached. This is because the DPI awareness context of a thread
    /// or process can be changed at any time.
    /// </remarks>
    private static DpiAwareness GetDpiAwareness()
    {
        IntPtr awarenessContext = User32.GetThreadDpiAwarenessContext();

        return User32.GetAwarenessFromDpiAwarenessContext(awarenessContext);
    }

    private void LoadInfo()
    {
        var info = MONITORINFOEX.CreateWritable();

        if (!User32.GetMonitorInfo(_monitor, ref info))
            throw ((ResultHandle)Marshal.GetHRForLastWin32Error()).GetException();

        Name = new string(info.Device);

        _isPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;

        WorkingArea
            = Rectangle.FromLTRB(info.rcWork.Left, info.rcWork.Top, info.rcWork.Right, info.rcWork.Bottom);
    }

    private unsafe void ApplyChanges(bool allDisplays)
    {
        var result = allDisplays
            ? User32.ChangeDisplaySettingsEx(null, IntPtr.Zero, IntPtr.Zero, ChangeDisplaySettingsFlags.ApplyChanges, null)
            : User32.ChangeDisplaySettingsEx(Name, ref _currentMode, IntPtr.Zero, ChangeDisplaySettingsFlags.ApplyChanges, null);

        if (result != ChangeDisplaySettingsResult.Successful)
            throw new Win32Exception(Strings.DisplayApplyDisplaySettingsFailed.InvariantFormat(result));

        foreach (var display in Devices)
        {
            display.LoadInfo();
        }
    }

    private unsafe void Move(int xOffset, int yOffset, bool stage)
    {
        _currentMode.PositionX += xOffset;
        _currentMode.PositionY += yOffset;
        _currentMode.Fields = DeviceModeFields.Position;

        if (!stage)
            return;
        
        var result = User32.ChangeDisplaySettingsEx(Name,
                                                    ref _currentMode,
                                                    IntPtr.Zero,
                                                    ChangeDisplaySettingsFlags.NoReset |
                                                    ChangeDisplaySettingsFlags.UpdateRegistry,
                                                    null);

        if (result != ChangeDisplaySettingsResult.Successful)
            throw new Win32Exception(Strings.DisplayChangePositionFailed.InvariantFormat(result));
    }

    /// <summary>
    /// Provides a referencing environment for a <see cref="MonitorEnumProc"/> callback.
    /// </summary>
    private sealed class CallbackClosure
    {
        private readonly List<Display> _retrievedDisplays = [];

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