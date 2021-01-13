//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides a set of static methods intended to aid in matters related to general composition using the Managed
    /// Extensibility Framework.
    /// </summary>
    public static class CompositionExtensions
    {
        private const char CONTRACT_GENERIC_OPENING = '(';
        private const char CONTRACT_GENERIC_CLOSING = ')';
        private const char CONTRACT_ARGUMENT_SEPARATOR = ',';

        private static readonly ConcurrentDictionary<string, CreationPolicy> _NamePolicyMap
            = new();

        /// <summary>
        /// Gets the <see cref="Type"/> being targeted by the import definition.
        /// </summary>
        /// <param name="definition">The import definition to retrieve the contract type from.</param>
        /// <param name="contractType">
        /// When this method returns, the contract type named in <c>definition</c>, if a type can be found; otherwise, null.
        /// </param>
        /// <returns>True if a type named by <c>definition</c> was found; otherwise, false.</returns>
        public static bool TryGetContractType(this ImportDefinition definition, [NotNullWhen(true)] out Type? contractType)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            contractType = null;

            string typeName = definition.ContractName;

            int genericOpeningIndex = typeName.IndexOf(CONTRACT_GENERIC_OPENING, StringComparison.Ordinal);

            if (genericOpeningIndex != -1)
            {
                string typeArguments = typeName.Substring(genericOpeningIndex + 1,
                                                          typeName.Length - genericOpeningIndex - 2);

                string innerTypeArguments = TrimInnerTypeArguments(typeArguments);
                int arity = innerTypeArguments.Split(CONTRACT_ARGUMENT_SEPARATOR).Length;

                typeName = FormattableString.Invariant($"{typeName[..genericOpeningIndex]}`{arity}");
            }

            IEnumerable<Assembly> nonDynamicAssemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic);

            foreach (var nonDynamicAssembly in nonDynamicAssemblies)
            {
                contractType = nonDynamicAssembly.GetType(typeName, false);

                if (contractType != null)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines the appropriate creation policy to use based on the import definition's contract.
        /// </summary>
        /// <param name="definition">The import definition to determine the creation policy for.</param>
        /// <returns>A <see cref="CreationPolicy"/> value to use based on <c>definition</c> and its contract.</returns>
        public static CreationPolicy DetermineCreationPolicy(this ImportDefinition definition)
        {
            if (definition == null) 
                throw new ArgumentNullException(nameof(definition));

            return _NamePolicyMap.GetOrAdd(definition.ContractName, _ => MapCreationPolicy(definition));
        }

        private static CreationPolicy MapCreationPolicy(ImportDefinition definition)
        {
            return !definition.TryGetContractType(out Type? contractType)
                ? CreationPolicy.Any
                : ImportDefiniens.DetermineCreationPolicy(contractType);
        }

        private static string TrimInnerTypeArguments(string typeArguments)
        {
            string[] separatedArguments = typeArguments.Split(CONTRACT_GENERIC_OPENING);

            if (separatedArguments.Length == 0)
                return typeArguments;

            foreach (var separatedArgument in separatedArguments)
            {
                string currentArgument = separatedArgument;
                int separatorIndex = currentArgument.IndexOf(CONTRACT_ARGUMENT_SEPARATOR, StringComparison.Ordinal);
                
                if (!currentArgument.Contains(CONTRACT_GENERIC_CLOSING, StringComparison.Ordinal))
                    continue;

                while (separatorIndex < currentArgument.IndexOf(CONTRACT_GENERIC_CLOSING, StringComparison.Ordinal))
                {
                    currentArgument = currentArgument[(separatorIndex + 1)..];
                    separatorIndex = currentArgument.IndexOf(CONTRACT_ARGUMENT_SEPARATOR, StringComparison.Ordinal);

                    if (separatorIndex == -1)
                        break;
                }

                if (separatorIndex == -1)
                    continue;

                typeArguments += currentArgument[separatorIndex..];
            }

            return typeArguments;
        }
    }
}
