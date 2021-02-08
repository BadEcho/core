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
    /// Provides an attribute that specifies that a type, property, field, or method provides a particular filterable
    /// export to Odin's Extensibility framework.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class FilterAttribute : ExtensibilityAttribute, IFilterMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterAttribute"/> class.
        /// </summary>
        /// <param name="partType">The concrete type of the part being exported.</param>
        /// <param name="typeIdentifier">The type identifier of the part being exported.</param>
        public FilterAttribute(Type partType, string typeIdentifier)
        {
            PartType = partType ?? throw new ArgumentNullException(nameof(partType));
            TypeIdentifier = typeIdentifier ?? throw new ArgumentNullException(nameof(typeIdentifier));
        }

        /// <inheritdoc/>
        public Type PartType 
        { get; }

        /// <inheritdoc/>
        public string TypeIdentifier 
        { get; }
    }
}
