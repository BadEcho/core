//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Convention;
using System.Composition.Hosting;
using System.Reflection;

namespace BadEcho.Extensibility.Hosting;

/// <summary>
/// Provides a strategy that directs a <see cref="PluginContext"/> to make available all plugins discoverable within a local
/// caller's context through the Bad Echo Extensibility framework.
/// </summary>
/// <remarks>
/// <para>
/// Assemblies that are neither plugin assemblies nor extensibility points can take advantage of the Bad Echo Extensibility framework
/// through this strategy. A local pluggable part is considered to be a built-in requirement of an assembly and, therefore,
/// is loaded through a <see cref="PluginHost.LoadRequirement{TContract}(bool)"/> call.
/// </para>
/// <para>
/// An example of a local requirement-styled pluggable part that should be universally understood to be useful is a non-trivial
/// application configuration provider.
/// </para>
/// </remarks>
internal sealed class LocalPluginContextStrategy : IPluginContextStrategy
{
    private readonly Assembly _callingAssembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalPluginContextStrategy"/> class.
    /// </summary>
    /// <param name="callingAssembly">
    /// The calling assembly that is to act as the local context from which pluggable parts are loaded.
    /// </param>
    public LocalPluginContextStrategy(Assembly callingAssembly)
    {
        Require.NotNull(callingAssembly, nameof(callingAssembly));

        _callingAssembly = callingAssembly;
    }

    /// <inheritdoc/>
    public CompositionHost CreateContainer()
    {
        var configuration = new ContainerConfiguration()
            .WithAssembly(_callingAssembly);

        ConventionBuilder conventions = this.LoadConventions(configuration);

        configuration.WithDefaultConventions(conventions);

        return configuration.CreateContainer();
    }
}