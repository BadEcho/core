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
    /// Defines a family of plugins that support Odin's Extensibility framework's filterable plugin capabilities, indicating
    /// that types belonging to this family can be isolated from types belonging to different families.
    /// </summary>
    public interface IFilterableFamily
    {
        /// <summary>
        /// Gets the identity of this filterable family.
        /// </summary>
        Guid FamilyId { get; }

        /// <summary>
        /// Gets the name of this filterable family.
        /// </summary>
        string Name { get; }
    }
}
