// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace BadEcho.Extensions;

/// <summary>
/// Provides synchronization locks for configuration files.
/// </summary>
internal static class FileLocks
{
    private static readonly ConcurrentDictionary<string, Lock> _Locks
        = new(OperatingSystem.IsWindows() || OperatingSystem.IsMacOS()
                  ? StringComparer.OrdinalIgnoreCase
                  : StringComparer.Ordinal);

    /// <summary>
    /// Gets the lock guarding the specified file.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>A <see cref="Lock"/> guarding <c>path</c>.</returns>
    public static Lock Get(string path)
        => _Locks.GetOrAdd(path, _ => new Lock());
}
