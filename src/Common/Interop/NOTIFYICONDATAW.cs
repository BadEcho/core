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

using BadEcho.Extensions;
using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Represents information that the system needs to display notifications in the notification area.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal unsafe struct NOTIFYICONDATAW
{
    /// <summary>
    /// The size of this structure in bytes.
    /// </summary>
    public uint cbSize;
    /// <summary>
    /// A handle to the window that receives notifications associated with an icon in the notification area.
    /// </summary>
    public IntPtr hWnd;
    /// <summary>
    /// The application-defined identifier of the taskbar icon.
    /// </summary>
    public uint uID;
    /// <summary>
    /// Flags that either indicate which of the other members of the structure contain valid data or provide
    /// additional information to the tooltip as to how it should display.
    /// </summary>
    public NotifyIconFlags uFlags;
    /// <summary>
    /// An application-defined message identifier.
    /// </summary>
    public uint uCallbackMessage;
    /// <summary>
    /// A handle to the icon to be added, modified, or deleted.
    /// </summary>
    public IntPtr hIcon;
    /// <summary>
    /// The state of the icon.
    /// </summary>
    public uint dwState;
    /// <summary>
    /// A value that specifies which bits of the <see cref="dwState"/> member are retrieved or modified.
    /// </summary>
    public uint dwStateMask;
    /// <summary>
    /// Either the timeout value, in milliseconds, for the notification, or a specification of which version of the Shell
    /// notification icon interface should be used.
    /// </summary>
    public uint uTimeoutOrVersion;
    /// <summary>
    /// Flags that can be set to modify the behavior and appearance of a balloon notification.
    /// </summary>
    public NotifyIconInfoFlags dwInfoFlags;
    /// <summary>
    /// A handle to a customized notification icon that should be used independently of the notification area icon.
    /// </summary>
    public IntPtr hBalloonIcon;

    private fixed char _szTip[128];
    private fixed char _szInfo[256];
    private fixed char _szInfoTitle[64];

    /// <summary>
    /// Gets or sets a string that specifies the text for a standard tooltip.
    /// </summary>
    public ReadOnlySpan<char> Tip
    {
        get => SzTip.SliceAtFirstNull();
        set => value.CopyToAndTerminate(SzTip);
    }

    /// <summary>
    /// Gets or sets a string that specifies the text to display in a balloon notification.
    /// </summary>
    public ReadOnlySpan<char> Info
    {
        get => SzInfo.SliceAtFirstNull();
        set => value.CopyToAndTerminate(SzInfo);
    }

    /// <summary>
    /// Gets or sets a string that specifies a title for a balloon notification.
    /// </summary>
    public ReadOnlySpan<char> InfoTitle
    {
        get => SzInfoTitle.SliceAtFirstNull();
        set => value.CopyToAndTerminate(SzInfoTitle);
    }

    /// <summary>
    /// Gets a null-terminated string that specifies the text for a standard tooltip.
    /// </summary>
    private Span<char> SzTip
    {
        get
        {
            fixed (char* c = _szTip)
            {
                return new Span<char>(c, 128);
            }
        }
    }

    /// <summary>
    /// Gets a null-terminated string that specifies the text to display in a balloon notification.
    /// </summary>
    private Span<char> SzInfo
    {
        get
        {
            fixed (char* c = _szInfo)
            {
                return new Span<char>(c, 256);
            }
        }
    }

    /// <summary>
    /// Gets a null-terminated string that specifies a title for a balloon notification.
    /// </summary>
    private Span<char> SzInfoTitle
    {
        get
        {
            fixed (char* c = _szInfoTitle)
            {
                return new Span<char>(c, 64);
            }
        }
    }
}
