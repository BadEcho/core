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
    public class GlobalPluginContextFactoryTests
    {
        private readonly IPluginContextFactory _factory;

        public GlobalPluginContextFactoryTests()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "plugins");
            _factory = new GlobalPluginContextFactory(path);
        }

        [Fact]
        public void IFakePart_GetExports()
        {
            var container = _factory.CreateContainer();

            var parts = container.GetExports<IFakePart>();
            
            Assert.NotEmpty(parts);
        }

        [Fact]
        public void IFakePart_GetExport()
        {
            var container = _factory.CreateContainer();

            Assert.Throws<CompositionFailedException>(container.GetExport<IFakePart>);
        }

        [Fact]
        public void INonSharedFakePart_IsNonShared()
        {
            var container = _factory.CreateContainer();

            var firstPart = container.GetExport<INonSharedFakePart>();
            var secondPart = container.GetExport<INonSharedFakePart>();

            Assert.NotEqual(firstPart, secondPart);
        }

        [Fact]
        public void ISharedFakePart_IsShared()
        {
            var container = _factory.CreateContainer();

            var firstPart = container.GetExport<ISharedFakePart>();
            var secondPart = container.GetExport<ISharedFakePart>();

            Assert.Equal(firstPart, secondPart);
        }
    }
}
