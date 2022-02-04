//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Fenestra;

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