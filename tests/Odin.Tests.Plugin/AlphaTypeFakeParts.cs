//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Composition;
using BadEcho.Odin.Extensibility;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin
{
    [FilterType(FakeIds.AlphaFakeIdValue)]
    public class AlphaTypeFakePart : IFilterable
    {
        public Guid TypeIdentifier
            => FakeIds.AlphaFakeId;
    }

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
            throw new NotImplementedException();
        }
    }
}
