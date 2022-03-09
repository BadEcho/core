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

namespace BadEcho.Presentation.Themes;

/// <summary>
/// Provides Bad Echo Presentation framework resource access to a WPF application.
/// </summary>
[Export(typeof(IResourceProvider))]
internal sealed class DefaultResourceProvider : IResourceProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultResourceProvider"/> class.
    /// </summary>
    public DefaultResourceProvider() 
        => ResourceUri = PackUri.FromRelativePath<DefaultResourceProvider>("Root.xaml");

    /// <inheritdoc/>
    public Uri ResourceUri 
    { get; }
}