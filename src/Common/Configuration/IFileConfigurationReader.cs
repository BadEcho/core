// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

namespace BadEcho.Configuration;

/// <summary>
/// Defines a reader of configuration data from a file.
/// </summary>
public interface IFileConfigurationReader : IConfigurationReader
{
    /// <summary>
    /// Gets the raw text of the configuration source to parse.
    /// </summary>
    internal string ConfigurationText { get; }
}
