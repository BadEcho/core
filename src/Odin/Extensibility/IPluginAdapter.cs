﻿//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Defines a connection point between a call-routable plugin and a host, allowing for the plugin to effectively
    /// segment a common contract it implements.
    /// </summary>
    /// <typeparam name="T">The type of contract that is segmented by the call-routable plugin.</typeparam>
    public interface IPluginAdapter<out T>
    {
        /// <summary>
        /// Gets the <typeparamref name="T"/> implemented by the call-routable plugin.
        /// </summary>
        T Contract { get; }
    }
}