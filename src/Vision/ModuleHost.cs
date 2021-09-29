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

using System;
using BadEcho.Fenestra.ViewModels;
using BadEcho.Odin;
using BadEcho.Omnified.Vision.Extensibility;
using BadEcho.Omnified.Vision.ViewModels;

namespace BadEcho.Omnified.Vision
{
    /// <summary>
    /// Provides a host container for a Vision module.
    /// </summary>
    internal sealed class ModuleHost : IDisposable
    {
        private readonly MessageFileWatcher? _messageFileWatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleHost"/> class.
        /// </summary>
        /// <param name="module">The Vision module to be hosted.</param>
        /// <param name="configuration">Configuration settings for the Vision application.</param>
        public ModuleHost(IVisionModule module, IVisionConfiguration configuration)
        {
            Require.NotNull(module, nameof(module));
            Require.NotNull(configuration, nameof(configuration));

            _messageFileWatcher = new MessageFileWatcher(module, configuration.MessageFilesDirectory);

            Location = module.DefaultLocation;
            ModuleViewModel = module.EnableModule(_messageFileWatcher);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleHost"/> class.
        /// </summary>
        /// <param name="moduleViewModel">The view model to use to display the module.</param>
        /// <param name="location">An enumeration value specifying anchor point to attach the module to.</param>
        private ModuleHost(IViewModel moduleViewModel, AnchorPointLocation location)
        {
            ModuleViewModel = moduleViewModel;
            Location = location;
        }

        /// <summary>
        /// Gets the view model exported by an activated Vision module.
        /// </summary>
        public IViewModel ModuleViewModel
        { get; }

        /// <summary>
        /// Gets the location of the anchor point for the Vision module.
        /// </summary>
        public AnchorPointLocation Location
        { get; }

        /// <summary>
        /// Creates a host container for the a module responsible for displaying the Vision application's title.
        /// </summary>
        /// <param name="configuration">Configuration settings for the Vision application.</param>
        /// <returns>A <see cref="ModuleHost"/> instance for displaying the Vision application's title.</returns>
        public static ModuleHost ForTitle(IVisionConfiguration configuration)
        {
            Require.NotNull(configuration, nameof(configuration));

            var titleViewModel = new VisionTitleViewModel();

            return new ModuleHost(titleViewModel, configuration.TitleLocation);
        }

        /// <inheritdoc/>
        public void Dispose() 
            => _messageFileWatcher?.Dispose();
    }
}
