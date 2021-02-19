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
    [Filterable(AlphaFamily.FamilyIdValue, typeof(AlphaFakeDependency))]
    public class AlphaFakeDependency : IFilterableFakeDependency
    {
        public Guid FamilyId
            => new(AlphaFamily.FamilyIdValue);
    }

    [Export(typeof(IFilterableFakePartWithDependencies))]
    [Filterable(AlphaFamily.FamilyIdValue, typeof(AlphaFakePartWithDependencies))]
    public class AlphaFakePartWithDependencies : IFilterableFakePartWithDependencies
    {
        [ImportingConstructor]
        public AlphaFakePartWithDependencies(IFilterableFakeDependency dependency)
        {
            Dependency = dependency;
        }

        public IFilterableFakeDependency Dependency { get; }

        public Guid FamilyId
            => new(AlphaFamily.FamilyIdValue);

        public int DoSomething()
        {
            return 0;
        }
    }

    [Export(typeof(IFilterableFakePartWithComposedDependencies))]
    [Filterable(AlphaFamily.FamilyIdValue, typeof(AlphaFakePartWithComposedDependencies))]
    public class AlphaFakePartWithComposedDependencies : IFilterableFakePartWithComposedDependencies
    {
        private const string DEPENDENCY_CONTRACT = "SuperFakeFilterableDependency";

        [ImportingConstructor]
        public AlphaFakePartWithComposedDependencies([Import(DEPENDENCY_CONTRACT)]IFilterableFakeDependency dependency)
        {
            Dependency = dependency;
        }

        public IFilterableFakeDependency Dependency { get; }

        public Guid FamilyId
            => new(AlphaFamily.FamilyIdValue);

        public int DoSomething()
        {
            return 0;
        }
        
        [Export(typeof(IConventionProvider))]
        [Filterable(AlphaFamily.FamilyIdValue, typeof(LocalDependencyRegistry))]
        private class LocalDependencyRegistry : DependencyRegistry<IFilterableFakeDependency> , IFilterable
        {
            public LocalDependencyRegistry()
                : base(DEPENDENCY_CONTRACT)
            { }

            public Guid FamilyId => new(AlphaFamily.FamilyIdValue);
        }
    }

    [Export(typeof(IFilterableFakePartWithNonFilterableDependencies))]
    [Filterable(AlphaFamily.FamilyIdValue, typeof(AlphaFakePartWithNonFilterableDependencies))]
    public class AlphaFakePartWithNonFilterableDependencies : IFilterableFakePartWithNonFilterableDependencies
    {
        private const string DEPENDENCY_CONTRACT = "SuperFakeNonFilterableDependency";

        [ImportingConstructor]
        public AlphaFakePartWithNonFilterableDependencies([Import(DEPENDENCY_CONTRACT)] IFakeDependency dependency)
        {
            Dependency = dependency;
        }

        public IFakeDependency Dependency { get; }

        public Guid FamilyId
            => new(AlphaFamily.FamilyIdValue);

        public int DoSomething()
        {
            return 0;
        }

        [Export(typeof(IConventionProvider))]
        [Filterable(AlphaFamily.FamilyIdValue, typeof(LocalDependencyRegistry))]
        private class LocalDependencyRegistry : DependencyRegistry<IFakeDependency>, IFilterable
        {
            public LocalDependencyRegistry()
                : base(DEPENDENCY_CONTRACT)
            { }

            public Guid FamilyId => new(AlphaFamily.FamilyIdValue);
        }
    }
}
