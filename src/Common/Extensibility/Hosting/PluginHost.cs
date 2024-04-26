//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Runtime.CompilerServices;
using BadEcho.Extensibility.Configuration;
using BadEcho.Logging;
using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Extensibility.Hosting;

/// <summary>
/// Provides a platform and host for consuming plugins supported by the Bad Echo Extensibility framework.
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
        => IsSupported<TContract>(Store.GlobalContext);

    /// <summary>
    /// Determines if the functionality provided by the specified generic contract is available locally from the executing
    /// process.
    /// environment.
    /// </summary>
    /// <typeparam name="TContract">The contract type to check for available exports.</typeparam>
    /// <returns>True if <typeparamref name="TContract"/> has exports provided; otherwise, false.</returns>
    /// <remarks>
    /// This method allows you to check for the existence of exports for <typeparamref name="TContract"/> while avoiding the
    /// instantiation of any exported parts that are found.
    /// </remarks>
    public static bool IsSupportedByProcess<TContract>()
    {
        Assembly? entryAssembly = Assembly.GetEntryAssembly();

        return entryAssembly != null && IsSupported<TContract>(Store.LoadContext(entryAssembly));
    }

    /// <summary>
    /// Determines if the functionality provided by the specified generic contract is available locally from the caller.
    /// </summary>
    /// <typeparam name="TContract">The contract type to check for available exports.</typeparam>
    /// <returns>True if <typeparamref name="TContract"/> has exports provided; otherwise, false.</returns>
    /// <remarks>
    /// This method allows you to check for the existence of exports for <typeparamref name="TContract"/> while avoiding the
    /// instantiation of any exported parts that are found.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static bool IsSupportedByCaller<TContract>()
    {
        Assembly callingAssembly = Assembly.GetCallingAssembly();

        return IsSupported<TContract>(Store.LoadContext(callingAssembly));
    }

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
    /// contract type for a family begs the question whether enough filterable families themselves are defined.
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
    /// Retrieves the export fulfilling the specified generic contract from the hosting process executable itself.
    /// </summary>
    /// <typeparam name="TContract">The contract type whose exports should be loaded.</typeparam>
    /// <returns>An exported <typeparamref name="TContract"/> value provided by the hosting process executable.</returns>
    /// <remarks>
    /// <para>
    /// This allows access to exports that an executing process or application wants to make available either to itself or the
    /// plugins that target it. A common usage for this approach would be the configuration provider endpoint for the application.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException"> 
    /// The process hosting the code is an unmanaged executable; this method can only be used if the process stems from a managed
    /// executable.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Multiple exports were found for the <typeparamref name="TContract"/> contract. It is expected that only a single contract
    /// implementation be made available within locally sourced contexts.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// No exports were found for the <typeparamref name="TContract"/> contract. Given that it is the process itself providing the export,
    /// an assumption is made that <typeparamref name="TContract"/> is a requirement for the proper execution of the application.
    /// To check if a contract implementation exists beforehand, call the <see cref="IsSupportedByProcess{TContract}"/> method first.
    /// </exception>
    public static TContract LoadFromProcess<TContract>()
    {
        Assembly entryAssembly = Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException(Strings.ProcessCannotExportContracts);

        return LoadLocally<TContract>(entryAssembly);
    }

    /// <summary>
    /// Retrieves the export fulfilling the specified generic contract from the caller itself.
    /// </summary>
    /// <typeparam name="TContract">The contract type whose exports should be loaded.</typeparam>
    /// <returns>An exported <typeparamref name="TContract"/> value provided by the caller.</returns>
    /// <remarks>
    /// <para>
    /// This allows access to exports that a component needs to make available either to itself or any plugins that happen
    /// to target it. This is an alternative way to access exports from non-plugin assemblies when a hosting process
    /// executable isn't available for such a task.
    /// </para>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Multiple exports were found for the <typeparamref name="TContract"/> contract. It is expected that only a single contract
    /// implementation be made available within locally sourced contexts.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// No exports were found for the <typeparamref name="TContract"/> contract. Given that it is the caller itself providing the export,
    /// an assumption is made that <typeparamref name="TContract"/> is a requirement for the proper execution of the component.
    /// To check if a contract implementation exists beforehand, call the <see cref="IsSupportedByCaller{TContract}"/> method first.
    /// </exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static TContract LoadFromCaller<TContract>()
    {
        Assembly callingAssembly = Assembly.GetCallingAssembly();

        return LoadLocally<TContract>(callingAssembly);
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
    
    private static bool IsSupported<TContract>(PluginContext context)
    {
        var lazyParts = context.Load<Lazy<TContract>>();

        return lazyParts.Any();
    }

    private static TContract LoadLocally<TContract>(Assembly assembly)
    {
        var parts = Store.LoadContext(assembly)
                         .Load<TContract>();

        TContract? uniquePart = default;

        foreach (var part in parts)
        {
            if (uniquePart != null)
            {
                throw new InvalidOperationException(
                    Strings.MultipleExportsFoundForLocalContract.InvariantFormat(typeof(TContract)));
            }

            uniquePart = part;
        }

        if (uniquePart == null)
        {
            throw new InvalidOperationException(
                Strings.NoExportFoundForLocalContract.InvariantFormat(typeof(TContract)));
        }

        return uniquePart;
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