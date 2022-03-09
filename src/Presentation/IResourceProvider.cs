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

namespace BadEcho.Presentation;

/// <summary>
/// Defines a provider of Bad Echo Presentation framework resource information.
/// </summary>
/// <remarks>
/// Exporting an implementation of this interface allows for resources defined in a library to be imported into the
/// application-scoped resource dictionary belonging to a WPF application. This allows both libraries directly
/// referenced by the host application and plugins alike a means to easily make their resources globally accessible.
/// </remarks>
public interface IResourceProvider
{
    /// <summary>
    /// Gets a URI pointing to a root resource dictionary that links to all the resources that are to be imported into the
    /// application.
    /// </summary>
    Uri ResourceUri { get; }
}