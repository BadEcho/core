//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin
{
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
}
