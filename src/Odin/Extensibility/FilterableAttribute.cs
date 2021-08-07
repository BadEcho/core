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

using System;
using System.Composition;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides an attribute that specifies that a type, property, field, or method provides a particular filterable
    /// export to Odin's Extensibility framework.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public sealed class FilterableAttribute : ExportAttribute, IFilterableMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterableAttribute"/> class.
        /// </summary>
        /// <param name="familyId">The identity of the filterable family that the part being exported belongs to.</param>
        /// <param name="partType">The concrete type of the part being exported.</param>
        public FilterableAttribute(string familyId, Type partType)
            : base(typeof(IFilterable))
        {
            Require.NotNull(familyId, nameof(familyId));
            Require.NotNull(partType, nameof(partType));
            
            PartType = partType;

            if (!Guid.TryParse(familyId, out Guid parsedId))
            {
                throw new ArgumentException(Strings.FamilyIdNotValid.InvariantFormat(familyId),
                                            nameof(familyId));
            }

            FamilyId = parsedId;
        }
        
        /// <inheritdoc/>
        public Guid FamilyId
        { get; }

        /// <inheritdoc/>
        public Type PartType 
        { get; }
    }
}