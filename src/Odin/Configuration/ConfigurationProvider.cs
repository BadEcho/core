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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BadEcho.Odin.Extensions;

namespace BadEcho.Odin.Configuration
{
    /// <summary>
    /// Provides a format-neutral source for hot-pluggable, but otherwise cached, configuration data.
    /// </summary>
    /// <remarks>
    /// In order to keep in line with common sense as well as best practices, exportation of any pluggable parts derived from
    /// this type should be done so that said parts are <c>shared</c> singletons, so that any unmanaged, disposable, resources
    /// are tied directly with the application's lifecycle.
    /// </remarks>
    public abstract class ConfigurationProvider : IConfigurationProvider
    {
        private readonly FileSystemWatcher _watcher = new(AppContext.BaseDirectory)
                                                      {
                                                          NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
                                                      };

        private readonly Dictionary<Type, object> _cachedSections = new();
        private readonly object _isMonitoringLock = new();

        private bool _isMonitoring;

        /// <inheritdoc/>
        public event EventHandler<EventArgs>? ConfigurationChanged;

        /// <summary>
        /// Gets the name of the file containing the configuration to be provided.
        /// </summary>
        protected abstract string SettingsFile
        { get; }

        /// <inheritdoc/>
        public T GetConfiguration<T>() where T : new()
        {
            lock (_isMonitoringLock)
            {
                if (!_isMonitoring)
                {
                    _watcher.Filter = SettingsFile;
                    _watcher.Changed += HandleConfigurationChanged;
                    _watcher.EnableRaisingEvents = true;

                    _isMonitoring = true;
                }
            }

            Type sectionType = typeof(T);

            if (_cachedSections.ContainsKey(sectionType))
                return (T) _cachedSections[sectionType];

            T? section = default;
            var settingsFile = new FileInfo(SettingsFile);

            if (settingsFile.Exists && settingsFile.Length > 0)
            {
                section = GetConfiguration<T>(settingsFile.ReadAllText(FileShare.ReadWrite));

                _cachedSections.Add(sectionType, section);
            }

            return section ?? new T();
        }

        /// <summary>
        /// Gets a sectional form instance of the configuration described by the provided text.
        /// </summary>
        /// <typeparam name="T">The type of object to parse the configuration as.</typeparam>
        /// <param name="configurationText">The text of the configuration source to parse.</param>
        /// <returns>A <typeparamref name="T"/> instance reflecting configuration described by <c>configurationText</c>.</returns>
        [return: NotNull]
        protected abstract T GetConfiguration<T>(string configurationText) where T : new();

        private void HandleConfigurationChanged(object sender, FileSystemEventArgs e)
        {
            var settingsFile = new FileInfo(SettingsFile);

            // Some text editors (*cough cough* Notepad++ *cough cough*) clear what's on disk before committing the actual
            // updated content. Let's ignore these rather superfluous updates. Also, in the event the entire contents of the
            // file have been deleted, it might be best to ignore this strange occurrence as well.
            if (settingsFile.Length == 0)
                return;

            _cachedSections.Clear();

            ConfigurationChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
