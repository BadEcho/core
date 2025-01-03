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
using System.Composition.Hosting;
using System.Reflection;

namespace BadEcho.Extensibility.Hosting;

/// <summary>
/// Provides a strategy that directs a <see cref="PluginContext"/> to make available all plugins discoverable within a localized
/// context through the Bad Echo Extensibility framework.
/// </summary>
/// <remarks>
/// <para>
/// Assemblies that are neither plugin assemblies nor extensibility points can take advantage of the Bad Echo Extensibility framework
/// through this strategy.
/// </para>
/// <para>
/// There are several kinds of pluggable parts that are covered by this strategy: 
/// <list type="bullet">
/// <item>
/// Parts local to an assembly making a call to the <see cref="PluginHost.LoadFromCaller{TContract}"/> method.
/// </item>
/// <item>
/// Parts local to a hosting process executable that are being accessed with a call to the
/// <see cref="PluginHost.LoadFromProcess{TContract}"/> method.
/// </item>
/// </list>
/// </para>
/// </remarks>
internal sealed class LocalPluginContextStrategy : IPluginContextStrategy
{
    private readonly Assembly _assembly;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalPluginContextStrategy"/> class.
    /// </summary>
    /// <param name="assembly">
    /// The assembly that is to act as the local context from which pluggable parts are loaded.
    /// </param>
    public LocalPluginContextStrategy(Assembly assembly)
    {
        Require.NotNull(assembly, nameof(assembly));

        _assembly = assembly;
    }

    /// <inheritdoc/>
    public CompositionHost CreateContainer()
    {
        var configuration = new ContainerConfiguration()
            .WithAssembly(_assembly);

        ConventionBuilder conventions = this.LoadConventions(configuration);

        configuration.WithDefaultConventions(conventions);

        return configuration.CreateContainer();
    }
}