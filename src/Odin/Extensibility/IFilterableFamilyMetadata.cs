//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

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
