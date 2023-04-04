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
