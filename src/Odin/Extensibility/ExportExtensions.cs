//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides a set of static methods that aid in matters related to the manipulation of <see cref="Export"/> objects
    /// provided by the Managed Extensibility Framework.
    /// </summary>
    public static class ExportExtensions
    {
        /// <summary>
        /// Returns an <see cref="ICompositionElement"/> that represents the current <see cref="Export"/> object.
        /// </summary>
        /// <param name="export">A delay-created export object to convert.</param>
        /// <returns>An <see cref="ICompositionElement"/> those represents <c>export</c>.</returns>
        public static ICompositionElement ToElement(this Export export)
        {
            if (export == null) 
                throw new ArgumentNullException(nameof(export));

            return export.Definition is ICompositionElement element
                ? element
                : new CompositionElement(export.Definition);
        }

        /// <summary>
        /// Retrieves an exported value from this delay-created exported object.
        /// </summary>
        /// <typeparam name="T">The contract type of the value being exported.</typeparam>
        /// <param name="export">A delay-created exported object to retrieve the exported value from.</param>
        /// <returns>The value exported from <c>export</c>.</returns>
        public static T ToValue<T>(this Export export)
        {
            if (export == null) 
                throw new ArgumentNullException(nameof(export));

            object? exportedValue = export.Value;

            if (exportedValue == null)
                throw new ArgumentException(Strings.ArgumentExportValueNoObject, nameof(export));
            
            if (exportedValue is ExportedDelegate exportedDelegate)
                exportedValue = exportedDelegate.CreateDelegate(typeof(T).UnderlyingSystemType);

            if (exportedValue is not T castValue)
            {
                throw new CompositionContractMismatchException(
                    Strings.CompositionInvalidExportType.InvariantFormat(export.ToElement().DisplayName,
                                                                         typeof(T)));
            }

            return castValue;
        }
    }
}
