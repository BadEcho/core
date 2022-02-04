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

namespace BadEcho.Odin;

/// <summary>
/// Provides a base generic class for classes that contain event data.
/// </summary>
/// <typeparam name="T">The event data type.</typeparam>
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