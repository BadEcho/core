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
using System.Composition.Convention;
using BadEcho.Extensibility;
using BadEcho.Extensibility.Hosting;
using BadEcho.Extensibility.Tests;

namespace BadEcho.Plugin.Tests;

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

    /// <suppressions>
    /// ReSharper disable UnusedType.Local
    /// </suppressions>
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