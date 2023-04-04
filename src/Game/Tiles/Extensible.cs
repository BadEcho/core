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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides tile-related content that supports custom property attribution.
/// </summary>
public abstract class Extensible
{
    /// <summary>
    /// Gets a mapping between the names of the content's custom properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string,string> CustomProperties
    { get; init; } = new Dictionary<string,string>();
}
