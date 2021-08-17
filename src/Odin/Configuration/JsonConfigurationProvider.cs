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

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace BadEcho.Odin.Configuration
{
    /// <summary>
    /// Provides a JSON-based source for hot-pluggable, but otherwise cached, configuration data.
    /// </summary>
    public abstract class JsonConfigurationProvider : ConfigurationProvider
    {
        /// <inheritdoc/>
        [return: NotNull]
        protected override T GetConfiguration<T>(string configurationText)
        {
            T? configuration = default;

            if (!string.IsNullOrEmpty(configurationText))
            {
                configuration = JsonSerializer.Deserialize<T>(
                    configurationText,
                    new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            }

            return configuration ?? new T();
        }
    }
}
