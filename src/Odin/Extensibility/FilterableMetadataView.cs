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
    /// Provides a metadata view for a filterable export's metadata.
    /// </summary>
    public sealed class FilterableMetadataView : IFilterableMetadata
    {
        /// <inheritdoc/>
        public Guid FamilyId
        { get; set; }

        /// <inheritdoc/>
        public Type? PartType
        { get; set; }
    }
}
