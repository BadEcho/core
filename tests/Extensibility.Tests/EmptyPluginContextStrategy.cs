//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Composition.Hosting;
using BadEcho.Extensibility.Hosting;

namespace BadEcho.Tests.Extensibility;

internal class EmptyPluginContextStrategy : IPluginContextStrategy
{
    public CompositionHost CreateContainer()
    {
        return new ContainerConfiguration()
            .CreateContainer();
    }
}