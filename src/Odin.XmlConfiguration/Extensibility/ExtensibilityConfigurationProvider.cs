//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Extensibility.Configuration;

namespace BadEcho.Odin.XmlConfiguration.Extensibility
{
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
}