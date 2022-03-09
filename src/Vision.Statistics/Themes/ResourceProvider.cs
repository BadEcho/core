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

using System.Composition;
using BadEcho.Presentation;

namespace BadEcho.Vision.Statistics.Themes;

/// <summary>
/// Provides access to resources belonging to the Statistics plugin for Vision.
/// </summary>
[Export(typeof(IResourceProvider))]
internal sealed class ResourceProvider : IResourceProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceProvider"/> class.
    /// </summary>
    public ResourceProvider() 
        => ResourceUri = PackUri.FromRelativePath<ResourceProvider>("Root.xaml");

    /// <inheritdoc/>
    public Uri ResourceUri 
    { get; }
}