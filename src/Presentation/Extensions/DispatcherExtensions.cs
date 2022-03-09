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

using System.Windows.Threading;

namespace BadEcho.Presentation.Extensions;

/// <summary>
/// Provides a set of static methods intended to simplify the use of <see cref="Dispatcher"/> related types.
/// </summary>
public static class DispatcherExtensions
{
    /// <summary>
    /// Executes the specified action action synchronously, at the specified priority, on the thread that the
    /// current object's <see cref="Dispatcher"/> is running on.
    /// </summary>
    /// <param name="dispatcherObject">The current object that is associated with a <see cref="Dispatcher"/>.</param>
    /// <param name="method">The action to invoke through the <see cref="Dispatcher"/>.</param>
    /// <param name="priority">
    /// The priority that determines in what order the specified callback is invoked relative to the other pending
    /// operations in the <see cref="Dispatcher"/>.
    /// </param>
    public static void Invoke(this DispatcherObject dispatcherObject, Action method, DispatcherPriority priority)
    {
        Require.NotNull(dispatcherObject, nameof(dispatcherObject));
        Require.NotNull(method, nameof(method));

        dispatcherObject.Dispatcher.Invoke(method, priority);
    }

    /// <summary>
    /// Processes all messages currently queued in the message queue of the current object's <see cref="Dispatcher"/>.
    /// </summary>
    /// <param name="dispatcherObject">The current object that is associated with a <see cref="Dispatcher"/>.</param>
    public static void ProcessMessages(this DispatcherObject dispatcherObject) 
        => dispatcherObject.Invoke(() => { }, DispatcherPriority.Background);

    /// <summary>
    /// Processes all messages currently queued in the dispatcher's message queue.
    /// </summary>
    /// <param name="dispatcher">The dispatcher to process messages for.</param>
    public static void ProcessMessages(this Dispatcher dispatcher)
    {
        Require.NotNull(dispatcher, nameof(dispatcher));

        dispatcher.Invoke(() => { }, DispatcherPriority.Background);
    }
}