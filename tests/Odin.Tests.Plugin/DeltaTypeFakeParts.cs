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
    [FilterType(FakeIds.DeltaFakeIdValue)]
    public class DeltaTypeFakePart : IFilterable
    { 
        public Guid TypeIdentifier
            => FakeIds.DeltaFakeId;
    }

    [Filter(typeof(DeltaFakePart),
            FakeIds.DeltaFakeIdValue)]
    public class DeltaFakePart : IFilterableFakePart
    {
        public Guid TypeIdentifier
            => FakeIds.DeltaFakeId;

        public int DoSomething()
        {
            return -1;
        }
    }
}
