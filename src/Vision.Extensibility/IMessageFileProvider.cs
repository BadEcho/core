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
/// Defines a provider of a Vision module's message file.
/// </summary>
public interface IMessageFileProvider
{
    /// <summary>
    /// Occurs when new messages are available for processing.
    /// </summary>
    event EventHandler<EventArgs<string>>? NewMessages;

    /// <summary>
    /// Gets the complete set of messages that have posted to the message file.
    /// </summary>
    string? CurrentMessages { get; }
}