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

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Represents the x- and y-coordinates of a point.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    /// <summary>
    /// Specifies the x-coordinate of the point.
    /// </summary>
    public int x;
    /// <summary>
    /// Specifies the y-coordinate of the point.
    /// </summary>
    public int y;
}
