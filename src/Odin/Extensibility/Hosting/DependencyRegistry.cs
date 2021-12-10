//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Convention;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Provides a means to register dependencies ahead of time for injection into pluggable parts during
    /// their initialization and exportation.
    /// </summary>
    /// <typeparam name="T">The type of object depended upon by a pluggable part.</typeparam>
    public abstract class DependencyRegistry<T> : IConventionProvider
    {
        private static readonly object _ArmedLock = new();
        private static T? _ArmedDependency;

        private readonly string _contractName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyRegistry{T}"/> class.
        /// </summary>
        /// <param name="contractName">The contract name to export the dependency as.</param>
        protected DependencyRegistry(string contractName)
        {
            _contractName = contractName;

            Dependency = _ArmedDependency;
        }

        /// <summary>
        /// Gets the loaded dependency object for injection into a pluggable part.
        /// </summary>
        public T? Dependency
         { get; }

        /// <summary>
        /// Executes a method that is dependent on the provided dependency value within a context that guarantees
        /// that the provided dependency value is armed throughout the method's execution.
        /// </summary>
        /// <param name="dependency">
        /// The dependency value to arm for the duration of the method's execution.
        /// </param>
        /// <param name="method">The method to execute within the armed context.</param>
        internal static void ExecuteWhileArmed(T dependency, Action method)
        {
            lock (_ArmedLock)
            {
                _ArmedDependency = dependency;

                method();
            }
        }

        /// <summary>
        /// Executes a method that is dependent on the provided dependency value within a context that guarantees
        /// that the provided dependency value is armed throughout the method's execution.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the provided method.</typeparam>
        /// <param name="dependency">The dependency value to arm for the duration of the method's execution.</param>
        /// <param name="method">The method to execute within the armed context.</param>
        /// <returns>The results of executing <c>method</c> within the armed context.</returns>
        internal static TResult ExecuteWhileArmed<TResult>(T dependency, Func<TResult> method)
        {
            lock (_ArmedLock)
            {
                _ArmedDependency = dependency;

                return method();
            }
        }

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