//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensibility.Configuration;
using BadEcho.Extensibility.Hosting;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BadEcho.Tests.Extensibility;

/// <suppressions>
/// ReSharper disable AssignNullToNotNullAttribute
/// </suppressions>
public class PluginHostTests
{
    public PluginHostTests()
    {
        var configuration = new ExtensibilityConfiguration {PluginDirectory = "testPlugins"};

        PluginHost.UpdateConfiguration(configuration);
    }

    [Fact]
    public void Load_IFakePart_ReturnsParts()
    {
        var parts = PluginHost.Load<IFakePart>();

        Assert.NotEmpty(parts);
    }

    [Fact]
    public void Load_AlphaFilterableFakePart_ReturnsPart()
    {
        var part = PluginHost.Load<IFilterableFakePart>(FakeFilterableIds.AlphaFakeId);

        Assert.NotNull(part);
    }

    [Fact]
    public void IsFilterable_BetaFilterableFakePart_ReturnsTrue()
    {
        Assert.True(PluginHost.IsFilterable(FakeFilterableIds.BetaFakeId));
    }

    [Fact]
    public void LoadAdapter_ISegmentedContract_ReturnsAdapter()
    {
        var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("test.json");

        var configurationRoot = builder.Build();

        var configuration = configurationRoot.Get<ExtensibilityConfiguration>();
        Assert.NotNull(configuration);

        PluginHost.UpdateConfiguration(configuration);

        var hostAdapter = PluginHost.LoadAdapter<ISegmentedContract>();

        Assert.NotNull(hostAdapter);
    }

    [Fact]
    public void Inject_PluggablePart_PartsImported()
    {
        var pluggablePart = new PluggablePart();

        PluginHost.Inject(pluggablePart);

        Assert.NotNull(pluggablePart.FakeParts);
        Assert.NotEmpty(pluggablePart.FakeParts);
    }

    [Theory]
    [InlineData(FakeFilterableIds.AlphaFakeIdValue)]
    [InlineData(FakeFilterableIds.BetaFakeIdValue)]
    [InlineData(FakeFilterableIds.GammaFakeIdValue)]
    [InlineData(FakeFilterableIds.DeltaFakeIdValue)]
    public void Inject_PluggablePartWithFilterableImports_PartImported(string fakeId)
    {
        var pluggablePart = new PluggablePartWithFilterableImports();

        PluginHost.Inject(pluggablePart, new Guid(fakeId));
    }

    [Fact]
    public void SelfInject_PluggablePartAndDependency_PartsImported()
    {
        var pluggablePart = new PluggablePartAndDependency();

        PluginHost.SelfInject<IFakeDependency>(pluggablePart);

        Assert.NotNull(pluggablePart.FakeParts);
        Assert.NotEmpty(pluggablePart.FakeParts);
    }

    [Fact]
    public void SelfInject_PluggablePartAndFilterableDependency_PartImported()
    {
        var pluggablePart = new PluggablePartAndFilterableDependency(FakeFilterableIds.AlphaFakeId);

        PluginHost.SelfInject<IFilterableFakeDependency>(pluggablePart, pluggablePart.FamilyId);

        Assert.NotNull(pluggablePart.FakePart);
    }

    [Fact]
    public void IsSupported_IUniqueRequirement_ReturnsTrue()
    {
        Assert.True(PluginHost.IsSupported<IUniqueRequirement>());
    }

    [Fact]
    public void IsSupported_NonPluginContract_ReturnsFalse()
    {
        Assert.False(PluginHost.IsSupported<ITestCollectionOrderer>());
    }

    [Fact]
    public void ArmedLoad_IFakePartWithComposedDependencies_ReturnsPart()
    {
        var dependency = new ComposedDependency();
        var part = PluginHost.ArmedLoad<IFakePartWithComposedDependencies, IFakeDependency>(dependency);

        Assert.NotNull(part);
    }

    private class ComposedDependency : IFakeDependency;
}