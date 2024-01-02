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

using System.Windows;

namespace BadEcho.Presentation.Markup;

/// <summary>
/// Provides a generator of Bad Echo Presentation framework <see cref="ResourceKey"/> and <see cref="DataTemplateKey"/> instances
/// originating from a specific assembly.
/// </summary>
/// <remarks>
/// This type exists to simplify the process of generating <see cref="BadEchoKey"/> instances, as some static collections may
/// be composed of a fair number of different resources.
/// </remarks>
public sealed class BadEchoKeyProvider
{
    private readonly Type _providerType;

    /// <summary>
    /// Initializes a new instance of the <see cref="BadEchoKeyProvider"/> class.
    /// </summary>
    /// <param name="providerType">The type that will be registering the resources.</param>
    public BadEchoKeyProvider(Type providerType)
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
        => new BadEchoKey(_providerType, name);
}