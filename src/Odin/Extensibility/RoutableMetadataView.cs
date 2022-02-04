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

namespace BadEcho.Odin.Extensibility;

/// <summary>
/// Provides a metadata view for a call-routable plugin's metadata.
/// </summary>
public sealed class RoutableMetadataView : IRoutableMetadata
{
    /// <inheritdoc/>
    public Guid PluginId 
    { get; set; }
}