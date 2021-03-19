//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BadEcho.Odin.Extensibility.Configuration;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Logging;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a platform and host for consuming plugins supported by Odin's Extensibility framework.
    /// </summary>
    public static class PluginHost
    {
        private static readonly object _StoreLock = new();

        private static readonly MalleableLazy<PluginStore> _Store
            = new(InitializeStore, LazyThreadSafetyMode.ExecutionAndPublication);

        private static IExtensibilityConfiguration _Configuration
            = new ExtensibilityConfiguration();

        /// <summary>
        /// Gets the current plugin store in use by the host.
        /// </summary>
        private static PluginStore Store
            => _Store.Value;

        /// <summary>
        /// Arms the host with the provided dependency value so that it can be injected into plugin-provided exports that require it
        /// and fulfill the specified generic contract type.
        /// </summary>
        /// <typeparam name="TContract">The contract type whose exports should be loaded.</typeparam>
        /// <typeparam name="TDependency">
        /// The type of object depended upon by plugin-provided exports fulfilling the specified generic contract type.
        /// </typeparam>
        /// <param name="value">The dependency value to arm the host with.</param>
        /// <returns>
        /// A collection of exported <typeparamref name="TContract"/> values injected with any required
        /// <typeparamref name="TDependency"/> value.
        /// </returns>
        public static IEnumerable<TContract> ArmedLoad<TContract, TDependency>(TDependency value)
        {
            Require.NotNull(value, nameof(value));
            PluginContext context = Store.GlobalContext;

            return DependencyRegistry<TDependency>
                .ExecuteWhileArmed(value, context.Load<TContract>);
        }

        /// <summary>
        /// Retrieves globally available plugin-provided exports fulfilling the specified generic contract type.
        /// </summary>
        /// <typeparam name="TContract">The contract type whose exports should be loaded.</typeparam>
        /// <returns>A collection of exported <typeparamref name="TContract"/> values.</returns>
        /// <remarks>
        /// This method will load plugin-provided exports under a global context, which means all discovered plugins
        /// will have their exports included in the results.
        /// </remarks>
        public static IEnumerable<TContract> Load<TContract>() 
            => Store.GlobalContext.Load<TContract>();

        /// <summary>
        /// Retrieves the plugin-provided export fulfilling the specified generic contract type that belongs to the specified
        /// filterable family.
        /// </summary>
        /// <typeparam name="TContract">The contract type whose export should be loaded.</typeparam>
        /// <param name="familyId">Identifies the filterable family that the plugin-provided export must belong to.</param>
        /// <returns>
        /// An exported <typeparamref name="TContract"/> filtered value belonging to the filterable family identified by
        /// <c>familyId</c>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The traditional goal of assigning contract types to filterable families is so that we can isolate individual parts
        /// bearing an association with their assigned family for each one of these particular contracts. When operating under a
        /// global context, we must always assume there may be more than one implementation of a particular contract type; when under
        /// a filterable context, we gain the convenience of expecting to only have a single instance returned.
        /// </para>
        /// <para>
        /// It is not considered an error if more than one contract export is found among filtered results, as we can still satisfy
        /// the inherent requirements of this particular method's signature (which asks for a single <typeparamref name="TContract"/>
        /// to be returned); however, it is still considered to be anomalous plugin design, as having multiple implementations of a
        /// contract type for a family beggars the question as to whether or not enough filterable families themselves are defined.
        /// Perhaps an additional family needs to be made to assign this errant additional part to.
        /// </para>
        /// </remarks>
        public static TContract Load<TContract>(Guid familyId)
        {
            if (!IsFilterable(familyId))
                throw new ArgumentException(Strings.FamilyIdNotRegistered.InvariantFormat(familyId), nameof(familyId));

            var parts = Store.FilterableContexts[familyId].Load<TContract>()
                             .ToList();

            if (parts.IsEmpty())
            {
                throw new ArgumentException(
                    Strings.NoExportFoundForFamily
                           .InvariantFormat(typeof(TContract), Store.FilterableFamilies[familyId].Name),
                    nameof(familyId));
            }

            if (!parts.IsSingle())
            {
                Logger.Warning(
                    Strings.MultipleExportsFoundForFamily
                           .InvariantFormat(typeof(TContract), Store.FilterableFamilies[familyId].Name));
            }

            return parts[0];
        }

        /// <summary>
        /// Determines if the functionality provided by the specified generic contract is available in the current operating
        /// environment.
        /// </summary>
        /// <typeparam name="TContract">The contract type to check for available exports.</typeparam>
        /// <returns>True if <typeparamref name="TContract"/> has exports provided; otherwise, false.</returns>
        /// <remarks>
        /// This method allows you to check for the existence of exports for <typeparamref name="TContract"/> while avoiding the
        /// instantiation of any exported parts that are found.
        /// </remarks>
        public static bool IsSupported<TContract>()
        {
            var lazyParts = Store.GlobalContext.Load<Lazy<TContract>>();

            return lazyParts.Any();
        }

        /// <summary>
        /// Retrieves a globally available plugin-provided export fulfilling the specified generic contract that is required by
        /// the caller in order to ensure its proper operation, with the expectation that the contract implementation can only
        /// be considered valid if there is one and only one export available.
        /// </summary>
        /// <typeparam name="TContract">The contract type whose export should be loaded.</typeparam>
        /// <returns>The sole, exported <typeparamref name="TContract"/> value.</returns>
        /// <remarks>
        /// <para>
        /// This method should only be used for contracts that have been designed such that their consumers depend on them for
        /// correct operation, whether at a system-wide or feature level, and that only a single provider should ever exist in
        /// a given operating environment. It is not intended to be used by a centralized host responsible for consuming numerous
        /// external plugins, but rather by a one-off endpoint to load contracts that are specific to that endpoint.
        /// </para>
        /// <para>
        /// An example usage for this method would be from within a plugin, targeting an extension to the plugin itself that provides
        /// a type of service that is specific to the context in which the plugin is loaded, such as a desktop user interface
        /// communication contract for interoperability with a desktop application.
        /// </para>
        /// <para>
        /// If the capabilities provided by <typeparamref name="TContract"/> is such that its availability is optional to the overall
        /// operation, support for these capabilities can be determined ahead of time by querying <see cref="IsSupported{TContract}"/>
        /// prior to calling this method.
        /// </para>
        /// </remarks>
        public static TContract LoadRequirement<TContract>()
        {
            var parts = Store.GlobalContext.Load<TContract>();
            TContract? uniquePart = default;

            foreach (var part in parts)
            {
                if (uniquePart != null)
                {
                    throw new InvalidOperationException(
                        Strings.MultipleExportsFoundForRequiredContract.InvariantFormat(typeof(TContract)));
                }

                uniquePart = part;
            }

            if (uniquePart == null)
            {
                throw new InvalidOperationException(
                    Strings.NoExportFoundForRequiredContract.InvariantFormat(typeof(TContract)));
            }

            return uniquePart;
        }

        /// <summary>
        /// Retrieves a host adapter for the specified segmented contract.
        /// </summary>
        /// <typeparam name="TContract">The type of contract to segment through the returned adapter.</typeparam>
        /// <returns>A <see cref="HostAdapter{TContract}"/> for the specified contract type.</returns>
        public static HostAdapter<TContract> LoadAdapter<TContract>()
            where TContract : notnull
        {
            return Store.LoadAdapter<TContract>();
        }

        /// <summary>
        /// Injects globally available exports into the provided attributed pluggable part.
        /// </summary>
        /// <param name="pluggablePart">An object containing loose import attributions.</param>
        public static void Inject(object pluggablePart)
        {
            Require.NotNull(pluggablePart, nameof(pluggablePart));

            Store.GlobalContext.Inject(pluggablePart);
        }

        /// <summary>
        /// Injects exports belonging to the specified filterable family into the provided attributed pluggable part.
        /// </summary>
        /// <param name="pluggablePart">An object containing loose import attributions.</param>
        /// <param name="familyId">Identifies the filterable family that the injected exports must belong to.</param>
        public static void Inject(object pluggablePart, Guid familyId)
        {
            Require.NotNull(pluggablePart, nameof(pluggablePart));

            if (!IsFilterable(familyId))
                throw new ArgumentException(Strings.FamilyIdNotRegistered.InvariantFormat(familyId), nameof(familyId));

            Store.FilterableContexts[familyId].Inject(pluggablePart);
        }

        /// <summary>
        /// Injects globally available exports into the provided attributed pluggable part within a context where the
        /// pluggable part receiving the exports is armed as a dependency for said exports.
        /// </summary>
        /// <typeparam name="TDependency">The type to arm the pluggable part receiving the injections as.</typeparam>
        /// <param name="pluggablePart">An object containing loose import attributions.</param>
        public static void SelfInject<TDependency>(object pluggablePart) 
            => SelfInject<TDependency>(pluggablePart, Store.GlobalContext);

        /// <summary>
        /// Injects exports belonging to the specified filterable family into the provided attributed pluggable part
        /// within a context where the pluggable part receiving the exports is armed as a dependency for said exports.
        /// </summary>
        /// <typeparam name="TDependency">The type to arm the pluggable part receiving the injections as.</typeparam>
        /// <param name="pluggablePart">An object containing loose import attributions.</param>
        /// <param name="familyId">Identifies the filterable family that the injected exports must belong to.</param>
        public static void SelfInject<TDependency>(object pluggablePart, Guid familyId)
        {
            if (!IsFilterable(familyId))
                throw new ArgumentException(Strings.FamilyIdNotRegistered.InvariantFormat(familyId), nameof(familyId));

            SelfInject<TDependency>(pluggablePart, Store.FilterableContexts[familyId]);
        }

        /// <summary>
        /// Determines if the provided identifier belongs to that of a known filterable family.
        /// </summary>
        /// <param name="familyId">The identifier to check.</param>
        /// <returns>True if <c>familyId</c> describes a known filterable family; otherwise, false.</returns>
        public static bool IsFilterable(Guid familyId)
            => Store.FilterableFamilies.ContainsKey(familyId);

        /// <summary>
        /// Updates the active configuration in use by the plugin host, cleaning up and re-initializing the inner
        /// <see cref="PluginStore"/> (if one was already initialized) to use the new configuration.
        /// </summary>
        /// <param name="configuration">The new Extensibility framework configuration the host should use.</param>
        /// <remarks>
        /// If the inner <see cref="PluginStore"/> has not yet been initialized, this will leave it in an uninitialized state,
        /// and instead prime it so that <c>configuration</c> is used instead of the configuration defaults.
        /// </remarks>
        public static void UpdateConfiguration(IExtensibilityConfiguration configuration)
        {
            Require.NotNull(configuration, nameof(configuration));

            lock (_StoreLock)
            {
                if (configuration == _Configuration)
                    return;
                
                _Configuration = configuration;

                if (!_Store.IsValueCreated)
                    return;

                Store.Dispose();

                _Store.Value = new PluginStore(_Configuration);
            }
        }

        private static PluginStore InitializeStore()
        {
            lock (_StoreLock)
            {
                // An additional lock is required here in order to synchronize configuration updates from UpdateConfiguration.
                return new PluginStore(_Configuration);
            }
        }

        private static void SelfInject<TDependency>(object pluggablePart, PluginContext context)
        {
            Require.NotNull(pluggablePart, nameof(pluggablePart));

            if (pluggablePart is not TDependency partAsDependency)
            {
                throw new ArgumentException(
                    Strings.IncompatibleDependencyTypeForInjection.InvariantFormat(pluggablePart.GetType(), typeof(TDependency)),
                    nameof(pluggablePart));
            }

            DependencyRegistry<TDependency>
                .ExecuteWhileArmed(partAsDependency, () => context.Inject(pluggablePart));
        }
    }
}