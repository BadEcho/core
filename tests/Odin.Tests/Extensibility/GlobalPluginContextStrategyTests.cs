//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Tests.ExtensibilityPoint;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility
{
    public class GlobalPluginContextStrategyTests
    {
        private readonly IPluginContextStrategy _strategy;

        public GlobalPluginContextStrategyTests()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "testPlugins");
            _strategy = new GlobalPluginContextStrategy(path);
        }

        [Fact]
        public void GetExports_IFakePart_ReturnsParts()
        {
            var container = _strategy.CreateContainer();

            var parts = container.GetExports<IFakePart>();
            
            Assert.NotEmpty(parts);
        }

        [Fact]
        public void GetExport_IFakePart_ThrowsException()
        {
            var container = _strategy.CreateContainer();

            Assert.Throws<CompositionFailedException>(container.GetExport<IFakePart>);
        }

        [Fact]
        public void GetExport_INonSharedFakePart_IsNonShared()
        {
            var container = _strategy.CreateContainer();

            var firstPart = container.GetExport<INonSharedFakePart>();
            var secondPart = container.GetExport<INonSharedFakePart>();

            Assert.NotEqual(firstPart, secondPart);
        }

        [Fact]
        public void GetExport_ISharedFakePart_IsShared()
        {
            var container = _strategy.CreateContainer();

            var firstPart = container.GetExport<ISharedFakePart>();
            var secondPart = container.GetExport<ISharedFakePart>();

            Assert.Equal(firstPart, secondPart);
        }

        [Fact]
        public void GetExport_IFakePartWithDependencies_ReturnsPartWithDependency()
        {
            var container = _strategy.CreateContainer();

            var part = container.GetExport<IFakePartWithDependencies>();
            var dependency = container.GetExport<IFakeDependency>();

            Assert.NotNull(part);
            Assert.NotNull(part.Dependency);
            Assert.NotNull(dependency);
            Assert.Equal(part.Dependency.GetType(), dependency.GetType());
        }

        [Fact]
        public void GetExport_IFakePartWithComposedDependencies_ReturnsPartWithDependency()
        {
            var composedDependency = new ComposedDependency();

            DependencyRegistry<IFakeDependency>
                .ExecuteWhileArmed(composedDependency,
                                   () =>
                                   {
                                       var container = _strategy.CreateContainer();
                                       var part = container.GetExport<IFakePartWithComposedDependencies>();

                                       Assert.NotNull(part);
                                       Assert.Equal(composedDependency, part.Dependency);
                                   });
        }

        [Fact]
        public void GetExport_IFakePartWithComposedDependencies_IsRecomposed()
        {
            var container = _strategy.CreateContainer();

            IFakePartWithComposedDependencies part = null!;

            DependencyRegistry<IFakeDependency>
                .ExecuteWhileArmed(new ComposedDependency(),
                                   () => part = container.GetExport<IFakePartWithComposedDependencies>());
            
            var newDependency = new ComposedDependency();

            IFakePartWithComposedDependencies newPart = null!;

            DependencyRegistry<IFakeDependency>
                    .ExecuteWhileArmed(newDependency,
                        () => newPart = container.GetExport<IFakePartWithComposedDependencies>());

            Assert.NotNull(newPart);
            Assert.Equal(newDependency, newPart.Dependency);
            Assert.NotNull(part);
            Assert.NotEqual(part.Dependency, newPart.Dependency);
        }

        [Fact]
        public void GetExports_IFilterableFamily_ValidCount()
        {
            var container = _strategy.CreateContainer();
            var families = container.GetExports<Lazy<IFilterableFamily, FilterableFamilyMetadataView>>();

            Assert.Equal(4, families.Count());
        }

        [Fact]
        public void GetExports_ExtensibilityPoint_ReturnsPart()
        {
            var container = _strategy.CreateContainer();
            
            Assert.NotNull(container.GetExport<IExtensibilityPart>());
        }

        private class ComposedDependency : IFakeDependency  
        { }
    }
}
