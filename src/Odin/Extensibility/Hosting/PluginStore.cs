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

using System.Collections.Concurrent;
using System.Reflection;
using BadEcho.Odin.Collections;
using BadEcho.Odin.Extensibility.Configuration;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides the primary container of the various contexts that make up Odin's Extensibility framework.
    /// </summary>
    internal sealed class PluginStore : IDisposable
    {
        private const LazyThreadSafetyMode LAZY_MODE = LazyThreadSafetyMode.ExecutionAndPublication;

        private readonly ConcurrentDictionary<Type, IHostAdapter> _hostAdapters
            = new();

        private readonly IExtensibilityConfiguration _configuration;

        private readonly ConcurrentDictionary<Assembly, PluginContext> _localContexts 
            = new();

        private readonly Lazy<PluginContext> _globalContext;
        private readonly Lazy<LazyConcurrentDictionary<Guid, PluginContext>> _filterableContexts;
        private readonly Lazy<IReadOnlyDictionary<Guid, IFilterableFamilyMetadata>> _filterableFamilies;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginStore"/> class.
        /// </summary>
        /// <param name="configuration">
        /// Configuration for the Extensibility framework, used to determine where plugins are loaded into the store from,
        /// among other things.
        /// </param>
        public PluginStore(IExtensibilityConfiguration configuration)
        {
            Require.NotNull(configuration, nameof(configuration));

            _configuration = configuration;

            _globalContext = new Lazy<PluginContext>(
                () => new PluginContext(new GlobalPluginContextStrategy(configuration.GetFullPathToPlugins())),
                LAZY_MODE);

            _filterableFamilies = new Lazy<IReadOnlyDictionary<Guid, IFilterableFamilyMetadata>>(
                InitializeFilterableFamilies, LAZY_MODE);

            _filterableContexts = new Lazy<LazyConcurrentDictionary<Guid, PluginContext>>(
                InitializeFilterableContexts, LAZY_MODE);
        }

        /// <summary>
        /// Gets the default plugin context, used to service requests for all discoverable plugins.
        /// </summary>
        /// <remarks>
        /// This context has a global reach, which means it will import all implementations of any requested contract
        /// across all discovered plugins.
        /// </remarks>
        public PluginContext GlobalContext
            => _globalContext.Value;

        /// <summary>
        /// Gets a dictionary containing filterable family identifiers paired with plugin contexts used to service requests
        /// for all plugins that fall within their respective filterable family. 
        /// </summary>
        /// <remarks>
        /// The contexts found in this dictionary are initialized in such a way so that they will not import any pluggable part not
        /// belonging to their filterable family. Contexts are not initialized until an attempt to access them is made through this
        /// dictionary.
        /// </remarks>
        public IReadOnlyDictionary<Guid, PluginContext> FilterableContexts
            => _filterableContexts.Value;

        /// <summary>
        /// Gets a dictionary containing filterable family identifiers paired with metadata describing the family.
        /// </summary>
        public IReadOnlyDictionary<Guid, IFilterableFamilyMetadata> FilterableFamilies
            => _filterableFamilies.Value;

        /// <summary>
        /// Retrieves a host adapter for the specified segmented contract type, initializing a new host adapter instance if one does
        /// not already exist.
        /// </summary>
        /// <typeparam name="T">The type of contract to segment through the returned adapter.</typeparam>
        /// <returns>
        /// A <see cref="HostAdapter{T}"/> for the specified contract type. This will either be an existing host adapter for the
        /// contract type, or a newly initialized instance if one does not already exist.
        /// </returns>
        /// <remarks>This is an entry point into Odin's Extensibility framework's call-routable plugin system.</remarks>
        public HostAdapter<T> LoadAdapter<T>()
            where T : notnull
        {
            IHostAdapter hostAdapter = _hostAdapters.GetOrAdd(
                typeof(T),
                _ => new HostAdapter<T>(GlobalContext, _configuration));

            return (HostAdapter<T>) hostAdapter;
        }

        /// <summary>
        /// Retrieves a plugin context used to service requests for plugins discoverable within the provided assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly that is to act as the local context from which pluggable parts are loaded.
        /// </param>
        /// <returns></returns>
        public PluginContext LoadContext(Assembly assembly) 
            => _localContexts.GetOrAdd(assembly, x => new PluginContext(new LocalPluginContextStrategy(x)));

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_globalContext.IsValueCreated)
                GlobalContext.Dispose();

            if (_filterableContexts.IsValueCreated)
            {
                foreach (var idContextPair in _filterableContexts.Value)
                {
                    var lazyContext = idContextPair.Value;

                    if (!lazyContext.IsValueCreated)
                        continue;

                    lazyContext.Value.Dispose();
                }
            }
        }
        
        private Dictionary<Guid, IFilterableFamilyMetadata> InitializeFilterableFamilies()
        {
            return GlobalContext.Load<IFilterableFamily, FilterableFamilyMetadataView>()
                                .Select(f => f.Metadata)
                                .ToDictionary<IFilterableFamilyMetadata, Guid>(kv => kv.FamilyId);
        }

        private LazyConcurrentDictionary<Guid, PluginContext> InitializeFilterableContexts()
        {
            var filterableContexts
                = new LazyConcurrentDictionary<Guid, PluginContext>(LAZY_MODE);

            IEnumerable<Guid> familyIds = GlobalContext.Load<IFilterableFamily, FilterableFamilyMetadataView>()
                                                       .Select(f => f.Metadata.FamilyId);
            foreach (var familyId in familyIds)
            {
                var filterableStrategy = new FilterablePluginContextStrategy(_configuration.GetFullPathToPlugins(), familyId);

                filterableContexts.GetOrAdd(familyId, () => new PluginContext(filterableStrategy));
            }

            return filterableContexts;
        }
    }
}