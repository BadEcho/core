//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace BadEcho.Odin.Extensibility.Configuration
{
    /// <summary>
    /// Provides configuration settings for a call-routable plugin suitable for binding against with a
    /// generic configuration provider (i.e. Microsoft's IConfiguration binder).
    /// </summary>
    public sealed class RoutablePluginConfiguration : IRoutablePluginConfiguration
    {
        IEnumerable<string> IRoutablePluginConfiguration.MethodClaims
            => MethodClaims ?? Enumerable.Empty<string>();

        /// <inheritdoc/>
        public Guid Id 
        { get; set; }

        /// <inheritdoc/>
        public bool Primary 
        { get; set; }

        /// <inheritdoc cref="IRoutablePluginConfiguration.MethodClaims"/>
        /// <remarks>
        /// This property exists in order to provide a configuration binder with a property that has no default value
        /// assigned to it (which causes issues with some binders) while maintaining the property's non-nullability
        /// contract found on the <see cref="IRoutablePluginConfiguration"/> interface.
        /// </remarks>
        public IEnumerable<string>? MethodClaims 
        { get; set; }
    }
}
