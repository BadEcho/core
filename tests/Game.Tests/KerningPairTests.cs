//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Game.Fonts;
using Xunit;

namespace BadEcho.Game.Tests;

public class KerningPairTests
{
    [Fact]
    public void GetHashCode_SimilarPairs_ReturnsUnique()
    {
        var abPair = new CharacterPair('a', 'b');
        var baPair = new CharacterPair('b', 'a');
        var lvPair = new CharacterPair('l', 'v');
        var ptPair = new CharacterPair('p', 't');

        // Different order.
        Assert.NotEqual(abPair.GetHashCode(), baPair.GetHashCode());
        // Equal sum of code values.
        Assert.NotEqual(lvPair.GetHashCode(), ptPair.GetHashCode());
    }
}
