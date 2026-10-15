// -----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Reflection;
using BadEcho.Interop;
using Xunit;

namespace BadEcho.Tests.Interop;

public class NotifyIconTests
{
    private static readonly WindowMessage _TaskbarCreated
        = User32.RegisterWindowMessage("TaskbarCreated");

    [SkipOnGitHubFact]
    public void Tooltip_AfterShellLosesIcon_IconRestored()
    {
        using (WindowHandle window = TestWindow.Create("Tooltip_AfterShellLosesIcon_IconRestored"))
        {
            using var wrapper = new LocalWindowWrapper(window);
            using var notifyIcon = new NotifyIcon(wrapper, "Before", CreateIconData());

            notifyIcon.Show();

            Assert.True(SendIconMessage(notifyIcon, window, NotifyIconMessage.Delete));

            notifyIcon.Tooltip = "After";

            Assert.True(SendIconMessage(notifyIcon, window, NotifyIconMessage.Modify));
        }
    }

    [SkipOnGitHubFact]
    public void TaskbarCreated_AfterShellLosesIcon_IconRestored()
    {
        using (WindowHandle window = TestWindow.Create("TaskbarCreated_AfterShellLosesIcon_IconRestored"))
        {
            using var wrapper = new LocalWindowWrapper(window);
            using var notifyIcon = new NotifyIcon(wrapper, "Tooltip", CreateIconData());

            notifyIcon.Show();

            Assert.True(SendIconMessage(notifyIcon, window, NotifyIconMessage.Delete));

            User32.SendMessage(window, _TaskbarCreated, IntPtr.Zero, IntPtr.Zero);

            Assert.True(SendIconMessage(notifyIcon, window, NotifyIconMessage.Modify));
        }
    }

    private static bool SendIconMessage(NotifyIcon notifyIcon, WindowHandle window, NotifyIconMessage message)
    {
        FieldInfo idField
            = typeof(NotifyIcon).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MissingFieldException(nameof(NotifyIcon), "_id");

        var id = (Guid)idField.GetValue(notifyIcon)!;
        var iconData = new NotifyIconData(window, id, NotifyIconFlags.Tip)
        {
            Tip = "Probe"
        };

        return Shell32.Shell_NotifyIcon(message, ref iconData);
    }

    private static byte[] CreateIconData()
    {   // A single 16x16, 32 bits-per-pixel image.
        const int size = 16;
        const int headerBytes = 40;
        const int pixelBytes = size * size * 4;
        const int maskBytes = size * 4;
        const int imageBytes = headerBytes + pixelBytes + maskBytes;
        const int imageOffset = 6 + 16;

        // The icon's identifier is derived from its contents, and the shell associates that identifier with the
        // executable's path; so, the color is varied by the test host's path.
        int pathHash = (Environment.ProcessPath ?? string.Empty).GetHashCode(StringComparison.OrdinalIgnoreCase);

        var data = new byte[imageOffset + imageBytes];

        using (var writer = new BinaryWriter(new MemoryStream(data)))
        {   // ICONDIR
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)1);
            // ICONDIRENTRY
            writer.Write((byte)size);
            writer.Write((byte)size);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((ushort)1);
            writer.Write((ushort)32);
            writer.Write((uint)imageBytes);
            writer.Write((uint)imageOffset);
            // BITMAPINFOHEADER (the height includes the AND mask)
            writer.Write(headerBytes);
            writer.Write(size);
            writer.Write(size * 2);
            writer.Write((ushort)1);
            writer.Write((ushort)32);
            writer.Write(0);
            writer.Write(pixelBytes + maskBytes);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            // BGRA pixels; the AND mask that follows is left zeroed.
            for (int i = 0; i < size * size; i++)
            {
                writer.Write((byte)(pathHash | 1));
                writer.Write((byte)(pathHash >> 8));
                writer.Write((byte)(pathHash >> 16));
                writer.Write((byte)0xFF);
            }
        }

        return data;
    }
}
