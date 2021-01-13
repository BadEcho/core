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
using BadEcho.Odin.Extensions;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides the definition for an import representation required by a <see cref="ComposablePart"/> object.
    /// </summary>
    internal class ImportDefiniens
    {
        private static readonly ConcurrentDictionary<Type, CreationPolicy> _TypePolicyName 
            = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDefiniens"/> class.
        /// </summary>
        /// <param name="contractName">The contract name of the <see cref="Export"/> object required for importing.</param>
        /// <param name="cardinality">
        /// An enumeration value that specifies the cardinality of the <see cref="Export"/> objects required for importing.
        /// </param>
        /// <param name="requiredCreationPolicy">
        /// An enumeration value that specifies the creation policy required for the exports used to satisfy the import.
        /// </param>
        /// <param name="metadataType">Optional. The type of metadata to be associated with imports.</param>
        public ImportDefiniens(string contractName,
                               ImportCardinality cardinality,
                               CreationPolicy requiredCreationPolicy,
                               Type? metadataType = null)
        {
            if (string.IsNullOrEmpty(contractName))
                throw new ArgumentException(Strings.ArgumentStringNullOrEmpty, nameof(contractName));

            ContractName = contractName;
            Cardinality = cardinality;
            RequiredCreationPolicy = requiredCreationPolicy;
            RequiredMetadata = MetadataExtensions.BuildMetadata(metadataType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDefiniens"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the <see cref="Export"/> object required for importing.</param>
        /// <param name="cardinality">
        /// An enumeration value that specifies the cardinality of the <see cref="Export"/> objects required for importing.
        /// </param>
        /// <param name="metadataType">Optional. The type of metadata to be associated with imports.</param>
        public ImportDefiniens(Type contractType, ImportCardinality cardinality, Type? metadataType = null)
            : this(AttributedModelServices.GetContractName(contractType),
                   cardinality,
                   DetermineCreationPolicy(contractType),
                   metadataType)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDefiniens"/> class.
        /// </summary>
        /// <param name="contractType">The contract type of the <see cref="Export"/> object required for importing.</param>
        /// <param name="cardinality">
        /// An enumeration value that specifies the cardinality of the <see cref="Export"/> objects required for importing.
        /// </param>
        /// <param name="requiredCreationPolicy">
        /// An enumeration value that specifies the creation policy required for the exports used to satisfy the import.
        /// </param>
        /// <param name="metadataType">Optional. The type of metadata to be associated with imports.</param>
        public ImportDefiniens(Type contractType,
                               ImportCardinality cardinality,
                               CreationPolicy requiredCreationPolicy,
                               Type? metadataType = null)
            : this(AttributedModelServices.GetContractName(contractType), cardinality, requiredCreationPolicy, metadataType)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportDefiniens"/> class.
        /// </summary>
        /// <param name="contractName">The contract name of the <see cref="Export"/> object required for importing.</param>
        /// <param name="cardinality">
        /// An enumeration value that specifies the cardinality of the <see cref="Export"/> objects required for importing.
        /// </param>
        /// <param name="requiredCreationPolicy">
        /// An enumeration value that specifies the creation policy required for the exports used to satisfy the import.
        /// </param>
        /// <param name="requiredMetadata">
        /// A collection of key-value pairs that contain the metadata names and types required for importing.
        /// </param>
        public ImportDefiniens(string contractName,
                               ImportCardinality cardinality,
                               CreationPolicy requiredCreationPolicy,
                               IEnumerable<KeyValuePair<string, Type>> requiredMetadata)
        {
            if (string.IsNullOrEmpty(contractName))
                throw new ArgumentException(Strings.ArgumentStringNullOrEmpty, nameof(contractName));

            ContractName = contractName;
            Cardinality = cardinality;
            RequiredCreationPolicy = requiredCreationPolicy;
            RequiredMetadata = requiredMetadata;
        }

        /// <summary>
        /// Gets the cardinality of <see cref="Export"/> objects required for importing.
        /// </summary>
        public ImportCardinality Cardinality 
        { get; }

        /// <summary>
        /// Gets the contract name of the <see cref="Export"/> objects required for importing.
        /// </summary>
        public string ContractName
        { get; }

        /// <summary>
        /// Gets a value that indicates whether import requirements must be satisfied before a <see cref="ComposablePart"/>
        /// can start producing exported objects.
        /// </summary>
        public virtual bool IsPrerequisite
            => true;

        /// <summary>
        /// Gets a value that indicates whether import requirements can be satisfied multiple times throughout the lifetime
        /// of a <see cref="ComposablePart"/>.
        /// </summary>
        public virtual bool IsRecomposable
            => false;

        /// <summary>
        /// Gets a value that specifies the <see cref="CreationPolicy"/> required for the exports used to satisfy the import.
        /// </summary>
        public CreationPolicy RequiredCreationPolicy
        { get; }

        /// <summary>
        /// Gets a collection of key-value pairs that contain the metadata names and types required for importing.
        /// </summary>
        public IEnumerable<KeyValuePair<string, Type>> RequiredMetadata
        { get; }

        /// <summary>
        /// Determines the appropriate creation policy to use based on the given contract type.
        /// </summary>
        /// <param name="contractType">The type of the contract to determine the creation policy for.</param>
        /// <returns>A <see cref="CreationPolicy"/> value to use based on <c>contractType</c>.</returns>
        public static CreationPolicy DetermineCreationPolicy(Type contractType)
        {
            return _TypePolicyName.GetOrAdd(contractType, MapCreationPolicy);
        }

        /// <summary>
        /// Uses this definiens to define an <see cref="ImportDefinition"/>.
        /// </summary>
        /// <returns>An <see cref="ImportDefinition"/> object created with this definiens.</returns>
        public virtual ImportDefinition DefineImport()
        {
            return new ContractBasedImportDefinition(ContractName,
                                                     ContractName,
                                                     RequiredMetadata,
                                                     Cardinality,
                                                     IsRecomposable,
                                                     IsPrerequisite,
                                                     RequiredCreationPolicy);
        }

        private static CreationPolicy MapCreationPolicy(Type contractType)
        {
            ImportPolicyAttribute? policyAttribute
                = contractType.GetAttribute<ImportPolicyAttribute>();

            if (policyAttribute == null)
                return CreationPolicy.Any;

            if (ImportPolicy.FactoryOnly == policyAttribute.ImportPolicy)
            {
                throw new ArgumentException(
                    Strings.ArgumentImportFactoryOnlyExport.InvariantFormat(contractType.Name),
                    nameof(contractType));
            }

            return (CreationPolicy) policyAttribute.ImportPolicy;
        }
    }
}
