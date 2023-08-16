//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides a custom marshaller for notification area information.
/// </summary>
[CustomMarshaller(typeof(NotifyIconData), MarshalMode.ManagedToUnmanagedRef, typeof(ManagedToUnmanagedRef))]
internal static unsafe class NotifyIconDataMarshaller
{
    /// <summary>
    /// Represents a stateful marshaller for notification area information.
    /// </summary>
    public ref struct ManagedToUnmanagedRef
    {
        private NOTIFYICONDATAW _unmanaged;
        private WindowHandle _windowHandle;
        private IconHandle? _iconHandle;
        private IconHandle? _balloonIconHandle;
        private bool _windowHandleAddRefd;
        private bool _iconHandleAddRefd;
        private bool _balloonIconHandleAddRefd;
        private IntPtr _originalWindowHandleValue;
        private IntPtr _originalIconHandleValue;
        private IntPtr _originalBalloonIconHandleValue;

        /// <summary>
        /// Converts a managed <see cref="NotifyIconData"/> instance to its unmanaged counterpart,
        /// loading the result into the marshaller.
        /// </summary>
        /// <param name="iconData">A managed instance of notification area information.</param>
        public void FromManaged(NotifyIconData iconData)
        {
            Require.NotNull(iconData, nameof(iconData));

            _windowHandleAddRefd = false;
            _iconHandleAddRefd = false;
            _balloonIconHandleAddRefd = false;

            _windowHandle = iconData.Window;
            _iconHandle = iconData.Icon;
            _balloonIconHandle = iconData.BalloonIcon;

            _unmanaged.cbSize = (uint) sizeof(NOTIFYICONDATAW);
            _windowHandle.DangerousAddRef(ref _windowHandleAddRefd);
            _unmanaged.hWnd = _originalWindowHandleValue = _windowHandle.DangerousGetHandle();
            _unmanaged.guidItem = GuidMarshaller.ConvertToUnmanaged(iconData.Id);
            _unmanaged.uFlags = iconData.Flags;
            _unmanaged.uCallbackMessage = iconData.CallbackMessage;

            if (_iconHandle != null)
            {
                _iconHandle.DangerousAddRef(ref _iconHandleAddRefd);
                _unmanaged.hIcon = _originalIconHandleValue = _iconHandle.DangerousGetHandle();
            }

            _unmanaged.dwState = iconData.State;
            _unmanaged.dwStateMask = iconData.StateMask;
            _unmanaged.uTimeoutOrVersion = iconData.TimeoutOrVersion;
            _unmanaged.dwInfoFlags = iconData.InfoFlags;

            if (_balloonIconHandle != null)
            {
                _balloonIconHandle.DangerousAddRef(ref _balloonIconHandleAddRefd);
                _unmanaged.hBalloonIcon = _originalBalloonIconHandleValue = _balloonIconHandle.DangerousGetHandle();
            }

            _unmanaged.Tip = iconData.Tip;
            _unmanaged.Info = iconData.Info;
            _unmanaged.InfoTitle = iconData.InfoTitle;
        }

        /// <summary>
        /// Provides the unmanaged notification area information currently loaded into the marshaller.
        /// </summary>
        /// <returns>The converted <see cref="NOTIFYICONDATAW"/> value.</returns>
        public readonly NOTIFYICONDATAW ToUnmanaged()
            => _unmanaged;

        /// <summary>
        /// Loads the provided unmanaged notification area information into the marshaller.
        /// </summary>
        /// <param name="unmanaged">The unmanaged instance of notification area information.</param>
        public void FromUnmanaged(NOTIFYICONDATAW unmanaged)
            => _unmanaged = unmanaged;

        /// <summary>
        /// Converts the unmanaged <see cref="NOTIFYICONDATAW"/> instance currently loaded into the marshaller
        /// into its managed counterpart, returning the result.
        /// </summary>
        /// <returns>The converted <see cref="NotifyIconData"/> instance.</returns>
        /// <exception cref="NotSupportedException">A handle originating from a <see cref="SafeHandle"/> was changed.</exception>
        public readonly NotifyIconData ToManaged()
        {
            // SafeHandle fields must match the underlying handle value during marshalling. They cannot change.
            if (_unmanaged.hWnd != _originalWindowHandleValue)
                throw new NotSupportedException(Strings.HandleCannotChangeDuringMarshalling);

            if (_unmanaged.hIcon != _originalIconHandleValue)
                throw new NotSupportedException(Strings.HandleCannotChangeDuringMarshalling);

            if (_unmanaged.hBalloonIcon != _originalBalloonIconHandleValue)
                throw new NotSupportedException(Strings.HandleCannotChangeDuringMarshalling);

            Guid managedId = GuidMarshaller.ConvertToManaged(_unmanaged.guidItem);
            
            return new NotifyIconData(_windowHandle, managedId, _unmanaged.uFlags)
                   {
                       CallbackMessage = _unmanaged.uCallbackMessage,
                       Icon = _iconHandle,
                       State = _unmanaged.dwState,
                       StateMask = _unmanaged.dwStateMask,
                       TimeoutOrVersion = _unmanaged.uTimeoutOrVersion,
                       InfoFlags = _unmanaged.dwInfoFlags,
                       BalloonIcon = _balloonIconHandle,
                       Tip = new string(_unmanaged.Tip),
                       Info = new string(_unmanaged.Info),
                       InfoTitle = new string(_unmanaged.InfoTitle)
                   };
        }

        /// <summary>
        /// Releases all resources in use by the marshaller.
        /// </summary>
        public readonly void Free()
        {
            if (_windowHandleAddRefd)
                _windowHandle.DangerousRelease();

            if (_iconHandle != null && _iconHandleAddRefd)
                _iconHandle.DangerousRelease();

            if (_balloonIconHandle != null && _balloonIconHandleAddRefd)
                _balloonIconHandle.DangerousRelease();
        }

        /// <summary>
        /// Represents information that the system needs to display notifications in the notification area.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct NOTIFYICONDATAW
        {
            /// <summary>
            /// The size of this structure in bytes.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// A handle to the window that receives notifications associated with an icon in
            /// the notification area.
            /// </summary>
            public IntPtr hWnd;
            /// <summary>
            /// The application-defined identifier of the taskbar icon.
            /// </summary>
            public uint uID;
            /// <summary>
            /// Flags that either indicate which of the other members of the structure contain valid data
            /// or provide additional information to the tooltip as to how it should display.
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
            /// A null-terminated string that specifies the text for a standard tooltip.
            /// </summary>
            public fixed char szTip[128];
            /// <summary>
            /// The state of the icon.
            /// </summary>
            public uint dwState;
            /// <summary>
            /// A value that specifies which bits of the <see cref="dwState"/> member are retrieved
            /// or modified.
            /// </summary>
            public uint dwStateMask;
            /// <summary>
            /// A null-terminated string that specifies the text to display in a balloon notification.
            /// </summary>
            public fixed char szInfo[256];
            /// <summary>
            /// Either the timeout value, in milliseconds, for the notification, or a specification of which
            /// version of the Shell notification icon interface should be used.
            /// </summary>
            public uint uTimeoutOrVersion;
            /// <summary>
            /// A null-terminated string that specifies the title for a balloon notification.
            /// </summary>
            public fixed char szInfoTitle[64];
            /// <summary>
            /// Flags that can be set to modify the behavior and appearance of a balloon notification.
            /// </summary>
            public NotifyIconInfoFlags dwInfoFlags;
            /// <summary>
            /// A registered <see cref="GUID"/> that identifies the icon. This is the preferred way to identify
            /// an icon in modern OS versions.
            /// </summary>
            public GUID guidItem;
            /// <summary>
            /// A handle to a customized notification icon that should be used independently of the
            /// notification area icon.
            /// </summary>
            public IntPtr hBalloonIcon;

            /// <summary>
            /// Gets or sets a string that specifies the text for a standard tooltip.
            /// </summary>
            public ReadOnlySpan<char> Tip
            {
                readonly get => SzTip.SliceAtFirstNull();
                set => value.CopyToAndTerminate(SzTip);
            }

            /// <summary>
            /// Gets or sets a string that specifies the text to display in a balloon notification.
            /// </summary>
            public ReadOnlySpan<char> Info
            {
                readonly get => SzInfo.SliceAtFirstNull();
                set => value.CopyToAndTerminate(SzInfo);
            }

            /// <summary>
            /// Gets or sets a string that specifies a title for a balloon notification.
            /// </summary>
            public ReadOnlySpan<char> InfoTitle
            {
                readonly get => SzInfoTitle.SliceAtFirstNull();
                set => value.CopyToAndTerminate(SzInfoTitle);
            }

            /// <summary>
            /// Gets a null-terminated string that specifies the text for a standard tooltip.
            /// </summary>
            private readonly Span<char> SzTip
            {
                get
                {
                    fixed (char* c = szTip)
                    {
                        return new Span<char>(c, 128);
                    }
                }
            }

            /// <summary>
            /// Gets a null-terminated string that specifies the text to display in a balloon notification.
            /// </summary>
            private readonly Span<char> SzInfo
            {
                get
                {
                    fixed (char* c = szInfo)
                    {
                        return new Span<char>(c, 256);
                    }
                }
            }

            /// <summary>
            /// Gets a null-terminated string that specifies a title for a balloon notification.
            /// </summary>
            private readonly Span<char> SzInfoTitle
            {
                get
                {
                    fixed (char* c = szInfoTitle)
                    {
                        return new Span<char>(c, 64);
                    }
                }
            }
        }
    }
}
