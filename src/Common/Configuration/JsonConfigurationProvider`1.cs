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

using System.Text.Json;
using System.Text.Json.Serialization;
using BadEcho.Extensions;

namespace BadEcho.Configuration;

/// <summary>
/// Provides a JSON-based source for hot-pluggable configuration data with hierarchical extension data support.
/// </summary>
/// <typeparam name="TExtensionData">The base type of the configuration's extension data.</typeparam>
public abstract class JsonConfigurationProvider<TExtensionData> : JsonConfigurationProvider
    where TExtensionData : new()
{
    /// <inheritdoc/>
    protected override JsonSerializerOptions CreateOptions()
    {
        var options = base.CreateOptions();

        options.Converters.Add(new JsonExtensionDataConverter(this));

        return options;
    }

    /// <summary>
    /// Provides a converter of <see cref="ExtensionDataStore{TExtensionData}"/> objects to or from JSON.
    /// </summary>
    private sealed class JsonExtensionDataConverter : JsonConverter<ExtensionDataStore<TExtensionData>>
    {
        private readonly JsonConfigurationProvider _configurationProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonExtensionDataConverter"/> class.
        /// </summary>
        /// <param name="configurationProvider">The configuration provider which will parse the extension data.</param>
        public JsonExtensionDataConverter(JsonConfigurationProvider configurationProvider)
            => _configurationProvider = configurationProvider;

        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert) 
            => typeToConvert.IsA<ExtensionDataStore<TExtensionData>>();

        /// <inheritdoc/>
        public override ExtensionDataStore<TExtensionData> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var extensionDataDocument = JsonDocument.ParseValue(ref reader);
            var configurationReader 
                = new JsonConfigurationReader(extensionDataDocument.RootElement.GetRawText(), _configurationProvider);

            return new ExtensionDataStore<TExtensionData>(configurationReader);
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, ExtensionDataStore<TExtensionData> value, JsonSerializerOptions options)
            // TODO: Uncomment when updating to .NET 6 adding support for writes.
            //=> writer.WriteRawValue(value.ConfigurationText);
            => throw new NotSupportedException();
    }
}