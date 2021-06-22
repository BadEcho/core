//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Composition;
using BadEcho.Fenestra;

namespace BadEcho.Omnified.Vision.Statistics.Themes
{
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
}
