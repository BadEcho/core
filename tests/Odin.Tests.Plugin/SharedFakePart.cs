//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;
using System.Composition.Convention;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Tests.Extensibility;

namespace BadEcho.Odin.Tests.Plugin
{
    [Export(typeof(ISharedFakePart))]
    public sealed class SharedFakePart : ISharedFakePart
    {
        public int DoSomething()
        {
            return -1;
        }

        [Export(typeof(IConventionProvider))]
        private sealed class SharedConventionProvider : IConventionProvider
        {
            public void ConfigureRules(ConventionBuilder conventions)
            {
                conventions.ForTypesDerivedFrom<ISharedFakePart>()
                           .Shared();
            }
        }
    }
}
