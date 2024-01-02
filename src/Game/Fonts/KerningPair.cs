//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
/// Provides the adjustment of space between two specific character glyphs.
/// </summary>
/// <suppressions>
/// ReSharper disable UnusedMember.Local
/// </suppressions>
public sealed class KerningPair : CharacterPair
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KerningPair"/> class.
    /// </summary>
    /// <param name="firstCharacter">The first unicode character in the pair.</param>
    /// <param name="secondCharacter">The second unicode character in the pair.</param>
    /// <param name="advance">An adjustment to the advance width when rendering the character pair.</param>
    public KerningPair(char firstCharacter, char secondCharacter, float advance)
        : base(firstCharacter, secondCharacter)
    {
        Advance = advance;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KerningPair"/> class.
    /// </summary>
    /// <remarks>Required for content pipeline deserialization.</remarks>
    private KerningPair()
    { }

    /// <summary>
    /// Gets an adjustment to the advance width when rendering the character pair.
    /// </summary>
    /// <remarks>
    /// This value is added to the <see cref="FontGlyph.Advance"/> value of the first character glyph when calculating
    /// its advance width, resulting in a (hopefully) finely tuned amount of spacing between the two characters.
    /// </remarks>
    public float Advance
    { get; init; }
}
