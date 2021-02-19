//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
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
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
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
            Require.NotNull(partType, nameof(partType));
            Require.NotNull(familyId, nameof(familyId));
            
            PartType = partType;

            if (!Guid.TryParse(familyId, out Guid parsedIdentifier))
            {
                throw new ArgumentException(Strings.FamilyIdNotValid.InvariantFormat(familyId),
                                            nameof(familyId));
            }

            FamilyId = parsedIdentifier;
        }
        
        /// <inheritdoc/>
        public Guid FamilyId
        { get; }

        /// <inheritdoc/>
        public Type PartType 
        { get; }
    }
}
