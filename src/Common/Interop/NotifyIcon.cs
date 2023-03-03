//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Buffers;
using System.ComponentModel;
using System.Runtime.InteropServices;
using BadEcho.Logging;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides a taskbar notification area icon.
/// </summary>
/// <suppressions>
/// ReSharper disable RedundantOverflowCheckingContext
/// (ReSharper incorrectly assumes no overflow checks exist when adding offsets to pointers)
/// </suppressions>
public sealed class NotifyIcon : IDisposable
{
    private static readonly WindowMessage _TrayEvent
        = User32.RegisterWindowMessage("NotificationAreaIcon.TrayEvent");

    private static readonly int _BitDepth
        = LoadBitDepth();

    private readonly IWindowWrapper _windowWrapper;
    private readonly IconHandle _iconHandle;
    private readonly Guid _id;

    private IconHandle? _balloonIconHandle;
    private string _tooltip;
    private bool _isVisible; 
    private bool _isAdded;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyIcon"/> class.
    /// </summary>
    /// <param name="windowWrapper">
    /// A wrapper around the window that will receive messages associated with the notification area icon.
    /// </param>
    /// <param name="tooltip">The text for the icon's tooltip.</param>
    /// <param name="data">The binary contents of the icon's .ico file.</param>
    /// <remarks>
    /// This will load all resources associated with the notification area icon, however it will not be displayed
    /// until <see cref="Show"/> is called.
    /// </remarks>
    public NotifyIcon(IWindowWrapper windowWrapper, string tooltip, byte[] data)
    {
        Require.NotNull(windowWrapper, nameof(windowWrapper));
        Require.NotNull(tooltip, nameof(tooltip));
        Require.NotNull(data, nameof(data));

        _windowWrapper = windowWrapper;
         
        _id = Guid.NewGuid();

        _tooltip = tooltip;
        _iconHandle = LoadIconHandle(data,
                                     User32.GetSystemMetrics(SystemMetric.SmallIconWidth),
                                     User32.GetSystemMetrics(SystemMetric.SmallIconHeight));

        _windowWrapper.AddHook(WndProc);
    }

    /// <summary>
    /// Occurs when the user has left clicked the icon.
    /// </summary>
    public event EventHandler? LeftClicked;

    /// <summary>
    /// Occurs when the user has right clicked the icon.
    /// </summary>
    public event EventHandler? RightClicked;

    /// <summary>
    /// Gets or sets the text for the icon's tooltip.
    /// </summary>
    public string Tooltip
    {
        get => _tooltip;
        set
        {
            if (_tooltip == value)
                return;

            _tooltip = value;

            if (_isVisible)
                Update();
        }
    }

    /// <summary>
    /// Displays the icon in the taskbar's notification area.
    /// </summary>
    /// <remarks>
    /// Following initialization, the notification area icon is not displayed until this method is called.
    /// </remarks>
    public void Show()
    {
        if (_isVisible)
            return;

        _isVisible = true;

        Update();
    }

    /// <summary>
    /// Removes the icon from the taskbar's notification area.
    /// </summary>
    public void Hide()
    {
        if (!_isVisible)
            return;

        _isVisible = false;

        Update();
    }

    /// <summary>
    /// Loads a custom icon to use for balloon notifications.
    /// </summary>
    /// <param name="data">The binary contents of the custom balloon icon's .ico file.</param>
    /// <remarks>
    /// <para>
    /// Modern day balloon notifications are in essence Toast popup notifications. While documentation
    /// for the notification icon API states that customized balloon icons are sized using SM_CXICON
    /// and SM_CYICON system metrics (typically 32x32), this is not actually the case in practice.
    /// </para>
    /// <para>
    /// In reality, the "balloon" notification popups seem to adhere to the current Windows desktop
    /// shell design documentation on Toast popups, which states that image dimensions should be 48x48
    /// pixels at 100% scaling.
    /// </para>
    /// <para>
    /// There is no system metric for this image size, so we need to load the icon that best fits a desired
    /// 48x48 pixel size, with the relevant display's scaling applied.
    /// </para>
    /// <para>
    /// Balloon notifications seem to only ever displayed on the primary display (regardless of whether
    /// the taskbar is on that display or another), so we refer to the scale factor of the primary
    /// display during our calculations.
    /// </para>
    /// <para>
    /// To support all possible DPI configurations, an .ico file should provide the following sizes for the
    /// balloon icon:
    /// <list type="bullet">
    /// <item>48x48</item>
    /// <item>60x60</item>
    /// <item>72x72</item>
    /// <item>96x96</item>
    /// <item>192x192</item>
    /// </list>
    /// </para>
    /// </remarks>
    public void LoadBalloonIcon(byte[] data)
    {
        Require.NotNull(data, nameof(data));

        _balloonIconHandle?.Dispose();

        Display primaryDisplay = Display.Primary;
        int iconSize = (int) primaryDisplay.ScaleFactor * 48;

        _balloonIconHandle = LoadIconHandle(data, iconSize, iconSize);
    }

    /// <summary>
    /// Sends a "balloon" notification to the user's desktop. 
    /// </summary>
    /// <remarks>
    /// In reality, the notification is much more akin to a Toast notification with newer versions of Windows, as opposed
    /// to an old school balloon-shaped tooltip.
    /// </remarks>
    /// <param name="text">The text to display on the notification.</param>
    /// <param name="title">The title to display on the notification.</param>
    /// <param name="iconType">
    /// An enumeration value specifying the type of balloon icon to display in the notification.
    /// </param>
    public void SendBalloonNotification(string text, string title, BalloonIconType iconType)
    {
        if (!_isAdded)
        {
            Logger.Warning(Strings.NotifyIconBalloonRequiresTaskbar);
            return;
        }

        NotifyIconInfoFlags infoFlags;

        if (iconType == BalloonIconType.Custom)
        {
            if (_balloonIconHandle == null)
                throw new ArgumentException(Strings.NotifyIconNoCustomBalloonIconLoaded, nameof(iconType));

            infoFlags = NotifyIconInfoFlags.User | NotifyIconInfoFlags.LargeIcon;
        }
        else
            infoFlags = (NotifyIconInfoFlags) iconType;

        var iconData = new NotifyIconData(_windowWrapper.Handle, _id, NotifyIconFlags.Info)
                       {
                           Info = text,
                           InfoTitle = title,
                           InfoFlags = infoFlags,
                           BalloonIcon = _balloonIconHandle
                       };

        if (!Shell32.Shell_NotifyIcon(NotifyIconMessage.Modify, ref iconData))
            throw new Win32Exception(Strings.NotifyIconBalloonFailed);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        Hide();

        _windowWrapper.RemoveHook(WndProc);
        _iconHandle.Dispose();
        _balloonIconHandle?.Dispose();

        _disposed = true;
    }

    private static int LoadBitDepth()
    {
        using (DeviceContextHandle deviceContext = User32.GetDC(IntPtr.Zero))
        {
            int pixelBits = Gdi32.GetDeviceCaps(deviceContext, DeviceInformation.BitsPixel);
            int colorPlanes = Gdi32.GetDeviceCaps(deviceContext, DeviceInformation.ColorPlanes);

            return pixelBits * colorPlanes;
        }
    }

    private static unsafe IconHandle LoadIconHandle(byte[] data, int width, int height)
    {
        uint bestFitResourceSize = 0;

        fixed (byte* pData = data)
        {
            IconHandle iconHandle;
            ICONDIR* iconDir = (ICONDIR*)pData;
            byte bestFitWidth = 0;
            byte bestFitHeight = 0;
            uint bestFitBitDepth = 0;
            uint bestFitImageOffset = 0;

            if (iconDir->idCount == 0)
                throw new ArgumentException(Strings.IconNoEntries, nameof(data));

            // Let's check if the data payload is big enough to describe all the images it says it does.
            if (sizeof(ICONDIRENTRY) * (iconDir->idCount - 1) + sizeof(ICONDIR) > data.Length)
                throw new ArgumentException(Strings.IconTooSmallForEntries, nameof(data));

            var iconEntries = new ReadOnlySpan<ICONDIRENTRY>(&iconDir->idEntries, iconDir->idCount);

            foreach (var iconEntry in iconEntries)
            {
                bool bestFit = false;
                uint iconBitDepth = iconEntry.wBitCount;

                if (bestFitResourceSize == 0)
                    bestFit = true;
                else
                {
                    int bestFitDelta = Math.Abs(bestFitWidth - width) + Math.Abs(bestFitHeight - height);
                    int currentDelta = Math.Abs(iconEntry.bWidth - width) + Math.Abs(iconEntry.bHeight - height);

                    // If the difference between desired/actual width/height is smaller, then this is the best fit so far.
                    if (currentDelta < bestFitDelta)
                        bestFit = true;
                    // If they're the same, we take a look at the bit depth.
                    else if (currentDelta == bestFitDelta)
                    {   // If the icon's bit depth is closer to our device's without exceeding it, it's a best fit.
                        if (iconEntry.wBitCount <= _BitDepth && iconEntry.wBitCount > bestFitBitDepth)
                            bestFit = true;
                        // If the bit depth does exceed our device's, but is still closer than the current best fit, then it's better!
                        else if (bestFitBitDepth > _BitDepth && iconEntry.wBitCount < bestFitBitDepth)
                            bestFit = true;
                    }
                }

                if (bestFit)
                {
                    bestFitWidth = iconEntry.bWidth;
                    bestFitHeight = iconEntry.bHeight;
                    bestFitImageOffset = iconEntry.dwImageOffset;
                    bestFitResourceSize = iconEntry.dwBytesInRes;
                    bestFitBitDepth = iconBitDepth;
                }
            }

            uint imageOffsetEnd;

            try
            {
                imageOffsetEnd = checked(bestFitImageOffset + bestFitResourceSize);
            }
            catch (OverflowException oEx)
            {
                throw new ArgumentException(Strings.IconImageOffsetOverflow, nameof(data), oEx);
            }

            if (imageOffsetEnd > data.Length)
                throw new ArgumentException(Strings.IconImageExceedsFile, nameof(data));

            // If the image in the icon data is unaligned, we copy it over to an aligned buffer.
            if (bestFitImageOffset % IntPtr.Size != 0)
            {   // In order to rent out a pre-allocated array, our size needs to be convertible to an int.
                if (bestFitResourceSize > int.MaxValue)
                    throw new ArgumentException(Strings.IconImageTooLarge, nameof(data));

                byte[] alignedBuffer = ArrayPool<byte>.Shared.Rent((int)bestFitResourceSize);
                Array.Copy(data, bestFitImageOffset, alignedBuffer, 0, bestFitResourceSize);

                fixed (byte* pAlignedBuffer = alignedBuffer)
                {
                    iconHandle = User32.CreateIconFromResourceEx(pAlignedBuffer, bestFitResourceSize, true, 0x30000, 0, 0, 0);
                }

                ArrayPool<byte>.Shared.Return(alignedBuffer);
            }
            else
            {
                try
                {
                    iconHandle = User32.CreateIconFromResourceEx(checked(pData + bestFitImageOffset),
                                                                 bestFitResourceSize,
                                                                 true,
                                                                 0x30000,
                                                                 0,
                                                                 0,
                                                                 0);
                }
                catch (OverflowException oEx)
                {
                    throw new ArgumentException(Strings.IconFileTooLarge, nameof(data), oEx);
                }
            }

            if (iconHandle.IsInvalid)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return iconHandle;
        }
    }

    private void Update()
    {
        var iconData
            = new NotifyIconData(_windowWrapper.Handle,
                                 _id,
                                 NotifyIconFlags.Message | NotifyIconFlags.Icon | NotifyIconFlags.Tip)
              {
                  CallbackMessage = (uint) _TrayEvent,
                  Icon = _iconHandle,
                  Tip = _tooltip
              };
            
        if (_isVisible)
        {
            var message = _isAdded ? NotifyIconMessage.Modify : NotifyIconMessage.Add;

            if (!Shell32.Shell_NotifyIcon(message, ref iconData))
                throw new Win32Exception(Strings.NotifyIconAddModifyFailed);
            
            _isAdded = true;
        }
        else
        {
            if (!Shell32.Shell_NotifyIcon(NotifyIconMessage.Delete, ref iconData))
                throw new Win32Exception(Strings.NotifyIconDeleteFailed);

            _isAdded = false;
        } 
    }

    private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        var message = (WindowMessage) msg;

        if (_TrayEvent == message)
        {
            handled = true;

            switch ((WindowMessage) lParam)
            {
                case WindowMessage.LeftButtonUp:
                    LeftClicked?.Invoke(this, EventArgs.Empty);
                    break;

                case WindowMessage.RightButtonUp:
                    RightClicked?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }
        else if (WindowMessage.Destroy == message) 
            Dispose();

        return IntPtr.Zero;
    }
}