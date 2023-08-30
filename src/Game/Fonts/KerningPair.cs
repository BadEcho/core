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

namespace BadEcho.Game.Fonts;

/// <summary>
/// Provides the adjustment of space between two specific glyphs.
/// TODO: finish.
/// </summary>
public class KerningPair
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KerningPair"/> class.
    /// </summary>
    /// <param name="firstCharacter">The first unicode character in the pair.</param>
    /// <param name="secondCharacter">The second unicode character in the pair.</param>
    public KerningPair(char firstCharacter, char secondCharacter)
    {
        FirstCharacter = firstCharacter;
        SecondCharacter = secondCharacter;
    }

    /// <summary>
    /// Gets the first unicode character in the pair.
    /// </summary>
    public char FirstCharacter
    { get; }

    /// <summary>
    /// Gets the second unicode character in the pair.
    /// </summary>
    public char SecondCharacter
    { get; }
}
