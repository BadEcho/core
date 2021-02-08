using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin
{
    [FilterType(FakeIds.BetaFakeIdValue)]
    public class BetaTypeFakePart : IFilterable
    {
        public Guid TypeIdentifier
            => FakeIds.BetaFakeId;
    }
}
