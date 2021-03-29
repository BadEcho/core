//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Threading;
using BadEcho.Odin;

namespace BadEcho.Fenestra.Extensions
{
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
        /// Process all messages currently queued in the message queue of the current object's <see cref="Dispatcher"/>.
        /// </summary>
        /// <param name="dispatcherObject">The current object that is associated with a <see cref="Dispatcher"/>.</param>
        public static void ProcessMessages(this DispatcherObject dispatcherObject) 
            => dispatcherObject.Invoke(() => { }, DispatcherPriority.Background);
    }
}
