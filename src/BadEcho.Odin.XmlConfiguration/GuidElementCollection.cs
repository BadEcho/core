//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Configuration;

namespace BadEcho.Odin.XmlConfiguration
{
    /// <summary>
    /// Provides a configuration element containing a collection of <see cref="GuidConfigurationElement"/> child elements.
    /// </summary>
    /// <typeparam name="TElement">The type of configuration element contained by the collection.</typeparam>
    [ConfigurationCollection(typeof(GuidConfigurationElement))]
    internal sealed class GuidElementCollection<TElement> : ConfigurationElementCollection<TElement, Guid>
        where TElement : GuidConfigurationElement, new()
    {
        /// <inheritdoc/>
        protected override Guid GetElementKey(TElement element) 
            => element.Id;
    }
}
