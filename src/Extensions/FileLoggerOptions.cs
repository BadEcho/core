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

namespace BadEcho.Extensions;

/// <summary>
/// Provides options for a <see cref="FileLogger"/>.
/// </summary>
public sealed class FileLoggerOptions
{
    /// <summary>
    /// Gets or sets the path to the log file.
    /// </summary>
    public string Path
    { get; set; } = string.Empty;
}
