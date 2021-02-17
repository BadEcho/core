using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadEcho.Odin.Extensibility.Configuration
{
    interface IContractConfiguration
    {
        string Name { get; }

        IEnumerable<IRoutablePluginConfiguration> RoutablePlugins { get; }
    }
}
