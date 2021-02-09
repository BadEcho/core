//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Composition.Hosting;
using System.IO;
using BadEcho.Odin.Extensibility.Hosting;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility
{
    public class GlobalPluginContextStrategyTests
    {
        private readonly IPluginContextStrategy _strategy;

        public GlobalPluginContextStrategyTests()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "plugins");
            _strategy = new GlobalPluginContextStrategy(path);
        }

        [Fact]
        public void IFakePart_GetExports()
        {
            var container = _strategy.CreateContainer();

            var parts = container.GetExports<IFakePart>();
            
            Assert.NotEmpty(parts);
        }

        [Fact]
        public void IFakePart_GetExport()
        {
            var container = _strategy.CreateContainer();

            Assert.Throws<CompositionFailedException>(container.GetExport<IFakePart>);
        }

        [Fact]
        public void INonSharedFakePart_IsNonShared()
        {
            var container = _strategy.CreateContainer();

            var firstPart = container.GetExport<INonSharedFakePart>();
            var secondPart = container.GetExport<INonSharedFakePart>();

            Assert.NotEqual(firstPart, secondPart);
        }

        [Fact]
        public void ISharedFakePart_IsShared()
        {
            var container = _strategy.CreateContainer();

            var firstPart = container.GetExport<ISharedFakePart>();
            var secondPart = container.GetExport<ISharedFakePart>();

            Assert.Equal(firstPart, secondPart);
        }

        [Fact]
        public void IFakePartWithDependencies_GetExport()
        {
            var container = _strategy.CreateContainer();

            var part = container.GetExport<IFakePartWithDependencies>();
            var dependency = container.GetExport<IFakeDependency>();

            Assert.NotNull(part);
            Assert.NotNull(part.Dependency);
            Assert.NotNull(dependency);
            Assert.Equal(part.Dependency.GetType(), dependency.GetType());
        }
    }
}
