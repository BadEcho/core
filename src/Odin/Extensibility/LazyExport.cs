//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides a lazy indirect reference to an exported object and its associated metadata.
    /// </summary>
    /// <typeparam name="TContract">The contractual type of value being exported.</typeparam>
    /// <typeparam name="TMetadata">The type of metadata accompanying the export.</typeparam>
    internal class LazyExport<TContract,TMetadata> : Lazy<TContract,TMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LazyExport{TContract,TMetadata}"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory that returns the exported value.</param>
        /// <param name="metadata">The metadata value accompanying the export.</param>
        public LazyExport(Func<TContract> valueFactory, IDictionary<string,object?> metadata)
            : base(valueFactory, AttributedModelServices.GetMetadataView<TMetadata>(metadata), LazyThreadSafetyMode.PublicationOnly)
        { }
    }
}