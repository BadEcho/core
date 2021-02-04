//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Convention;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Defines a provider of rule configurations used to define objects as Odin's Extensibility framework parts.
    /// </summary>
    public interface IConventionProvider
    {
        /// <summary>
        /// Configures the provided convention builder with rules specified by the provider.
        /// </summary>
        /// <param name="conventions">A convention builder to configure with the provider's rules.</param>
        void ConfigureRules(ConventionBuilder conventions);
    }
}
