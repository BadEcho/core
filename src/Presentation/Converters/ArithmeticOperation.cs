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

namespace BadEcho.Presentation.Converters;

/// <summary>
/// Specifies a type of arithmetic operation.
/// </summary>
public enum ArithmeticOperation
{
    /// <summary>
    /// An addition operation.
    /// </summary>
    Addition,
    /// <summary>
    /// A subtraction operation.
    /// </summary>
    Subtraction,
    /// <summary>
    /// A multiplication operation.
    /// </summary>
    Multiplication,
    /// <summary>
    /// A division operation.
    /// </summary>
    Division
}
