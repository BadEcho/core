//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Defines a connection point between a call-routable plugin and a <see cref="IHostAdapter"/>, allowing for
    /// the plugin to effectively segment a common contract it implements.
    /// </summary>
    /// <typeparam name="T">The type of contract that is segmented by the call-routable plugin.</typeparam>
    public interface IPluginAdapter<out T>
    {
        /// <summary>
        /// Gets the identity of the call-routable plugin.
        /// </summary>
        Guid PluginIdentifier { get; }

        /// <summary>
        /// Gets the <typeparamref name="T"/> implemented by the call-routable plugin.
        /// </summary>
        T Contract { get; }
    }
}
