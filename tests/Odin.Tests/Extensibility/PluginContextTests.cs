//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using BadEcho.Odin.Extensibility.Hosting;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility
{
    public class PluginContextTests
    {
        private readonly PluginContext _context;

        public PluginContextTests()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "plugins");
            var strategy = new GlobalPluginContextStrategy(path);

            _context = new PluginContext(strategy);
        }

        [Fact]
        public void PluggablePart_Inject()
        {
            var pluggablePart = new PluggablePart();

            _context.Inject(pluggablePart);

            Assert.NotNull(pluggablePart.FakeParts);
        }

        [Fact]
        public void IFakePart_Load()
        {
            var parts = _context.Load<IFakePart>();

            Assert.NotEmpty(parts);
        }
        
        private sealed class PluggablePart
        {
            [ImportMany] 
            public IEnumerable<IFakePart>? FakeParts 
            { get; set; }
        }
    }
}
