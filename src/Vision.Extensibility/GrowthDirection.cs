//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Vision.Extensibility;

/// <summary>
/// Specifies the direction a Vision module grows from its anchor point.
/// </summary>
public enum GrowthDirection
{
    /// <summary>
    /// The module does not grow.
    /// </summary>
    None,
    /// <summary>
    /// The module will grow opposite of its anchor point on the horizontal plane.
    /// </summary>
    Horizontal,
    /// <summary>
    /// The module will grow opposite of its anchor point on the vertical plane.
    /// </summary>
    Vertical
}