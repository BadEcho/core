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

namespace BadEcho.Configuration;

/// <summary>
/// Provides a type-safe means to access extension data of a common object hierarchy.
/// </summary>
/// <typeparam name="T">The base type of the configuration's extension data.</typeparam>
public sealed class ExtensionDataStore<T>
    where T : new()
{
    private readonly IConfigurationReader? _configurationReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensionDataStore{T}"/> class.
    /// </summary>
    public ExtensionDataStore(IConfigurationReader configurationReader)
        => _configurationReader = configurationReader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtensionDataStore{T}"/> class.
    /// </summary>
    /// <remarks>An extension data store initialized via its default constructor will not contain any data.</remarks>
    public ExtensionDataStore()
    { }

    /// <summary>
    /// Gets the raw text of the configuration source to parse.
    /// </summary>
    internal string ConfigurationText
        => _configurationReader?.ConfigurationText ?? string.Empty;

    /// <summary>
    /// Gets a specified section from the configuration as an instance of type <typeparamref name="TImpl"/>.
    /// </summary>
    /// <typeparam name="TImpl">The specific type of object to parse the configuration's extension data as.</typeparam>
    /// <param name="sectionName">The name of the section to parse.</param>
    /// <returns>
    /// A <typeparamref name="TImpl"/> instance reflecting the section named <c>sectionName</c> found in the configuration's
    /// extension data.
    /// </returns>
    public TImpl GetConfiguration<TImpl>(string sectionName) where TImpl : class, T, new()
        => _configurationReader?.GetConfiguration<TImpl>(sectionName) ?? new TImpl();
}