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
    /// Specifies the manner in which exported implementations of an interface must be composed and imported by Odin's
    /// Extensibility framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class ImportPolicyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportPolicyAttribute"/> class.
        /// </summary>
        /// <param name="importPolicy">
        /// The import policy to use when importing implementations of the interface marked with this attribute.
        /// </param>
        public ImportPolicyAttribute(ImportPolicy importPolicy)
        {
            ImportPolicy = importPolicy;
        }

        /// <summary>
        /// Gets a value that specifies the <see cref="Extensibility.ImportPolicy"/> of the attributed part. 
        /// </summary>
        public ImportPolicy ImportPolicy
        { get; }
    }
}