//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides a disposable lazy indirect reference to an exported object and its associated metadata.
    /// </summary>
    /// <typeparam name="TContract">The contractual type of value being exported.</typeparam>
    /// <typeparam name="TMetadata">The type of metadata accompanying the export.</typeparam>
    internal sealed class DisposableLazyExport<TContract,TMetadata> : LazyExport<TContract,TMetadata>, IDisposable
    {
        private readonly IDisposable _disposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableLazyExport{TContract,TMetadata}"/> class.
        /// </summary>
        /// <param name="valueFactory">The value factory that returns the exported value.</param>
        /// <param name="metadata">The metadata value accompanying the export.</param>
        /// <param name="disposable">
        /// The disposable delay-exported object itself, to be disposed upon the disposal of this object.
        /// </param>
        public DisposableLazyExport(Func<TContract> valueFactory, IDictionary<string, object?> metadata, IDisposable disposable)
            : base(valueFactory, metadata)
        {
            _disposable = disposable;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
