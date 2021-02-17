//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Composition;
using System.Composition.Convention;
using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin
{
    [Export(typeof(IFilterableFakePart))]
    [Filter(typeof(AlphaFakePart),
            FakeIds.AlphaFakeIdValue)]
    public class AlphaFakePart : IFilterableFakePart
    {
        public Guid TypeIdentifier
            => FakeIds.AlphaFakeId;

        public int DoSomething() 
            => 54;
    }

    [Export(typeof(IFilterableFakePart))]
    [Filter(typeof(BetaFakePart),
            FakeIds.BetaFakeIdValue)]
    public class BetaFakePart : IFilterableFakePart
    {
        public Guid TypeIdentifier
            => FakeIds.BetaFakeId;

        public int DoSomething() 
            => 29290892;
    }

    [Export(typeof(IFilterableFakePart))]
    [Filter(typeof(DeltaFakePart),
            FakeIds.DeltaFakeIdValue)]
    public class DeltaFakePart : IFilterableFakePart
    {
        public Guid TypeIdentifier
            => FakeIds.DeltaFakeId;

        public int DoSomething() 
            => -1;
    }

    [Export(typeof(IFilterableFakePart))]
    [Filter(typeof(SharedGammaFakePart),
            FakeIds.GammaFakeIdValue)]
    public sealed class SharedGammaFakePart : IFilterableFakePart
    {
        public int DoSomething() 
            => 0;

        public Guid TypeIdentifier
            => FakeIds.GammaFakeId;

        [Export(typeof(IConventionProvider))]
        private sealed class SharedConventionProvider : IConventionProvider
        {
            public void ConfigureRules(ConventionBuilder conventions)
            {
                conventions.ForType<SharedGammaFakePart>()
                           .Shared();
            }
        }
    }
}
