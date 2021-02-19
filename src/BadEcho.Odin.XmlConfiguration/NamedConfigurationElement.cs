//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;

namespace BadEcho.Odin.XmlConfiguration
{
    /// <summary>
    /// Provides a <see cref="ConfigurationElement"/> whose name is used as its key.
    /// </summary>
    internal class NamedConfigurationElement : ConfigurationElement
    {
        private const string NAME_ATTRIBUTE_SCHEMA = "name";

        /// <summary>
        /// Gets or sets the identifying name for this configuration element.
        /// </summary>
        /// <remarks>
        /// Configuration attributes decorate this property to add support for use of the declarative coding model
        /// by derivations and/or consumers of this class.
        /// </remarks>
        [ConfigurationProperty(NAME_ATTRIBUTE_SCHEMA, IsKey = true, IsRequired = true)]
        public string Name
        {
            get => (string) base[NAME_ATTRIBUTE_SCHEMA];
            set => base[NAME_ATTRIBUTE_SCHEMA] = value;
        }

        /// <summary>
        /// Creates a representative configuration object for the <see cref="Name"/> configuration property.
        /// </summary>
        /// <returns>A <see cref="ConfigurationProperty"/> object for the <see cref="Name"/> configuration property.</returns>
        /// <remarks>
        /// This method of creating the property for <see cref="Name"/> should be used when adhering to the programmatic
        /// coding model.
        /// </remarks>
        protected static ConfigurationProperty CreateNameProperty()
            => new(NAME_ATTRIBUTE_SCHEMA,
                   typeof(string),
                   string.Empty,
                   null,
                   null,
                   ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
    }
}
