using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Odin.Extensibility.Hosting;

namespace BadEcho.Odin.Tests.Extensibility
{
    internal class EmptyPluginContextStrategy : IPluginContextStrategy
    {
        public CompositionHost CreateContainer()
        {
            return new ContainerConfiguration()
                .CreateContainer();
        }
    }
}
