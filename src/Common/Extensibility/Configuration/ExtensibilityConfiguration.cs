//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Extensibility.Configuration;

/// <summary>
/// Provides configuration settings for the Bad Echo Extensibility framework suitable for binding against with a
/// generic configuration provider (i.e. Microsoft's IConfiguration binder, or even just a JSON deserializer).
/// </summary>
public sealed class ExtensibilityConfiguration : IExtensibilityConfiguration
{
    IEnumerable<IContractConfiguration> IExtensibilityConfiguration.SegmentedContracts
        => SegmentedContracts ?? Enumerable.Empty<IContractConfiguration>();

    /// <inheritdoc/>
    public string? PluginDirectory 
    { get; init; }

    /// <inheritdoc cref="IExtensibilityConfiguration.SegmentedContracts"/>
    /// <remarks>
    /// This property exists in order to provide a configuration binder with a concrete type which it will be able
    /// to instantiate (<see cref="ContractConfiguration"/>), as well as to provide a property that has no
    /// default value assigned to it (which causes issues with some binders) while maintaining the property's
    /// non-nullability contract found on the <see cref="IExtensibilityConfiguration"/> interface.
    /// </remarks>
    public IEnumerable<ContractConfiguration>? SegmentedContracts
    { get; init; }
}