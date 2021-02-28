//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Hosting;
using BadEcho.Odin.Extensibility.Hosting;

namespace BadEcho.Odin.Tests.Extensibility
{
    internal class EmptyPluginContextStrategy : IPluginContextStrategy
    {
        public CompositionHost CreateContainer()
        {
            return new ContainerConfiguration()
                .CreateContainer();
        }
    }
}
