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
    [FilterType(FakeIds.AlphaFakeIdValue)]
    public class AlphaFakePartType : IFilterable
    {
        public Guid TypeIdentifier
            => FakeIds.AlphaFakeId;
    }

    [FilterType(FakeIds.BetaFakeIdValue)]
    public class BetaFakePartType : IFilterable
    {
        public Guid TypeIdentifier
            => FakeIds.BetaFakeId;
    }

    [FilterType(FakeIds.DeltaFakeIdValue)]
    public class DeltaFakePartType : IFilterable
    {
        public Guid TypeIdentifier
            => FakeIds.DeltaFakeId;
    }

    [FilterType(FakeIds.GammaFakeIdValue)]
    public class GammaFakePartType : IFilterable
    {
        public Guid TypeIdentifier
            => FakeIds.GammaFakeId;
    }
}
