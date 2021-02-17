//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using BadEcho.Odin.Configuration;
using BadEcho.Odin.Extensibility.Configuration;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides an adapter that is responsible for establishing connections between a host and several routable
    /// plugin adapters that segment a common contract.
    /// </summary>
    /// <typeparam name="T">
    /// The type of contract being segmented by the plugins connected through this adapter.
    /// </typeparam>
    internal sealed class HostAdapter<T> : IHostAdapter
        where T : notnull
    {
        private readonly IDictionary<string, IPluginAdapter<T>> _routingTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostAdapter{T}"/> class.
        /// </summary>
        /// <param name="context">The context to access the routable adapters through.</param>
        public HostAdapter(PluginContext context)
        {
            Require.NotNull(context, nameof(context));

            IDictionary<Guid, IPluginAdapter<T>> adapters
                = context.Load<IPluginAdapter<T>, RoutingMetadataView>()
                         .ToDictionary(k => k.Metadata.PluginIdentifier, v => v.Value);

            ContractElement configuration = LoadContractConfiguration();

            _routingTable = CreateRoutingTable(configuration, adapters);
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

        private static ContractElement LoadContractConfiguration()
        {
            string contractName = typeof(T).Name;

            ExtensibilitySection configuration = ExtensibilitySection.GetSection();

            if (configuration == null)
                throw new ConfigurationMissingException(MissingConfigurationType.Section, ExtensibilitySection.Schema);

            ContractElement? contractElement = configuration.Contracts[contractName];

            if (null == contractElement)
                throw new ConfigurationMissingException(MissingConfigurationType.Element, contractName);

            IEnumerable<IGrouping<string, MethodClaimElement>> claimsByName
                = contractElement.RoutablePlugins
                                 .SelectMany(p => p.MethodClaims)
                                 .GroupBy(m => m.Name);

            if (claimsByName.Any(m => m.Count() > 1))
                throw new ConfigurationErrorsException(Strings.MethodClaimedByMultiplePlugins);

            return contractElement;
        }

        private static IDictionary<string, IPluginAdapter<T>> CreateRoutingTable(
            ContractElement configuration,
            IDictionary<Guid, IPluginAdapter<T>> adapters)
        {
            Guid primaryId = configuration.FindPrimaryPluginId();

            IEnumerable<RoutablePluginElement> nonPrimaryPlugins
                = configuration.RoutablePlugins.Where(p => p.Id != primaryId);

            IDictionary<string, IPluginAdapter<T>> routingTable
                = nonPrimaryPlugins
                  .SelectMany(p => p.MethodClaims, (config, claim) => new {config.Id, claim.Name})
                  .ToDictionary(k => k.Name, v => adapters[v.Id]);

            IPluginAdapter<T> primaryAdapter = adapters[primaryId];

            IEnumerable<string> unclaimedMethodNames
                = typeof(T).GetMethods()
                           .Where(m => !routingTable.Keys.Contains(m.Name))
                           .Select(m => m.Name)
                           .Distinct();

            foreach (var unclaimedMethodName in unclaimedMethodNames)
            {
                routingTable.Add(unclaimedMethodName, primaryAdapter);
            }

            return routingTable;
        }
    }
}