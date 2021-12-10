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

using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Tests.ExtensibilityPoint;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility;

public class FilterablePluginContextStrategyTests
{
    private readonly string _path;

    public FilterablePluginContextStrategyTests()
    {
        _path = Path.Combine(Environment.CurrentDirectory, "testPlugins");
    }

    [Theory]
    [InlineData(FakeFilterableIds.AlphaFakeIdValue)]
    [InlineData(FakeFilterableIds.BetaFakeIdValue)]
    [InlineData(FakeFilterableIds.GammaFakeIdValue)]
    public void GetExports_FilterableTypes_ReturnsPartsFromFamily(string fakeId)
    {
        var strategy = new FilterablePluginContextStrategy(_path, new Guid(fakeId));
        var container = strategy.CreateContainer();
        var parts = container.GetExports<IFilterableFakePart>().ToList();

        Assert.NotEmpty(parts);
        Assert.All(parts, p => Assert.Equal(new Guid(fakeId), p.FamilyId));
    }

    [Theory]
    [InlineData(FakeFilterableIds.BetaFakeIdValue)]
    [InlineData(FakeFilterableIds.DeltaFakeIdValue)]
    public void GetExport_SingleFilterableTypes_ReturnsPartFromFamily(string fakeId)
    {
        var strategy = new FilterablePluginContextStrategy(_path, new Guid(fakeId));
        var container = strategy.CreateContainer();
        var part = container.GetExport<IFilterableFakePart>();

        Assert.NotNull(part);
        Assert.Equal(new Guid(fakeId), part.FamilyId);
    }

    [Fact]
    public void GetExport_BetaFilterableType_IsNonShared()
    {
        var strategy = new FilterablePluginContextStrategy(_path, FakeFilterableIds.BetaFakeId);
        var container = strategy.CreateContainer();
        var firstPart = container.GetExport<IFilterableFakePart>();
        var secondPart = container.GetExport<IFilterableFakePart>();

        Assert.NotEqual(firstPart, secondPart);
    }

    [Fact]
    public void GetExport_GammaFilterableType_IsShared()
    {
        var strategy = new FilterablePluginContextStrategy(_path, FakeFilterableIds.GammaFakeId);
        var container = strategy.CreateContainer();
        var firstPart = container.GetExport<IFilterableFakePart>();
        var secondPart = container.GetExport<IFilterableFakePart>();

        Assert.Equal(firstPart, secondPart);
    }

    [Fact]
    public void GetExport_IFilterableFakePartWithDependencies_ReturnsPartWithDependency()
    {
        var strategy = new FilterablePluginContextStrategy(_path, FakeFilterableIds.AlphaFakeId);
        var container = strategy.CreateContainer();
        var part = container.GetExport<IFilterableFakePartWithDependencies>();
        var dependency = container.GetExport<IFilterableFakeDependency>();

        Assert.NotNull(part);
        Assert.NotNull(part.Dependency);
        Assert.NotNull(dependency);
        Assert.Equal(part.Dependency.GetType(), dependency.GetType());
    }

    [Fact]
    public void GetExport_IFilterableFakePartWithComposedDependencies_ReturnsPartWithDependency()
    {
        var strategy = new FilterablePluginContextStrategy(_path, FakeFilterableIds.AlphaFakeId);
        var container = strategy.CreateContainer();
        var dependency = container.GetExport<IFilterableFakeDependency>();

        DependencyRegistry<IFilterableFakeDependency>
            .ExecuteWhileArmed(dependency,
                               () =>
                               {
                                   var part = container.GetExport<IFilterableFakePartWithComposedDependencies>();
                                   Assert.NotNull(part);
                                   Assert.Equal(dependency, part.Dependency);
                               });
    }

    [Fact]
    public void GetExport_IFilterableFakePartWithComposedDependencies_IsRecomposed()
    {
        var strategy = new FilterablePluginContextStrategy(_path, FakeFilterableIds.AlphaFakeId);
        var container = strategy.CreateContainer();
        var dependency = container.GetExport<IFilterableFakeDependency>();

        IFilterableFakePartWithComposedDependencies part = null!;

        DependencyRegistry<IFilterableFakeDependency>
            .ExecuteWhileArmed(dependency,
                               () => part = container.GetExport<IFilterableFakePartWithComposedDependencies>());

        var newDependency = container.GetExport<IFilterableFakeDependency>();

        IFilterableFakePartWithComposedDependencies newPart = null!;

        DependencyRegistry<IFilterableFakeDependency>
            .ExecuteWhileArmed(newDependency,
                               () => newPart = container.GetExport<IFilterableFakePartWithComposedDependencies>());

        Assert.NotNull(newPart);
        Assert.Equal(newDependency, newPart.Dependency);
        Assert.NotNull(part);
        Assert.NotEqual(part.Dependency, newPart.Dependency);
    }

    [Fact]
    public void GetExport_IFilterableFakePartWithNonFilterableDependencies_ReturnsPartWithDependency()
    {
        var strategy = new FilterablePluginContextStrategy(_path, FakeFilterableIds.AlphaFakeId);
        var container = strategy.CreateContainer();
        var dependency = new ComposedDependency();

        DependencyRegistry<IFakeDependency>
            .ExecuteWhileArmed(dependency,
                               () =>
                               {
                                   var part = container.GetExport<IFilterableFakePartWithNonFilterableDependencies>();

                                   Assert.NotNull(part);
                                   Assert.Equal(dependency, part.Dependency);
                               });
    }

    [Fact]
    public void GetExports_ExtensibilityPart_ReturnsEmpty()
    {
        var strategy = new FilterablePluginContextStrategy(_path, FakeFilterableIds.AlphaFakeId);
        var container = strategy.CreateContainer();

        Assert.Empty(container.GetExports<IExtensibilityPart>());
    }

    private class ComposedDependency : IFakeDependency
    { }
}