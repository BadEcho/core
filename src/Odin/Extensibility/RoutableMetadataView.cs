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
    public sealed class RoutableMetadataView : IRoutableMetadata
    {
        /// <inheritdoc/>
        public Guid PluginId 
        { get; set; }
    }
}
