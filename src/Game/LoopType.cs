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

namespace BadEcho.Game;

/// <summary>
/// Specifies the type of looping behavior for a sequence.
/// </summary>
public enum LoopType
{
    /// <summary>
    /// The sequence does not repeat.
    /// </summary>
    None,
    /// <summary>
    /// The sequence is executed from beginning to end and then repeated from the beginning.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A sequence of elements <c>[A. B, C, D]</c> executed in a circular loop has the following execution order:
    /// </para>
    /// <para><c>A -> B -> C -> D -> A -> B -> ...</c></para>
    /// </remarks>
    Circular,
    /// <summary>
    /// The sequence is executed from beginning to end and then from end to beginning repeatedly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A sequence of elements <c>[A, B, C, D]</c> executed in a ping pong loop has the following execution order:
    /// </para>
    /// <para><c>A -> B -> C -> D -> C -> B -> A -> B -> ...</c></para>
    /// </remarks>
    PingPong
}
