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

using BadEcho.Odin.Extensibility.Configuration;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility.Hosting;

/// <summary>
/// Provides an adapter that is responsible for establishing connections between a host and several routable
/// plugin adapters that segment a common contract.
/// </summary>
/// <typeparam name="T">
/// The type of contract being segmented by the plugins connected through this adapter.
/// </typeparam>
public sealed class HostAdapter<T> : IHostAdapter
    where T : notnull
{
    private readonly IDictionary<string, IPluginAdapter<T>> _routingTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostAdapter{T}"/> class.
    /// </summary>
    /// <param name="context">The context to access the routable adapters through.</param>
    /// <param name="configuration">
    /// Configuration for the Extensibility framework containing call-routable plugin information.
    /// </param>
    internal HostAdapter(PluginContext context, IExtensibilityConfiguration configuration)
    {
        Require.NotNull(context, nameof(context));
        Require.NotNull(configuration, nameof(configuration));

        IDictionary<Guid, IPluginAdapter<T>> adapters
            = context.Load<IPluginAdapter<T>, RoutableMetadataView>()
                     .ToDictionary(k => k.Metadata.PluginId, v => v.Value);

        IContractConfiguration contractConfiguration = FindContractConfiguration(configuration);

        _routingTable = CreateRoutingTable(contractConfiguration, adapters);
    }

    object IHostAdapter.Route(string methodName) 
        => Route(methodName);

    /// <summary>
    /// Routes this call to the appropriate call-routable plugin <typeparamref name="T"/> implementation.
    /// </summary>
    /// <param name="methodName">The name of the method to execute.</param>
    /// <returns>
    /// The <typeparamref name="T"/> implementation registered to execute the method specified by <c>methodName</c>.
    /// </returns>
    public T Route(string methodName)
    {
        Require.NotNullOrEmpty(methodName, nameof(methodName));

        if (!_routingTable.ContainsKey(methodName))
        {
            throw new InvalidOperationException(
                Strings.HostAdapterUnregisteredMethod.InvariantFormat(methodName));
        }
            
        return _routingTable[methodName].Contract;
    }

    private static IContractConfiguration FindContractConfiguration(IExtensibilityConfiguration configuration)
    {
        string contractName = typeof(T).Name;

        IContractConfiguration? contractConfiguration
            = configuration.SegmentedContracts
                           .FirstOrDefault(c => c.Name == contractName);

        if (contractConfiguration == null)
        {
            throw new ArgumentException(Strings.NoContractInConfiguration.InvariantFormat(contractName),
                                        nameof(configuration));
        }

        return contractConfiguration;
    }

    private static IDictionary<string, IPluginAdapter<T>> CreateRoutingTable(
        IContractConfiguration configuration,
        IDictionary<Guid, IPluginAdapter<T>> adapters)
    {
        Guid primaryId = configuration.RoutablePlugins.First(p => p.Primary).Id;
            
        IEnumerable<IRoutablePluginConfiguration> nonPrimaryPlugins
            = configuration.RoutablePlugins.Where(p => p.Id != primaryId);

        IDictionary<string, IPluginAdapter<T>> routingTable
            = nonPrimaryPlugins
              .SelectMany(p => p.MethodClaims, (config, method) => new {config.Id, Claim = method})
              .ToDictionary(k => k.Claim, v => adapters[v.Id]);

        IPluginAdapter<T> primaryAdapter = adapters[primaryId];

        IEnumerable<string> unclaimedMethodNames
            = typeof(T).GetMethods()
                       .Where(m => !routingTable.ContainsKey(m.Name))
                       .Select(m => m.Name)
                       .Distinct();

        foreach (var unclaimedMethodName in unclaimedMethodNames)
        {
            routingTable.Add(unclaimedMethodName, primaryAdapter);
        }

        return routingTable;
    }
}