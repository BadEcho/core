//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Odin
{
    /// <summary>
    /// Provides a base generic class for classes that contain event data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
        /// </summary>
        /// <param name="data">The event data.</param>
        public EventArgs(T data) 
            => Data = data;

        /// <summary>
        /// Gets the event data.
        /// </summary>
        public T Data
        { get; }
    }
}