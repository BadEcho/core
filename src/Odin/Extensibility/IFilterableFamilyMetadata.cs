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
    /// Defines metadata that describes a filterable family of plugins used by Odin's Extensibility framework to
    /// isolate plugins from each other.
    /// </summary>
    public interface IFilterableFamilyMetadata
    {
        /// <summary>
        /// Gets the identity of the filterable family.
        /// </summary>
        Guid FamilyId { get; }

        /// <summary>
        /// Gets the name of the filterable family.
        /// </summary>
        string Name { get; }
    }
}
