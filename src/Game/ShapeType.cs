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

namespace BadEcho.Game;

/// <summary>
/// Specifies the type of shape that defines an external boundary.
/// </summary>
/// <remarks>
/// This is used solely for the purpose of expressing type identity during pipeline serialization, as only simple values may
/// be transmitted.
/// </remarks>
public enum ShapeType
{
    /// <summary>
    /// The boundary shape is rectangular with the dimensions identical to that of the texture's region.
    /// </summary>
    RectangleSource,
    /// <summary>
    /// The boundary shape is rectangular with its dimensions provided as content processor parameters.
    /// </summary>
    RectangleCustom,
    /// <summary>
    /// The boundary shape is circular with its dimensions provided as content processor parameters.
    /// </summary>
    CircleCustom
}
