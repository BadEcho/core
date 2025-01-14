//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Defines a publisher of intercepted messages.
/// </summary>
/// <typeparam name="TProcedure">The type of procedure (callback function) used to process messages.</typeparam>
public interface IMessageSource<in TProcedure> where TProcedure : Delegate
{
    /// <summary>
    /// Adds a callback function that will receive intercepted messages.
    /// </summary>
    /// <param name="callback">The callback function to invoke when publishing intercepted messages.</param>
    void AddCallback(TProcedure callback);

    /// <summary>
    /// Removes a callback function previously receiving intercepted messages.
    /// </summary>
    /// <param name="callback">The callback function to remove.</param>
    void RemoveCallback(TProcedure callback);
}