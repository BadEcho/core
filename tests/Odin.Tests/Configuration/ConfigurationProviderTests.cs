//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BadEcho.Odin.Extensibility.Configuration;
using BadEcho.Odin.XmlConfiguration.Extensibility;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BadEcho.Odin.Tests.Configuration
{
    public class ConfigurationProviderTests
    {
        [Fact]
        public void GetExtensibilityConfiguration_Json_ReturnsValid()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("test.json");

            var configuration = builder.Build();

            var extensibility = configuration.Get<ExtensibilityConfiguration>();

            ValidateConfiguration(extensibility);
        }

        [Fact]
        public void GetContractSection_Json_ReturnsValid()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("test.json");

            var configuration = builder.Build();

            var contracts = configuration.GetSection("segmentedContracts")
                                         .Get<IEnumerable<ContractConfiguration>>();

            Assert.Single(contracts);
        }

        [Fact]
        public void GetExtensibilityConfiguration_Xml_ReturnsValid()
        {
            var extensibility = ExtensibilityConfigurationProvider.LoadConfiguration();

            ValidateConfiguration(extensibility);
        }

        private static void ValidateConfiguration(IExtensibilityConfiguration extensibility)
        {
            Assert.Equal("testPlugins", extensibility.PluginDirectory);
            Assert.NotEmpty(extensibility.SegmentedContracts);

            var contract = extensibility.SegmentedContracts.First();

            Assert.Equal("ISegmentedContract", contract.Name);
            Assert.NotEmpty(contract.RoutablePlugins);
            Assert.Equal(2, contract.RoutablePlugins.Count());

            var firstPlugin = contract.RoutablePlugins.First();
            var secondPlugin = contract.RoutablePlugins.Skip(1).First();

            Assert.True(firstPlugin.Primary);
            Assert.False(secondPlugin.Primary);

            Assert.Equal(new Guid("BE157C54-2BC0-48B8-9E39-0AE53D8A4E61"), firstPlugin.Id);
            Assert.Equal(new Guid("F544EC74-919E-4167-A421-FA74223F49C5"), secondPlugin.Id);

            Assert.NotEmpty(secondPlugin.MethodClaims);

            var methodClaim = secondPlugin.MethodClaims.First();

            Assert.Equal("SomeOtherMethod", methodClaim);
        }
    }
}
