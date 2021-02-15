//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Composition;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides an attribute that specifies that a type, property, field, or method provides a particular filterable
    /// export to Odin's Extensibility framework.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class FilterAttribute : ExportAttribute, IFilterMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterAttribute"/> class.
        /// </summary>
        /// <param name="partType">The concrete type of the part being exported.</param>
        /// <param name="typeIdentifier">The type identifier of the part being exported.</param>
        public FilterAttribute(Type partType, string typeIdentifier)
            : base(typeof(IFilterable))
        {
            Require.NotNull(partType, nameof(partType));
            Require.NotNull(typeIdentifier, nameof(typeIdentifier));

            PartType = partType;
            TypeIdentifier = typeIdentifier;
        }

        /// <inheritdoc/>
        public Type PartType 
        { get; }

        /// <inheritdoc/>
        public string TypeIdentifier 
        { get; }
    }
}
