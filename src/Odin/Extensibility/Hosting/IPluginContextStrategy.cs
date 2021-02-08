//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Hosting;

namespace BadEcho.Odin.Extensibility.Hosting
{
    /// <summary>
    /// Defines an initialization strategy for a <see cref="PluginContext"/> which influences the manner in which plugins are loaded
    /// through Odin's Extensibility framework.
    /// </summary>
    internal interface IPluginContextStrategy
    {
        /// <summary>
        /// Creates the <see cref="CompositionHost"/> used by the <see cref="PluginContext"/> to compose its objects.
        /// </summary>
        /// <returns>
        /// A <see cref="CompositionHost"/> instance configured to compose objects as specified by the
        /// <see cref="IPluginContextStrategy"/> implementation.
        /// </returns>
        CompositionHost CreateContainer();
    }
}