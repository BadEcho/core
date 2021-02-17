using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadEcho.Odin.Extensibility.Configuration
{
    interface IRoutablePluginConfiguration
    {
        Guid Id { get; }

        bool Primary { get; }

        IEnumerable<string> MethodClaims { get; }
    }
}
