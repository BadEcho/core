using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Odin.Extensibility.Hosting;
using Xunit;

namespace BadEcho.Odin.Tests.Extensibility
{
    public class HostAdapterTests
    {
        public void NoPlugins_Initialize()
        {
            var strategy = new EmptyPluginContextStrategy();
            var context = new PluginContext(strategy);

            var hostAdapter = new HostAdapter<IFakePart>(context);
        }
     
    }
}
