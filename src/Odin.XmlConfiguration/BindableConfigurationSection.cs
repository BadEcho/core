//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;
using System.Xml.Serialization;

namespace BadEcho.Odin.XmlConfiguration
{
    /// <summary>
    /// Provides a custom configuration section that supports being bound to an XML namespace.
    /// </summary>
    internal class BindableConfigurationSection : ConfigurationSection
    {
        private const string XML_NAMESPACE_ATTRIBUTE_SCHEMA = "xmlns";

        /// <summary>
        /// Gets or sets the namespace binding for this section.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property exists to prevent <see cref="ConfigurationManager"/> from taking any issue to this section
        /// being bound to a particular XML namespace. As a side note, the spelling of this property is based on precedent
        /// established by good ol' Microsoft (send all complaints to <see cref="XmlAttributes.Xmlns"/>).
        /// </para>
        /// <para>
        /// Configuration attributes decorate this property to add support for use of the declarative coding model
        /// by derivations and/or consumers of this class.
        /// </para>
        /// </remarks>
        [ConfigurationProperty(XML_NAMESPACE_ATTRIBUTE_SCHEMA)]
        public string Xmlns
        {
            get => (string) base[XML_NAMESPACE_ATTRIBUTE_SCHEMA];
            set => base[XML_NAMESPACE_ATTRIBUTE_SCHEMA] = value;
        }

        /// <summary>
        /// Creates a representative configuration object for the <see cref="Xmlns"/> configuration property.
        /// </summary>
        /// <returns>A <see cref="ConfigurationProperty"/> object for the <see cref="Xmlns"/> configuration property.</returns>
        /// <remarks>
        /// This method of creating the property for <see cref="Xmlns"/> should be used when adhering to the programmatic
        /// coding model.
        /// </remarks>
        protected static ConfigurationProperty CreateXmlnsProperty()
            => new(XML_NAMESPACE_ATTRIBUTE_SCHEMA,
                   typeof(string),
                   string.Empty,
                   null,
                   null,
                   ConfigurationPropertyOptions.None);
    }
}