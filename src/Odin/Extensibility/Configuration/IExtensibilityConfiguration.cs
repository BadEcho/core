using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadEcho.Odin.Extensibility.Configuration
{
    interface IExtensibilityConfiguration
    {
        string PluginDirectory { get; }

        ICollection<IContractConfiguration> SegmentedContracts { get; }
    }
}
