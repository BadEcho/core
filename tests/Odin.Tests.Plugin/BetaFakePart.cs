using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin
{
    [Filter(
            typeof(BetaFakePart),
            FakeIds.BetaFakeIdValue)]
    public class BetaFakePart : IFilterableFakePart
    {
        public Guid TypeIdentifier
            => FakeIds.BetaFakeId;

        public int DoSomething()
        {
            return 29290892;
        }
    }
}
