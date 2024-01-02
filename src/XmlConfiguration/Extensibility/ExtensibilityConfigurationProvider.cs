//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensibility.Configuration;

namespace BadEcho.XmlConfiguration.Extensibility;

/// <summary>
/// Provides a means to retrieve Extensibility framework configuration sections from classic .NET XML configuration files.
/// </summary>
public static class ExtensibilityConfigurationProvider
{
    /// <summary>
    /// Loads configuration settings for the Bad Echo Extensibility framework.
    /// </summary>
    /// <returns>
    /// An <see cref="IExtensibilityConfiguration"/> object containing settings for the Extensibility framework.
    /// </returns>
    public static IExtensibilityConfiguration LoadConfiguration()
        => ExtensibilitySection.GetSection();
}