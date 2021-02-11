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

    [Filter(typeof(AlphaFakeDependency), FakeIds.AlphaFakeIdValue)]
    public class AlphaFakeDependency : IFilterableFakeDependency
    {
        public Guid TypeIdentifier => FakeIds.AlphaFakeId;
    }

    [Filter(typeof(AlphaFakePartWithDependencies),
            FakeIds.AlphaFakeIdValue)]
    public class AlphaFakePartWithDependencies : IFilterableFakePartWithDependencies
    {
        [ImportingConstructor]
        public AlphaFakePartWithDependencies(IFilterableFakeDependency dependency)
        {
            Dependency = dependency;
        }

        public IFilterableFakeDependency Dependency { get; }

        public Guid TypeIdentifier
            => FakeIds.AlphaFakeId;

        public int DoSomething()
        {
            return 0;
        }
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
    
    [Filter(typeof(SharedGammaFakePart),
            FakeIds.GammaFakeIdValue)]
    public sealed class SharedGammaFakePart : IFilterableFakePart
    {
        public int DoSomething()
        {
            return 0;
        }

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
