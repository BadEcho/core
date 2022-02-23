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

namespace BadEcho.Extensibility;

/// <summary>
/// Defines metadata that describes a call-routable plugin to the Bad Echo Extensibility framework.
/// </summary>
public interface IRoutableMetadata
{
    /// <summary>
    /// Gets the identity of the call-routable plugin being exported.
    /// </summary>
    Guid PluginId { get; }
}