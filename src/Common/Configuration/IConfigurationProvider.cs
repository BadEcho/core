﻿// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

namespace BadEcho.Configuration;

/// <summary>
/// Defines a provider for a format-neutral source of hot-pluggable configuration data.
/// </summary>
public interface IConfigurationProvider : IConfigurationReader
{
    /// <summary>
    /// Occurs when the configuration data has been externally updated.
    /// </summary>
    event EventHandler<EventArgs>? ConfigurationChanged;
}