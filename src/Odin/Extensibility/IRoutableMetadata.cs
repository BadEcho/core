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
    /// Defines metadata that describes a call-routable plugin to Odin's Extensibility framework.
    /// </summary>
    public interface IRoutableMetadata
    {
        /// <summary>
        /// Gets the identity of the call-routable plugin being exported.
        /// </summary>
        Guid PluginId { get; }
    }
}
