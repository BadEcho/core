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
    [Filterable(AlphaFamily.FamilyIdValue, typeof(AlphaFakePart))]
    public class AlphaFakePart : IFilterableFakePart
    {
        public Guid FamilyId
            => new(AlphaFamily.FamilyIdValue);

        public int DoSomething() 
            => 54;
    }

    [Export(typeof(IFilterableFakePart))]
    [Filterable(BetaFamily.FamilyIdValue, typeof(BetaFakePart))]
    public class BetaFakePart : IFilterableFakePart
    {
        public Guid FamilyId
            => new(BetaFamily.FamilyIdValue);

        public int DoSomething() 
            => 29290892;
    }

    [Export(typeof(IFilterableFakePart))]
    [Filterable(DeltaFamily.FamilyIdValue, typeof(DeltaFakePart))]
    public class DeltaFakePart : IFilterableFakePart
    {
        public Guid FamilyId
            => new(DeltaFamily.FamilyIdValue);

        public int DoSomething() 
            => -1;
    }

    [Export(typeof(IFilterableFakePart))]
    [Filterable(GammaFamily.FamilyIdValue, typeof(SharedGammaFakePart))]
    public sealed class SharedGammaFakePart : IFilterableFakePart
    {
        public int DoSomething() 
            => 0;

        public Guid FamilyId
            => new(GammaFamily.FamilyIdValue);

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
