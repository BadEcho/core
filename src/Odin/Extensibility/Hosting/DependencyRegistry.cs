//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Composition.Convention;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a means to register dependencies ahead of time for injection into pluggable parts during
    /// their initialization and exportation.
    /// </summary>
    /// <typeparam name="TDependency">The type of object depended upon by a pluggable part.</typeparam>
    public abstract class DependencyRegistry<TDependency> : IConventionProvider
    {
        private readonly string _contractName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyRegistry{TDependency}"/> class.
        /// </summary>
        /// <param name="contractName">The contract name to export the dependency as.</param>
        protected DependencyRegistry(string contractName)
        {
            _contractName = contractName;

            Dependency = ArmedDependency;
        }

        /// <summary>
        /// Gets the loaded dependency object for injection into a pluggable part.
        /// </summary>
        public TDependency? Dependency
         { get; }

        /// <summary>
        /// Gets or sets a dependency object armed for injection into a pluggable part.
        /// </summary>
        [field: ThreadStatic]
        internal static TDependency? ArmedDependency
        { get; set; }

        /// <inheritdoc/>
        public void ConfigureRules(ConventionBuilder conventions)
        {
            Require.NotNull(conventions, nameof(conventions));

            conventions.ForType(GetType())
                       .ExportProperties(p => p.Name == nameof(Dependency),
                                         (_, ex) => ex.AsContractName(_contractName));

        }
    }
}