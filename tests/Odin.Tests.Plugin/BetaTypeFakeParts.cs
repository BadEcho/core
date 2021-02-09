//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
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

    [Filter(typeof(BetaFakePart),
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
