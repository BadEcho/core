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
    /// Provides a collection of <see cref="NamedConfigurationElement"/> objects.
    /// </summary>
    /// <typeparam name="TElement">The type of configuration element contained by the collection.</typeparam>
    [ConfigurationCollection(typeof(NamedConfigurationElement))]
    internal sealed class NamedElementCollection<TElement> : ConfigurationElementCollection<TElement, string>
        where TElement : NamedConfigurationElement, new()
    {
        /// <inheritdoc/>
        protected override string GetElementKey(TElement element) 
            => element.Name;
    }
}
