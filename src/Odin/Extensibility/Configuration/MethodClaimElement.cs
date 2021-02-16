//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Configuration;
using System.Threading;
using BadEcho.Odin.Configuration;

namespace BadEcho.Odin.Extensibility.Configuration
{
    /// <summary>
    /// Provides a configuration element that expresses a claim of ownership over a segmented contract's
    /// method.
    /// </summary>
    public sealed class MethodClaimElement : NamedConfigurationElement
    {
        private static readonly Lazy<ConfigurationPropertyCollection> _Properties
            = new(InitializeProperties, LazyThreadSafetyMode.PublicationOnly);

        /// <inheritdoc/>
        protected override ConfigurationPropertyCollection Properties
            => _Properties.Value;

        private static ConfigurationPropertyCollection InitializeProperties()
            => new()
               {
                   CreateNameProperty()
               };
    }
}
