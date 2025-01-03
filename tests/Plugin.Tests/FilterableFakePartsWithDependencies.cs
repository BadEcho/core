//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;
using BadEcho.Extensibility;
using BadEcho.Extensibility.Hosting;
using BadEcho.Extensibility.Tests;

namespace BadEcho.Plugin.Tests;

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
    private const string DEPENDENCY_CONTRACT 
        = nameof(AlphaFakePartWithComposedDependencies) + nameof(LocalDependency);

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
    [Filterable(AlphaFamily.FamilyIdValue, typeof(LocalDependency))]
    private class LocalDependency : DependencyRegistry<IFilterableFakeDependency> , IFilterable
    {
        public LocalDependency()
            : base(DEPENDENCY_CONTRACT)
        { }

        /// <inheritdoc />
        public override IFilterableFakeDependency Dependency
            => LoadDependency();

        public Guid FamilyId => new(AlphaFamily.FamilyIdValue);
    }
}

[Export(typeof(IFilterableFakePartWithNonFilterableDependencies))]
[Filterable(AlphaFamily.FamilyIdValue, typeof(AlphaFakePartWithNonFilterableDependencies))]
public class AlphaFakePartWithNonFilterableDependencies : IFilterableFakePartWithNonFilterableDependencies
{
    private const string DEPENDENCY_CONTRACT 
        = nameof(AlphaFakePartWithNonFilterableDependencies) + nameof(LocalDependency);

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
    [Filterable(AlphaFamily.FamilyIdValue, typeof(LocalDependency))]
    private class LocalDependency : DependencyRegistry<IFakeDependency>, IFilterable
    {
        public LocalDependency()
            : base(DEPENDENCY_CONTRACT)
        { }

        /// <inheritdoc />
        public override IFakeDependency Dependency
            => LoadDependency();

        public Guid FamilyId => new(AlphaFamily.FamilyIdValue);
    }
}