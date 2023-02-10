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

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with the Windows Shell API.
/// </summary>
internal static partial class Shell32
{
    private const string LIBRARY_NAME = "shell32";

    /// <summary>
    /// Sends a message to the taskbar's status area.
    /// </summary>
    /// <param name="dwMessage">A value that specifies the action to be taken by this function.</param>
    /// <param name="lpData">
    /// A <see cref="NotifyIconData"/> instance containing notification area information.
    /// </param>
    /// <returns>True if successful; otherwise, false.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "Shell_NotifyIconW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool Shell_NotifyIcon(NotifyIconMessage dwMessage, ref NotifyIconData lpData);
}
