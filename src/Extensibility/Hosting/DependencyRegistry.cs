//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Convention;
using BadEcho.Properties;

namespace BadEcho.Extensibility.Hosting;

/// <summary>
/// Provides a means to register dependencies ahead of time for injection into pluggable parts during
/// their initialization and exportation.
/// </summary>
/// <typeparam name="T">The type of object depended upon by a pluggable part.</typeparam>
public abstract class DependencyRegistry<T> : IConventionProvider
{
    private static readonly Lock _ArmedLock = new();
    private static T? _ArmedDependency;

    private readonly string _contractName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyRegistry{T}"/> class.
    /// </summary>
    /// <param name="contractName">The contract name to export the dependency as.</param>
    protected DependencyRegistry(string contractName)
    {
        _contractName = contractName;
    }

    /// <summary>
    /// Gets the loaded dependency object for injection into a pluggable part.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property is abstract in order to ensure that types deriving from this have their own unique property
    /// definition of <see cref="Dependency"/>, such that registries that provide the same dependency type
    /// <typeparamref name="T"/> can be differentiated by MEF's export descriptors.
    /// </para>
    /// <para>
    /// <see cref="DependencyRegistry{T}"/>-originating contracts are cached based on the hash code of the attributed part's
    /// member info. This will result in collisions for all <see cref="DependencyRegistry{T}"/> derivations of the same type
    /// <typeparamref name="T"/>. 
    /// </para>
    /// <para>
    /// When authoring a new <see cref="DependencyRegistry{T}"/> type, the simplest way to implement this property is to
    /// have it call <see cref="LoadDependency"/>, which will retrieve the dependency the registry has been armed with.
    /// </para>
    /// </remarks>
    public abstract T Dependency
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

    /// <summary>
    /// Retrieves the dependency this registry was armed with.
    /// </summary>
    /// <returns>The armed dependency for this registry.</returns>
    protected T LoadDependency()
    {
        if (_ArmedDependency == null)
            throw new InvalidOperationException(Strings.NoDependencyArmed);

        return _ArmedDependency;
    }
}