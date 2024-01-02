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

using BadEcho.Extensions;

namespace BadEcho.Game.Fonts;

/// <summary>
/// Provides a pair of characters.
/// </summary>
public class CharacterPair
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterPair"/> class.
    /// </summary>
    /// <param name="firstCharacter">The first unicode in the pair.</param>
    /// <param name="secondCharacter">The second unicode in the pair.</param>
    public CharacterPair(char firstCharacter, char secondCharacter)
    {
        FirstCharacter = firstCharacter;
        SecondCharacter = secondCharacter;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterPair"/> class.
    /// </summary>
    /// <remarks>Required for content pipeline deserialization.</remarks>
    protected CharacterPair()
    { }

    /// <summary>
    /// Gets the first unicode character in the pair.
    /// </summary>
    public char FirstCharacter
    { get; init; }

    /// <summary>
    /// Gets the second unicode character in the pair.
    /// </summary>
    public char SecondCharacter
    { get; init; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not CharacterPair other)
            return false;

        return FirstCharacter == other.FirstCharacter
               && SecondCharacter == other.SecondCharacter;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(FirstCharacter, SecondCharacter);
}
