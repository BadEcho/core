using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Extensibility.Hosting;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility
{
    public class FilterablePluginContextStrategyTests
    {
        private readonly string _path;

        public FilterablePluginContextStrategyTests()
        {
            _path = Path.Combine(Environment.CurrentDirectory, "plugins");
        }

        [Theory]
        [InlineData(FakeIds.AlphaFakeIdValue)]
        [InlineData(FakeIds.BetaFakeIdValue)]
        [InlineData(FakeIds.GammaFakeIdValue)]
        public void FilterableTypes_GetExports(string fakeId)
        {
            var strategy = new FilterablePluginContextStrategy(_path, new Guid(fakeId));
            var container = strategy.CreateContainer();

            var parts = container.GetExports<IFilterableFakePart>();

            Assert.NotEmpty(parts);
            Assert.All(parts, p => Assert.Equal(new Guid(fakeId), p.TypeIdentifier));
        }

        [Theory]
        [InlineData(FakeIds.AlphaFakeIdValue)]
        [InlineData(FakeIds.BetaFakeIdValue)]
        public void SingleFilterableTypes_GetExport(string fakeId)
        {
            var strategy = new FilterablePluginContextStrategy(_path, new Guid(fakeId));
            var container = strategy.CreateContainer();

            var part = container.GetExport<IFilterableFakePart>();

            Assert.NotNull(part);
            Assert.Equal(new Guid(fakeId), part.TypeIdentifier);
        }

        [Fact]
        public void FilterableType_IsNonShared()
        {
            var strategy = new FilterablePluginContextStrategy(_path, FakeIds.BetaFakeId);
            var container = strategy.CreateContainer();

            var firstPart = container.GetExport<IFilterableFakePart>();
            var secondPart = container.GetExport<IFilterableFakePart>();

            Assert.NotEqual(firstPart, secondPart);
        }

        [Fact]
        public void SharedFilterableType_IsShared()
        {
            var strategy = new FilterablePluginContextStrategy(_path, FakeIds.GammaFakeId);
            var container = strategy.CreateContainer();

            var firstPart = container.GetExport<IFilterableFakePart>();
            var secondPart = container.GetExport<IFilterableFakePart>();

            Assert.Equal(firstPart, secondPart);
        }
    }
}
