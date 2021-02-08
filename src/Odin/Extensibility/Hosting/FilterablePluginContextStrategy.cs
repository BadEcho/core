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
    /// Provides a strategy that directs a <see cref="PluginContext"/> to cover only the exports filtered against a specific
    /// type identifier.
    /// </summary>
    internal sealed class FilterablePluginContextStrategy : IPluginContextStrategy
    {
        private static readonly ConventionBuilder _FilterableBuilder
            = InitializeConventions();

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
            
            var assemblies 
                = globalConfiguration.LoadFromDirectory(_pluginDirectory).ToList();

            var conventions 
                = this.LoadConventions(globalConfiguration.WithAssemblies(assemblies));

            var matchingPartTypes = FindMatchingPartTypes(assemblies);

            return new ContainerConfiguration()
                   .WithParts(matchingPartTypes)
                   .WithDefaultConventions(conventions)
                   .CreateContainer();
        }

        private static ConventionBuilder InitializeConventions()
        {
            var conventions = new ConventionBuilder();

            conventions.ForTypesDerivedFrom<IFilterable>()
                       .Export<IFilterable>(
                           ex => ex.AddMetadata(nameof(IFilterMetadata.PartType), type => type)
                                   .AddMetadata(nameof(IFilterMetadata.TypeIdentifier),
                                                type => type.GetAttribute<FilterAttribute>()?.TypeIdentifier));
            return conventions;
        }

        private IEnumerable<Type> FindMatchingPartTypes(IEnumerable<Assembly> assemblies)
        {
            var configuration = new ContainerConfiguration()
                .WithAssemblies(assemblies)
                .WithDefaultConventions(_FilterableBuilder);

            using (var container = configuration.CreateContainer())
            {
                var filterableParts = container.GetExports<Lazy<IFilterable, FilterMetadataView>>();

                foreach (var filterablePart in filterableParts)
                {
                    if (null == filterablePart.Metadata.PartType)
                        continue;

                    if (!Guid.TryParse(filterablePart.Metadata.TypeIdentifier, out var typeIdentifier))
                        continue;

                    if (_typeIdentifier.Equals(typeIdentifier))
                        yield return filterablePart.Metadata.PartType;
                }
            }
        }
    }
}
