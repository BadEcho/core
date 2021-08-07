//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace BadEcho.Odin.Extensibility.Configuration
{
    /// <summary>
    /// Provides configuration settings for a contract being segmented by one or more call-routable plugins,
    /// suitable for binding against with a generic configuration provider (i.e. Microsoft's IConfiguration binder).
    /// </summary>
    public sealed class ContractConfiguration : IContractConfiguration
    {
        IEnumerable<IRoutablePluginConfiguration> IContractConfiguration.RoutablePlugins
            => RoutablePlugins ?? Enumerable.Empty<IRoutablePluginConfiguration>();

        /// <inheritdoc/>
        public string Name 
        { get; set; } = string.Empty;

        /// <inheritdoc cref="IContractConfiguration.RoutablePlugins"/>
        /// <remarks>
        /// This property exists in order to provide a configuration binder with a concrete type which it will be able
        /// to instantiate (<see cref="RoutablePluginConfiguration"/>), as well as to provide a property that has no
        /// default value assigned to it (which causes issues with some binders) while maintaining the property's
        /// non-nullability contract found on the <see cref="IContractConfiguration"/> interface.
        /// </remarks>
        public IEnumerable<RoutablePluginConfiguration>? RoutablePlugins 
        { get; set; }
    }
}
