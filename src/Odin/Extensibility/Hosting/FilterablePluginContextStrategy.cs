//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Linq;
using System.Reflection;
using BadEcho.Odin.Extensions;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a strategy that directs a <see cref="PluginContext"/> to make available only the exports filtered against a specific
    /// type identifier.
    /// </summary>
    internal sealed class FilterablePluginContextStrategy : IPluginContextStrategy
    {
        private readonly string _pluginDirectory;
        private readonly Guid _typeIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterablePluginContextStrategy"/> class.
        /// </summary>
        /// <param name="pluginDirectory">Full path to the directory where plugins will be loaded from.</param>
        /// <param name="typeIdentifier">The identity of the type of plugin to allow through the filter.</param>
        public FilterablePluginContextStrategy(string pluginDirectory, Guid typeIdentifier)
        {
            _pluginDirectory = pluginDirectory;
            _typeIdentifier = typeIdentifier;
        }

        /// <inheritdoc/>
        public CompositionHost CreateContainer()
        {
            var globalConfiguration = new ContainerConfiguration();

            IEnumerable<Assembly> assemblies
                = globalConfiguration.LoadFromDirectory(_pluginDirectory);

            ConventionBuilder conventions
                = this.LoadConventions(globalConfiguration.WithAssemblies(assemblies));

            globalConfiguration.WithDefaultConventions(conventions);

            IEnumerable<Type> matchingPartTypes = FindMatchingPartTypes(globalConfiguration);
            
            return new ContainerConfiguration()
                   .WithParts(matchingPartTypes)
                   .WithDefaultConventions(conventions)
                   .CreateContainer();
        }

        private IEnumerable<Type> FindMatchingPartTypes(ContainerConfiguration globalConfiguration)
        {
            using (var globalContainer = globalConfiguration.CreateContainer())
            {
                var filterableParts = globalContainer.GetExports<Lazy<IFilterable, FilterMetadataView>>();
                
                return filterableParts
                       .Where(filterablePart =>
                                  _typeIdentifier.Equals(filterablePart.Metadata.TypeIdentifier))
                       .Select(matchingPart => matchingPart.Metadata.PartType)
                       .WhereNotNull();
            }
        }
    }
}
