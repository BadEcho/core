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

using System.Composition.Hosting;
using BadEcho.Odin.Extensibility.Hosting;

namespace BadEcho.Odin.Tests.Extensibility;

internal class EmptyPluginContextStrategy : IPluginContextStrategy
{
    public CompositionHost CreateContainer()
    {
        return new ContainerConfiguration()
            .CreateContainer();
    }
}