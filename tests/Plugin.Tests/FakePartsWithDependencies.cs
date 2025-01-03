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
using BadEcho.Extensibility.Hosting;
using BadEcho.Extensibility.Tests;

namespace BadEcho.Plugin.Tests;

[Export(typeof(IFakePartWithDependencies))]
public class FakePartWithDependencies : IFakePartWithDependencies
{
    [ImportingConstructor]
    public FakePartWithDependencies(IFakeDependency dependency)
    {
        Dependency = dependency;
    }

    public IFakeDependency Dependency { get; }
}

[Export(typeof(IFakePartWithComposedDependencies))]
public class FakePartWithComposedDependencies : IFakePartWithComposedDependencies
{
    private const string DEPENDENCY_CONTRACT 
        = nameof(FakePartWithComposedDependencies) + nameof(LocalDependency);

    [ImportingConstructor]
    public FakePartWithComposedDependencies([Import(DEPENDENCY_CONTRACT)] IFakeDependency dependency)
    {
        Dependency = dependency;
    }

    public IFakeDependency Dependency { get; }

    /// <suppressions>
    /// ReSharper disable ClassNeverInstantiated.Local
    /// </suppressions>
    [Export(typeof(IConventionProvider))]
    private class LocalDependency : DependencyRegistry<IFakeDependency>
    {
        public LocalDependency()
            : base(DEPENDENCY_CONTRACT)
        { }

        /// <inheritdoc />
        public override IFakeDependency Dependency
            => LoadDependency();
    }
}