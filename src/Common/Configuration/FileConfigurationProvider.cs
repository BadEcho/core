// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Extensions;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace BadEcho.Configuration;

/// <summary>
/// Provides hot-pluggable configuration data sourced from a file.
/// </summary>
public abstract class FileConfigurationProvider : ConfigurationProvider, IFileConfigurationReader, IDisposable
{
    private readonly FileSystemWatcher _watcher = new()
    {
        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
    };

    private readonly ConcurrentDictionary<(Type SectionType, string? SectionName), object> _cachedSections = new();
    private readonly Lock _isMonitoringLock = new();
    private readonly Lock _cacheLock = new();

    private int _cacheGeneration;
    private bool _isMonitoring;
    private bool _disposed;

    /// <summary>
    /// Gets the name of the file containing the configuration to be provided.
    /// </summary>
    protected abstract string SettingsFile
    { get; }

    string IFileConfigurationReader.ConfigurationText
        => ReadConfigurationText();

    /// <inheritdoc cref="IFileConfigurationReader.ConfigurationText"/>
    protected string ConfigurationText
        => ReadConfigurationText();

    /// <inheritdoc/>
    public override T GetConfiguration<T>(string? sectionName)
    {
        string settingsPath = GetSettingsPath();
        string? settingsDirectory = Path.GetDirectoryName(settingsPath);

        lock (_isMonitoringLock)
        {
            // If the directory doesn't exist yet, we'll try to start monitoring next time.
            if (!_isMonitoring && Directory.Exists(settingsDirectory))
            {
                _watcher.Path = settingsDirectory;
                _watcher.Filter = Path.GetFileName(settingsPath);
                _watcher.Changed += HandleWatcherChanged;
                _watcher.Created += HandleWatcherChanged;
                _watcher.Renamed += HandleWatcherChanged;
                _watcher.EnableRaisingEvents = true;

                _isMonitoring = true;
            }
        }

        var cacheKey = (typeof(T), sectionName);

        if (_cachedSections.TryGetValue(cacheKey, out object? cachedSection))
            return (T)cachedSection;
        
        // The generation is captured before reading so that content read prior to an invalidation is never cached.
        int generation = _cacheGeneration;
        var settingsFile = new FileInfo(settingsPath);

        if (settingsFile is not { Exists: true, Length: > 0 })
            return new T();

        T section = ReadConfiguration<T>(settingsFile.ReadAllText(FileShare.ReadWrite), sectionName);

        lock (_cacheLock)
        {
            if (generation == _cacheGeneration)
                section = (T)_cachedSections.GetOrAdd(cacheKey, section);
        }

        return section;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and (optionally) managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only managed resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            _watcher.Dispose();

        _disposed = true;
    }

    /// <summary>
    /// Gets an instance of an optionally named configuration section described by the provided text.
    /// </summary>
    /// <typeparam name="T">The type of object to parse the configuration as.</typeparam>
    /// <param name="configurationText">The text of the configuration source to parse.</param>
    /// <param name="sectionName">
    /// Optional. The name of the section to parse, or null to parse the entire configuration text.
    /// </param>
    /// <returns>A <typeparamref name="T"/> instance reflecting configuration described by <c>configurationText</c>.</returns>
    [return: NotNull]
    protected abstract T ReadConfiguration<T>(string configurationText, string? sectionName = null) where T : new();

    private string ReadConfigurationText()
    {
        var settingsFile = new FileInfo(GetSettingsPath());

        return settingsFile is { Exists: true, Length: > 0 }
            ? settingsFile.ReadAllText(FileShare.ReadWrite)
            : string.Empty;
    }

    private void InvalidateCachedSections()
    {
        lock (_cacheLock)
        {
            _cacheGeneration++;
            _cachedSections.Clear();
        }
    }

    private string GetSettingsPath()
        => Path.GetFullPath(SettingsFile, AppContext.BaseDirectory);

    private void HandleWatcherChanged(object sender, FileSystemEventArgs e)
    {
        var settingsFile = new FileInfo(GetSettingsPath());

        // Some text editors (*cough cough* Notepad++ *cough cough*) clear what's on disk before committing the actual
        // updated content, and some might even replace the file by way of a temporary file, leaving it briefly absent.
        // Let's ignore these rather superfluous updates as well as all intermediate states. Also, in the event the entire
        // contents of the file have been deleted, we ignore this strange occurrence as well.
        if (settingsFile is not { Exists: true, Length: > 0 })
            return;

        InvalidateCachedSections();

        OnConfigurationChanged();
    }
}
