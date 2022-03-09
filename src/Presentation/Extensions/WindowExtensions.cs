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

using System.Drawing;
using System.Windows;
using BadEcho.Interop;

namespace BadEcho.Presentation.Extensions;

/// <summary>
/// Provides a set of static methods intended to simplify the use of windows and dialogs.
/// </summary>
public static class WindowExtensions
{
    /// <summary>
    /// This represents the number of WPF's device-independent units corresponding to one inch of pixels on a physical
    /// display.
    /// </summary>
    private const double PIXEL_PER_INCH = 96.0;

    /// <summary>
    /// Moves the window to the provided display device.
    /// </summary>
    /// <param name="window">The current window to move.</param>
    /// <param name="targetDisplay">The display device to move the window to.</param>
    public static void MoveToDisplay(this Window window, Display targetDisplay)
    {
        Require.NotNull(window, nameof(window));
        Require.NotNull(targetDisplay, nameof(targetDisplay));

        Rectangle targetArea = CreateTargetArea(window, targetDisplay.WorkingArea);
        Rectangle scaledTargetArea = Display.IsDpiPerMonitor
            ? NonDeviceAreaUsingPerMonitorDpi(targetDisplay, targetArea)
            : NonDeviceAreaUsingSystemDpi(targetArea);

        window.Left = scaledTargetArea.Left;
        window.Top = scaledTargetArea.Top;
        window.Width = scaledTargetArea.Width;
        window.Height = scaledTargetArea.Height;
    }

    private static Rectangle NonDeviceAreaUsingPerMonitorDpi(Display display, Rectangle referenceArea)
    {
        double pixelCoefficient = PIXEL_PER_INCH / display.MonitorDpi;

        return ApplyPixelCoefficient(pixelCoefficient, referenceArea);
    }

    private static Rectangle NonDeviceAreaUsingSystemDpi(Rectangle referenceArea)
    {
        double pixelCoefficient = PIXEL_PER_INCH / Display.SystemDpi;

        return ApplyPixelCoefficient(pixelCoefficient, referenceArea);
    }

    private static Rectangle ApplyPixelCoefficient(double pixelCoefficient, Rectangle referenceArea)
    {
        var x = (int)(referenceArea.X * pixelCoefficient);
        var y = (int)(referenceArea.Y * pixelCoefficient);
        var width = (int)(referenceArea.Width * pixelCoefficient);
        var height = (int)(referenceArea.Height * pixelCoefficient);

        return new Rectangle(x, y, width, height);
    }

    private static Rectangle CreateTargetArea(Window window, Rectangle referenceArea)
    {
        var width = (int) window.Width;
        var height = (int) window.Height;

        // If the window state is Maximized, it must be set to normal before any changes are made to size and position.
        if (window.WindowState == WindowState.Maximized)
        {
            window.WindowState = WindowState.Normal;
            // We will want to set the state back to maximized after we're done changing the size, unless the window is meant
            // to allow for transparency. In that case, WPF happens to handle the Maximized state poorly, often assigning an
            // incorrect size and position for the window, causing it to overflow onto the next monitor.
            // The Bad Echo presentation framework corrects any inaccuracies Microsoft's resizing method may introduce to transparent
            // windows, and properly size it to the reference area.
            if (!window.AllowsTransparency)
            {   // We must wait for the state to completely change to normal before switching it back to Maximized, as switching it
                // back to Maximized after changing the position and size will result in our changes being canceled.
                window.StateChanged += HandleStateChangedToNormal;
            }

            width = referenceArea.Width;
            height = referenceArea.Height;
        }

        return new Rectangle(referenceArea.X, referenceArea.Y, width, height);
    }

    private static void HandleStateChangedToNormal(object? sender, EventArgs e)
    {
        if (sender is not Window window)
            return;

        window.StateChanged -= HandleStateChangedToNormal;
        window.WindowState = WindowState.Maximized;
    }
}