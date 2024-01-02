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

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Specifies the type of custom property attributed to tile-related content.
/// </summary>
internal enum CustomPropertyType
{
    /// <summary>
    /// The custom property contains a text value.
    /// </summary>
    String,
    /// <summary>
    /// The custom property contains a boolean value.
    /// </summary>
    Bool,
    /// <summary> 
    /// The custom property contains a <see cref="Color"/> value represented by a hex code string.
    /// </summary>
    Color,
    /// <summary>
    /// The custom property contains a 32-bit signed integer value.
    /// </summary>
    Int,
    /// <summary>
    /// The custom property contains a floating-point number value.
    /// </summary>
    Float,
    /// <summary>
    /// The custom property contains a relative path referencing a file.
    /// </summary>
    File
}
