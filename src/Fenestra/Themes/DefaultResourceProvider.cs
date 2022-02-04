//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Composition;

namespace BadEcho.Fenestra.Themes;

/// <summary>
/// Provides Fenestra framework resource access to a WPF application.
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