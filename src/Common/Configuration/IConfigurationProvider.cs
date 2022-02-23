﻿//-----------------------------------------------------------------------
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

namespace BadEcho.Configuration;

/// <summary>
/// Defines a provider for a format-neutral source of hot-pluggable configuration data for an application.
/// </summary>
public interface IConfigurationProvider : IConfigurationReader
{
    /// <summary>
    /// Occurs when the configuration data has been externally updated.
    /// </summary>
    event EventHandler<EventArgs>? ConfigurationChanged;
}