//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho;

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