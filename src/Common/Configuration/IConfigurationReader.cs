//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Configuration;

/// <summary>
/// Defines a reader of configuration data.
/// </summary>
public interface IConfigurationReader
{
    /// <summary>
    /// Gets the raw text of the configuration source to parse.
    /// </summary>
    internal string ConfigurationText { get; }

    /// <summary>
    /// Gets the root of the configuration as an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to parse the configuration as.</typeparam>
    /// <returns>A <typeparamref name="T"/> instance reflecting the configuration.</returns>
    T GetConfiguration<T>() where T : new();

    /// <summary>
    /// Gets a specified section from the configuration as an instance of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to parse the configuration section as.</typeparam>
    /// <param name="sectionName">The name of the section to parse.</param>
    /// <returns>
    /// A <typeparamref name="T"/> instance reflecting the section named <c>sectionName</c> found in the
    /// configuration.
    /// </returns>
    T GetConfiguration<T>(string sectionName) where T : new();
}