//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Extensibility.Configuration;

namespace BadEcho.Odin.XmlConfiguration.Extensibility;

/// <summary>
/// Provides a means to retrieve Extensibility framework configuration sections from classic .NET XML configuration files.
/// </summary>
public static class ExtensibilityConfigurationProvider
{
    /// <summary>
    /// Loads configuration settings for Odin's Extensibility framework.
    /// </summary>
    /// <returns>
    /// An <see cref="IExtensibilityConfiguration"/> object containing settings for the Extensibility framework.
    /// </returns>
    public static IExtensibilityConfiguration LoadConfiguration()
    {
        ExtensibilitySection section = ExtensibilitySection.GetSection();

        if (section == null)
            throw new ConfigurationMissingException(MissingConfigurationType.Section, ExtensibilitySection.Schema);

        return section;
    }
}