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

using Microsoft.Xna.Framework;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a container for custom properties attributed to tile-related content.
/// </summary>
public sealed class CustomProperties
{
    /// <summary>
    /// Gets a mapping between the name of the content's custom string properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, string> Strings
    { get; init; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets a mapping between the name of the content's custom bool properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, bool> Booleans
    { get; init; } = new Dictionary<string, bool>();

    /// <summary>
    /// Gets a mapping between the name of the content's custom color properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, Color> Colors
    { get; init; } = new Dictionary<string, Color>();

    /// <summary>
    /// Gets a mapping between the name of the content's custom integer properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, int> Integers
    { get; init; } = new Dictionary<string, int>();

    /// <summary>
    /// Gets a mapping between the name of the content's custom floating-point number properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, float> Floats
    { get; init; } = new Dictionary<string, float>();

    /// <summary>
    /// Gets a mapping between the name of the content's custom file properties and their values.
    /// </summary>
    public IReadOnlyDictionary<string, FileInfo> Files
    { get; init; } = new Dictionary<string, FileInfo>();
}
