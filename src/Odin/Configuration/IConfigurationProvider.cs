//-----------------------------------------------------------------------
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

using System;

namespace BadEcho.Odin.Configuration
{
    /// <summary>
    /// Defines a format-neutral source for hot-pluggable configuration data.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Occurs when the configuration has been externally updated.
        /// </summary>
        event EventHandler<EventArgs>? ConfigurationChanged;

        /// <summary>
        /// Gets the application's configuration in a particular sectional form.
        /// </summary>
        /// <typeparam name="T">The type of object to parse the configuration as.</typeparam>
        /// <returns>A <typeparamref name="T"/> instance reflecting the application's configuration.</returns>
        T GetConfiguration<T>() where T: new();
    }
}
