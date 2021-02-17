//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides a metadata view for a call-routable plugin's metadata.
    /// </summary>
    public sealed class RoutingMetadataView : IRoutingMetadata
    {
        /// <inheritdoc/>
        public Guid PluginIdentifier 
        { get; set; }
    }
}
