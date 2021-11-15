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

namespace BadEcho.Odin.Configuration
{
    /// <summary>
    /// Provides a type-safe means to access extension data of a common object hierarchy.
    /// </summary>
    /// <typeparam name="T">The base type of the configuration's extension data.</typeparam>
    public sealed class ExtensionDataStore<T>
        where T : new()
    {
        private readonly IConfigurationReader _configurationReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionDataStore{T}"/> class.
        /// </summary>
        public ExtensionDataStore(IConfigurationReader configurationReader)
            => _configurationReader = configurationReader;

        /// <summary>
        /// Gets the raw text of the configuration source to parse.
        /// </summary>
        internal string ConfigurationText
            => _configurationReader.ConfigurationText;

        /// <summary>
        /// Gets a specified section from the configuration as an instance of type <typeparamref name="TImpl"/>.
        /// </summary>
        /// <typeparam name="TImpl">The specific type of object to parse the configuration's extension data as.</typeparam>
        /// <param name="sectionName">The name of the section to parse.</param>
        /// <returns>
        /// A <typeparamref name="TImpl"/> instance reflecting the section named <c>sectionName</c> found in the configuration's
        /// extension data.
        /// </returns>
        public TImpl GetConfiguration<TImpl>(string sectionName) where TImpl : T, new()
            => _configurationReader.GetConfiguration<TImpl>(sectionName);
    }
}
