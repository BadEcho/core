//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a container for <see cref="ComposablePart"/> objects as well the means for their composition
    /// within Odin's Extensibility framework.
    /// </summary>
    internal sealed class ExtensibilityContainer : CompositionContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensibilityContainer"/> class.
        /// </summary>
        /// <param name="catalog">A catalog that provides <see cref="Export"/> objects to the container.</param>
        /// <param name="providers">
        /// An array of <see cref="ExportProvider"/> objects that provide access to additional <see cref="Export"/> objects.
        /// </param>
        public ExtensibilityContainer(ComposablePartCatalog catalog, params ExportProvider[]? providers)
            : base(catalog, true, providers)
        { }

        /// <summary>
        /// Composes (if necessary) and retrieves values from all the exports that satisfy the constraints imposed by the provided
        /// definiens.
        /// </summary>
        /// <typeparam name="T">The contractual type of export to process.</typeparam>
        /// <param name="definiens">Definiens used to constrain the extent of composition and ultimately the result of this method.</param>
        /// <returns>A collection of values from all exports satisfying the constraints imposed by <c>definiens</c>.</returns>
        public IEnumerable<T> GetExportedValues<T>(ImportDefiniens definiens)
        {
            ImportDefinition definition = definiens.DefineImport();
            IEnumerable<Export> exports = GetExports(definition);

            // As MEF doesn't use deferred execution when returning exported values, we won't either.
            return exports.Select(export => export.ToValue<T>()).ToList();
        }
    }
}
