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

using System.Runtime.InteropServices;
using System.Windows.Interop;
using BadEcho.Interop;

namespace BadEcho.Presentation.Extensions;

/// <summary>
/// Provides a set of static methods intended to simplify the use of Win32 window sources containing WPF content.
/// </summary>
public static class HwndSourceExtensions
{
    /// <summary>
    /// Gets a non-owning <see cref="SafeHandle"/> instance for this source's window handle.
    /// </summary>
    /// <param name="source">The <see cref="HwndSource"/> to create a non-owning safe handle for.</param>
    /// <returns>A non-owning <see cref="WindowHandle"/> for the window handle of <c>source</c>.</returns>
    public static WindowHandle GetSafeHandle(this HwndSource source)
    {
        Require.NotNull(source, nameof(source));

        return new WindowHandle(source.Handle, false);
    }
}
