﻿//-----------------------------------------------------------------------
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

using System;
using System.Composition;
using BadEcho.Fenestra;

namespace BadEcho.Omnified.Vision.Apocalypse.Themes
{
    /// <summary>
    /// Provides access to resources belonging to the Apocalypse plugin for Vision.
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