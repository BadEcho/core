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
    /// Provides an attribute that specifies that a class defines a type or family of filterable exports to Odin's
    /// Extensibility framework.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FilterTypeAttribute : ExportAttribute, IFilterMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterTypeAttribute"/> class.
        /// </summary>
        /// <param name="typeIdentifier">The type identifier for exported parts belonging to the family.</param>
        public FilterTypeAttribute(string typeIdentifier)
            : base(typeof(IFilterable))
        {
            Require.NotNull(typeIdentifier, nameof(typeIdentifier));

            TypeIdentifier = typeIdentifier;
        }
        
        /// <inheritdoc/>
        public Type PartType
            => typeof(IFilterable);

        /// <inheritdoc/>
        public string TypeIdentifier 
        { get; }
    }
}
