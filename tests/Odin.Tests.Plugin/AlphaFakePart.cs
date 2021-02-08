using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin
{
    [Filter(typeof(AlphaFakePart), 
            FakeIds.AlphaFakeIdValue)]
    public class AlphaFakePart : IFilterableFakePart
    {
        public Guid TypeIdentifier
            => FakeIds.AlphaFakeId;

        public int DoSomething()
        {
            return 54;
        }
    }
}
