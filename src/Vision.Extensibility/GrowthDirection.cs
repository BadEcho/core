//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Omnified.Vision.Extensibility;

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