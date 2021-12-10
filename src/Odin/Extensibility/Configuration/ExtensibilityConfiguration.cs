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

namespace BadEcho.Odin.Extensibility.Configuration;

/// <summary>
/// Provides configuration settings for Odin's Extensibility framework suitable for binding against with a
/// generic configuration provider (i.e. Microsoft's IConfiguration binder, or even just a JSON deserializer).
/// </summary>
public sealed class ExtensibilityConfiguration : IExtensibilityConfiguration
{
    IEnumerable<IContractConfiguration> IExtensibilityConfiguration.SegmentedContracts
        => SegmentedContracts ?? Enumerable.Empty<IContractConfiguration>();

    /// <inheritdoc/>
    public string? PluginDirectory 
    { get; set; }

    /// <inheritdoc cref="IExtensibilityConfiguration.SegmentedContracts"/>
    /// <remarks>
    /// This property exists in order to provide a configuration binder with a concrete type which it will be able
    /// to instantiate (<see cref="ContractConfiguration"/>), as well as to provide a property that has no
    /// default value assigned to it (which causes issues with some binders) while maintaining the property's
    /// non-nullability contract found on the <see cref="IExtensibilityConfiguration"/> interface.
    /// </remarks>
    public IEnumerable<ContractConfiguration>? SegmentedContracts
    { get; set; }
}