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

using Microsoft.Extensions.Hosting;

namespace BadEcho.Extensions;

/// <summary>
/// Provides extension methods the aid in matters related to hosting environment information.
/// </summary>
internal static class HostEnvironmentExtensions
{
    /// <summary>
    /// Gets the absolute path to the provided path, creating any directories in the path if they do not exist,
    /// optionally basing the path on a host's configured content root path, if a host is running.
    /// </summary>
    /// <param name="environment">The hosting environment the application is running in, if one exists.</param>
    /// <param name="path">The path (either absolute or relative) to resolve.</param>
    /// <returns>The absolute path to <c>path</c>.</returns>
    public static string ResolvePath(this IHostEnvironment? environment, string path)
    {
        if (!Path.IsPathFullyQualified(path) && environment != null) 
            path = Path.Join(environment.ContentRootPath, path);

        FileInfo info = new FileInfo(path);

        info.Directory?.Create();

        return info.FullName;
    }
}
