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

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Represents the coordinates of the upper-left and lower-right corners of a rectangle.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct RECT
{
    /// <summary>
    /// The x-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int Left;

    /// <summary>
    /// The y-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int Top;

    /// <summary>
    /// The x-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int Right;

    /// <summary>
    /// The y-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int Bottom;

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public readonly int Height
        => Bottom - Top;

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public readonly int Width
        => Right - Left;
}