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
    [Filter(
            typeof(SharedGammaFakePart),
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
