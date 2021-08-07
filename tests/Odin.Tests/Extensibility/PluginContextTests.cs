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

using System;
using System.IO;
using BadEcho.Odin.Extensibility.Hosting;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility
{
    /// <suppressions>
    /// ReSharper disable UnusedAutoPropertyAccessor.Local
    /// ReSharper disable AssignNullToNotNullAttribute
    /// </suppressions>
    public class PluginContextTests
    {
        private readonly PluginContext _context;

        public PluginContextTests()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "testPlugins");
            var strategy = new GlobalPluginContextStrategy(path);

            _context = new PluginContext(strategy);
        }

        [Fact]
        public void Inject_PluggablePart_FakePartsImported()
        {
            var pluggablePart = new PluggablePart();

            _context.Inject(pluggablePart);

            Assert.NotNull(pluggablePart.FakeParts);
            Assert.NotEmpty(pluggablePart.FakeParts);
        }

        [Fact]
        public void Load_IFakePart_NotEmpty()
        {
            var parts = _context.Load<IFakePart>();

            Assert.NotEmpty(parts);
        }

        [Fact]
        public void Load_NonExistentLazy_NotNullButEmpty()
        {
            var parts = _context.Load<Lazy<ICloneable>>();

            Assert.NotNull(parts);
            Assert.Empty(parts);
        }
    }
}
