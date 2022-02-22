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

using System.Windows;

namespace BadEcho.Fenestra.Markup;

/// <summary>
/// Provides a generator of Fenestra <see cref="ResourceKey"/> and <see cref="DataTemplateKey"/> instances originating from
/// a specific assembly.
/// </summary>
/// <remarks>
/// This type exists to simplify the process of generating <see cref="FenestraKey"/> instances, as some static collections may
/// be composed of a fair number of different resources.
/// </remarks>
public sealed class FenestraKeyProvider
{
    private readonly Type _providerType;

    /// <summary>
    /// Initializes a new instance of the <see cref="FenestraKeyProvider"/> class.
    /// </summary>
    /// <param name="providerType">The type that will be registering the resources.</param>
    public FenestraKeyProvider(Type providerType)
    {
        Require.NotNull(providerType, nameof(providerType));

        _providerType = providerType;
    }

    /// <summary>
    /// Creates a <see cref="ResourceKey"/> instance with the provided uniquely identifying resource name.
    /// </summary>
    /// <param name="name">The uniquely identifying name of the resource.</param>
    /// <returns>A <see cref="ResourceKey"/> instance pointing to the resource identified by <c>name</c>.</returns>
    public ResourceKey CreateKey(string name)
        => new FenestraKey(_providerType, name);
}