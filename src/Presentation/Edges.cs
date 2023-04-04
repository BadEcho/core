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

namespace BadEcho.Presentation;

/// <summary>
/// Specifies edges of a frame around a rectangle.
/// </summary>
[Flags]
public enum Edges
{
    /// <summary>
    /// The left edge of the frame.
    /// </summary>
    Left = 0x1,
    /// <summary>
    /// The right edge of the frame.
    /// </summary>
    Right = 0x2,
    /// <summary>
    /// The top edge of the frame.
    /// </summary>
    Top = 0x4,
    /// <summary>
    /// The bottom edge of the frame.
    /// </summary>
    Bottom = 0x8
}