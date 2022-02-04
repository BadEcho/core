//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Extensibility.Configuration;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.XmlConfiguration.Extensibility;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility;

/// <suppressions>
/// ReSharper disable UnusedVariable
/// </suppressions>
public class HostAdapterTests
{
    private readonly IPluginContextStrategy _strategy;

    public HostAdapterTests()
    {
        var path = Path.Combine(Environment.CurrentDirectory, "testPlugins");
        _strategy = new GlobalPluginContextStrategy(path);
    }

    [Fact]
    public void Initialize_NoPlugins_ThrowsException()
    {
        var strategy = new EmptyPluginContextStrategy();
        var context = new PluginContext(strategy);
        Assert.Throws<ArgumentException>(() => new HostAdapter<ISegmentedContract>(context, new ExtensibilityConfiguration()));
    }

    [Fact]
    public void Route_TwoSegments_ClaimedMethodsExecuted()
    {
        var configuration = CreateAlphaPrimaryConfiguration();
        var context = new PluginContext(_strategy);
        var hostAdapter = new HostAdapter<ISegmentedContract>(context, configuration);

        var firstPart = hostAdapter.Route(nameof(ISegmentedContract.SomeMethod));

        Assert.Equal(ISegmentedContract.FirstSomeMethod, firstPart.SomeMethod());

        var secondPart = hostAdapter.Route(nameof(ISegmentedContract.SomeOtherMethod));

        Assert.Equal(ISegmentedContract.SecondSomeOtherMethod, secondPart.SomeOtherMethod());
    }

    [Fact]
    public void Route_XmlConfig_ClaimedMethodsExecuted()
    {
        var configuration = CreateAlphaPrimaryConfiguration();
        var context = new PluginContext(_strategy);
        var hostAdapter = new HostAdapter<ISegmentedContract>(context, configuration);

        var proxy = RoutableProxy.Create<ISegmentedContract>(hostAdapter);

        Assert.Equal(ISegmentedContract.FirstSomeMethod, proxy.SomeMethod());
        Assert.Equal(ISegmentedContract.SecondSomeOtherMethod, proxy.SomeOtherMethod());
    }

    [Fact]
    public void Route_JsonConfig_ClaimedMethodsExecuted()
    {
        var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("test.json");

        var configurationRoot = builder.Build();

        var configuration = configurationRoot.Get<ExtensibilityConfiguration>();
        var context = new PluginContext(_strategy);
        var hostAdapter = new HostAdapter<ISegmentedContract>(context, configuration);

        var proxy = RoutableProxy.Create<ISegmentedContract>(hostAdapter);

        Assert.Equal(ISegmentedContract.FirstSomeMethod, proxy.SomeMethod());
        Assert.Equal(ISegmentedContract.SecondSomeOtherMethod, proxy.SomeOtherMethod());
    }

    [Fact]
    public void Route_TwoSegmentsProxied_ClaimedMethodsExecuted()
    {
        var configuration = ExtensibilityConfigurationProvider.LoadConfiguration();
        var context = new PluginContext(_strategy);
        var hostAdapter = new HostAdapter<ISegmentedContract>(context, configuration);

        var proxy = RoutableProxy.Create<ISegmentedContract>(hostAdapter);

        Assert.Equal(ISegmentedContract.FirstSomeMethod, proxy.SomeMethod());
        Assert.Equal(ISegmentedContract.SecondSomeOtherMethod, proxy.SomeOtherMethod());
    }

    private IExtensibilityConfiguration CreateAlphaPrimaryConfiguration()
    {
        return new ExtensibilityConfiguration
               {
                   SegmentedContracts
                       = new List<ContractConfiguration>
                         {
                             new()
                             {
                                 Name = nameof(ISegmentedContract),
                                 RoutablePlugins
                                     = new List<RoutablePluginConfiguration>
                                       {
                                           new() { Id = FakeAdapterIds.AlphaFakeId, Primary = true },
                                           new() { 
                                                     Id = FakeAdapterIds.BetaFakeId, 
                                                     MethodClaims  = new List<string> {nameof(ISegmentedContract.SomeOtherMethod)}
                                                 }
                                       }
                             }
                         }
               };
    }
}