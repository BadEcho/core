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

using BadEcho.Extensibility;
using BadEcho.Extensibility.Tests;

namespace BadEcho.Plugin.Tests;

[FilterableFamily(FamilyIdValue, NAME)]
public class AlphaFamily : IFilterableFamily
{
    internal const string FamilyIdValue = FakeFilterableIds.AlphaFakeIdValue;
    private const string NAME = "Alpha";

    public Guid FamilyId
        => new(FamilyIdValue);

    public string Name
        => NAME;
}

[FilterableFamily(FamilyIdValue, NAME)]
public class BetaFamily : IFilterableFamily
{
    internal const string FamilyIdValue = FakeFilterableIds.BetaFakeIdValue;
    private const string NAME = "Beta";

    public Guid FamilyId
        => new (FamilyIdValue);

    public string Name
        => NAME;
}

[FilterableFamily(FamilyIdValue, NAME)]
public class DeltaFamily : IFilterableFamily
{
    internal const string FamilyIdValue = FakeFilterableIds.DeltaFakeIdValue;
    private const string NAME = "Delta";

    public Guid FamilyId
        => new(FamilyIdValue);

    public string Name
        => NAME;
}

[FilterableFamily(FamilyIdValue, NAME)]
public class GammaFamily : IFilterableFamily
{
    internal const string FamilyIdValue = FakeFilterableIds.GammaFakeIdValue;
    private const string NAME = "Gamma";

    public Guid FamilyId
        => new(FamilyIdValue);

    public string Name
        => NAME;
}