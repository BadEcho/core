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

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace BadEcho.Configuration;

/// <summary>
/// Provides a JSON-based source for hot-pluggable configuration data.
/// </summary>
public abstract class JsonConfigurationProvider : ConfigurationProvider
{
    /// <inheritdoc/>
    [return: NotNull]
    protected override T ReadConfiguration<T>(string configurationText, string? sectionName = null)
    {
        T? configuration = default;

        if (!string.IsNullOrEmpty(sectionName))
        {
            var document = JsonDocument.Parse(configurationText);
            var element = document.RootElement.GetProperty(sectionName);

            configurationText = element.GetRawText();
        }

        if (!string.IsNullOrEmpty(configurationText))
            configuration = JsonSerializer.Deserialize<T>(configurationText, CreateOptions());

        return configuration ?? new T();
    }

    /// <summary>
    /// Creates the options used when deserializing JSON configuration data.
    /// </summary>
    /// <returns>The <see cref="JsonSerializerOptions"/> instance to use during deserialization.</returns>
    protected virtual JsonSerializerOptions CreateOptions()
        => new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    /// <summary>
    /// Provides a reader of JSON configuration data based on a provided configuration text.
    /// </summary>
    protected sealed class JsonConfigurationReader : IConfigurationReader
    {
        private readonly string _configurationText;
        private readonly JsonConfigurationProvider _configurationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfigurationReader"/> class.
        /// </summary>
        /// <param name="configurationText">The text of the configuration source to parse.</param>
        /// <param name="configurationProvider">
        /// The configuration provider which will parse the provided configuration text as the root configuration.
        /// </param>
        public JsonConfigurationReader(string configurationText, JsonConfigurationProvider configurationProvider)
        {
            _configurationText = configurationText;
            _configurationProvider = configurationProvider;
        }

        string IConfigurationReader.ConfigurationText 
            => _configurationText;

        /// <inheritdoc/>
        public T GetConfiguration<T>() where T : new() 
            => _configurationProvider.ReadConfiguration<T>(_configurationText);

        /// <inheritdoc/>
        public T GetConfiguration<T>(string sectionName) where T : new() 
            => _configurationProvider.ReadConfiguration<T>(_configurationText, sectionName);
    }
}