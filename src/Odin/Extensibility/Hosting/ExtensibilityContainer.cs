//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a container for <see cref="ComposablePart"/> objects as well the means for their composition
    /// within Odin's Extensibility framework.
    /// </summary>
    /// <suppressions>
    /// ReSharper disable SuspiciousTypeConversion.Global
    /// False positives occur. There are types that derive from ExportDefinition and ICompositionElement.
    /// </suppressions>
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
        /// <returns>
        /// A collection of <typeparamref name="T"/> values from all exports satisfying the constraints imposed by <c>definiens</c>.
        /// </returns>
        public IEnumerable<T> GetExportedValues<T>(ImportDefiniens definiens)
        {
            ImportDefinition definition = definiens.DefineImport();
            IEnumerable<Export> exports = GetExports(definition);

            // As MEF doesn't use deferred execution when returning exported values, we won't either.
            return exports.Select(ConvertToValue<T>).ToList();
        }

        /// <summary>
        /// Composes (if necessary) and retrieves lazy values coupled with metadata from all the exports that satisfy the constraints
        /// imposed by the provided definiens.
        /// </summary>
        /// <typeparam name="TContract">The contractual type of export to process.</typeparam>
        /// <typeparam name="TMetadata">The type of exported metadata required.</typeparam>
        /// <param name="definiens">Definiens used to constrain the extent of composition and ultimately the result of this method.</param>
        /// <returns>
        /// A collection of <see cref="Lazy{TContract,TMetadata}"/> values from all exports satisfying the constraints imposed by
        /// <c>definiens</c>.
        /// </returns>
        public IEnumerable<Lazy<TContract,TMetadata>> GetExports<TContract,TMetadata>(ImportDefiniens definiens)
        {
            ImportDefinition definition = definiens.DefineImport();
            IEnumerable<Export> exports = GetExports(definition, null);

            // As MEF doesn't use deferred execution when returning exported values, we won't either.
            return exports.Select(ConvertToValue<TContract, TMetadata>).ToList();
        }

        private static ICompositionElement ConvertToElement(Export export)
        {
            if (export is ICompositionElement element)
                return element;

            return export.Definition is ICompositionElement definitionElement
                ? definitionElement
                : new CompositionElement(export.Definition);
        }

        private static T ConvertToValue<T>(Export export)
        {
            object? exportedValue = export.Value;

            if (exportedValue == null)
                throw new ArgumentException(Strings.ArgumentExportValueNoObject, nameof(export));

            if (exportedValue is ExportedDelegate exportedDelegate)
                exportedValue = exportedDelegate.CreateDelegate(typeof(T).UnderlyingSystemType);

            if (exportedValue is not T castValue)
            {
                throw new CompositionContractMismatchException(
                    Strings.CompositionInvalidExportType.InvariantFormat(ConvertToElement(export).DisplayName,
                                                                         typeof(T)));
            }

            return castValue;
        }

        private static Lazy<TContract, TMetadata> ConvertToValue<TContract, TMetadata>(Export export)
        {
            if (export is IDisposable disposableExport)
            {
                return new DisposableLazyExport<TContract, TMetadata>(() => ConvertToValue<TContract>(export),
                                                                      export.Metadata,
                                                                      disposableExport);
            }

            return new LazyExport<TContract, TMetadata>(() => ConvertToValue<TContract>(export), export.Metadata);
        }
    }
}
