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

namespace BadEcho.Game.UI;

/// <summary>
/// Defines a user interface element which supports the measuring and arranging of itself in an allocated layout space.
/// </summary>
/// <remarks>
/// Implementations of this interface are capable of having their measurement and arrangement states invalidated so that they are
/// recalculated during subsequent <c>Measure</c> and <c>Arrange</c> passes, the mechanics of which are implementation details.
/// </remarks>
public interface IArrangeable
{
    /// <summary>
    /// Invalidates the measurement state for the user interface element, which will then be updated during the next
    /// <c>Measure</c> pass.
    /// </summary>
    void InvalidateMeasure();

    /// <summary>
    /// Invalidates the arrangement state for the user interface element, which will then be updated during the next
    /// <c>Arrange</c> pass.
    /// </summary>
    void InvalidateArrange();
}
