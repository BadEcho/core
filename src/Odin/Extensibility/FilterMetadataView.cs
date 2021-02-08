//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides a metadata view for a filterable export's metadata.
    /// </summary>
    public sealed class FilterMetadataView : IFilterMetadata
    {
        /// <inheritdoc/>
        [DefaultValue(null)]
        public Type? PartType
        { get; set; }
        
        /// <inheritdoc/>
        [DefaultValue(null)]
        public string? TypeIdentifier 
        { get; set; }
    }
}
