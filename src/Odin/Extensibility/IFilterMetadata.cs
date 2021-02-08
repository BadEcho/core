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
    /// Defines metadata that describes a filterable export to Odin's Extensibility framework.
    /// </summary>
    public interface IFilterMetadata
    {
        /// <summary>
        /// Gets the concrete type of the part exported.
        /// </summary>
        Type? PartType { get; }

        /// <summary>
        /// Gets the type identifier of the part being exported.
        /// </summary>
        string? TypeIdentifier { get; }
    }
}
