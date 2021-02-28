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
    /// Provides an attribute that specifies that a class defines a family of filterable exports to Odin's Extensibility
    /// framework.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class FilterableFamilyAttribute : ExportAttribute, IFilterableFamilyMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterableFamilyAttribute"/> class.
        /// </summary>
        /// <param name="familyId">The identity of the filterable family being defined.</param>
        /// <param name="name">The name of the filterable family being defined.</param>
        public FilterableFamilyAttribute(string familyId, string name)
            : base(typeof(IFilterableFamily))
        {
            Require.NotNullOrEmpty(name, nameof(name));
            Require.NotNull(familyId, nameof(familyId));

            if (!Guid.TryParse(familyId, out Guid parsedId))
            {
                throw new ArgumentException(Strings.FamilyIdNotValid.InvariantFormat(familyId),
                                            nameof(familyId));
            }

            FamilyId = parsedId;
            Name = name;
        }

        /// <inheritdoc/>
        public Guid FamilyId
        { get; }

        /// <inheritdoc/>
        public string Name 
        { get; }
    }
}