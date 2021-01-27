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
    /// Defines a factory that aides in the construction of <see cref="PluginContext"/> objects.
    /// </summary>
    internal interface IPluginContextFactory
    {
        /// <summary>
        /// Creates the <see cref="CompositionHost"/> used by the <see cref="PluginContext"/> to compose its objects.
        /// </summary>
        /// <returns>
        /// A <see cref="CompositionHost"/> instance configured to compose objects as specified by the
        /// <see cref="IPluginContextFactory"/>
        /// implementation.
        /// </returns>
        CompositionHost CreateContainer();
    }
}