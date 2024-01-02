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

namespace BadEcho.Tests.Extensibility;

public static class FakeAdapterIds
{
    public const string AlphaFakeIdValue = "BE157C54-2BC0-48B8-9E39-0AE53D8A4E61";
    public static Guid AlphaFakeId
        => new(AlphaFakeIdValue);

    public const string BetaFakeIdValue = "F544EC74-919E-4167-A421-FA74223F49C5";
    public static Guid BetaFakeId
        => new(BetaFakeIdValue);
}

public interface ISegmentedContract
{
    const string FirstSomeMethod = "First";
    const string FirstSomeOtherMethod = "StillFirst";
    const string SecondSomeMethod = "Second";
    const string SecondSomeOtherMethod = "StillSecond";

    string SomeMethod();

    string SomeOtherMethod();
}