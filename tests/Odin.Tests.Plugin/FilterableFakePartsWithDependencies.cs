//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Composition;
using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin
{
    [Export(typeof(IFilterableFakeDependency))]
    [Filter(typeof(AlphaFakeDependency),
            FakeIds.AlphaFakeIdValue)]
    public class AlphaFakeDependency : IFilterableFakeDependency
    {
        public Guid TypeIdentifier
            => FakeIds.AlphaFakeId;
    }

    [Export(typeof(IFilterableFakePartWithDependencies))]
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

    [Export(typeof(IFilterableFakePartWithComposedDependencies))]
    [Filter(typeof(AlphaFakePartWithComposedDependencies),
            FakeIds.AlphaFakeIdValue)]
    public class AlphaFakePartWithComposedDependencies : IFilterableFakePartWithComposedDependencies
    {
        private const string DEPENDENCY_CONTRACT = "SuperFakeFilterableDependency";

        [ImportingConstructor]
        public AlphaFakePartWithComposedDependencies([Import(DEPENDENCY_CONTRACT)]IFilterableFakeDependency dependency)
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
        
        [Export(typeof(IConventionProvider))]
        [Filter(typeof(LocalDependencyRegistry), FakeIds.AlphaFakeIdValue)]
        private class LocalDependencyRegistry : DependencyRegistry<IFilterableFakeDependency> , IFilterable
        {
            public LocalDependencyRegistry()
                : base(DEPENDENCY_CONTRACT)
            { }

            public Guid TypeIdentifier => FakeIds.AlphaFakeId;
        }
    }

    [Export(typeof(IFilterableFakePartWithNonFilterableDependencies))]
    [Filter(typeof(AlphaFakePartWithNonFilterableDependencies),
            FakeIds.AlphaFakeIdValue)]
    public class AlphaFakePartWithNonFilterableDependencies : IFilterableFakePartWithNonFilterableDependencies
    {
        private const string DEPENDENCY_CONTRACT = "SuperFakeNonFilterableDependency";

        [ImportingConstructor]
        public AlphaFakePartWithNonFilterableDependencies([Import(DEPENDENCY_CONTRACT)] IFakeDependency dependency)
        {
            Dependency = dependency;
        }

        public IFakeDependency Dependency { get; }

        public Guid TypeIdentifier
            => FakeIds.AlphaFakeId;

        public int DoSomething()
        {
            return 0;
        }

        [Export(typeof(IConventionProvider))]
        [Filter(typeof(LocalDependencyRegistry), FakeIds.AlphaFakeIdValue)]
        private class LocalDependencyRegistry : DependencyRegistry<IFakeDependency>, IFilterable
        {
            public LocalDependencyRegistry()
                : base(DEPENDENCY_CONTRACT)
            { }

            public Guid TypeIdentifier => FakeIds.AlphaFakeId;
        }
    }
}
