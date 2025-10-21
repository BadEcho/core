// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using BadEcho.Extensions.Properties;

namespace BadEcho.Extensions;

/// <summary>
/// Provides configured <typeparamref name="TOptions"/> instances that allow for changes to be persisted.
/// </summary>
/// <typeparam name="TOptions">The type of options being requested.</typeparam>
public sealed class WritableOptions<TOptions> : IWritableOptions<TOptions>, IDisposable
    where TOptions : class
{
    private readonly IOptionsFactory<TOptions> _factory;
    private readonly IOptionsMonitorCache<TOptions> _cache;
    private readonly List<IDisposable> _registrations = [];
    private readonly IHostEnvironment _environment;
    private readonly Dictionary<string, string> _fileNameMap = [];
    private readonly Dictionary<string, string> _sectionNameMap = [];
    
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WritableOptions{TOptions}"/> class.
    /// </summary>
    public WritableOptions(IOptionsFactory<TOptions> factory,
                           IEnumerable<IOptionsChangeTokenSource<TOptions>> sources,
                           IEnumerable<ConfigureWritableOptions<TOptions>> configureOptionsSet,
                           IOptionsMonitorCache<TOptions> cache,
                           IHostEnvironment environment)
    {
        _factory = factory;
        _cache = cache;
        _environment = environment;

        foreach (var configureOptions in configureOptionsSet)
        {
            _fileNameMap[configureOptions.Name ?? string.Empty] = configureOptions.FileName;
            _sectionNameMap[configureOptions.Name ?? string.Empty] = configureOptions.SectionName;
        }

        // Avoids any unnecessary enumerator allocations, since the DI container typically uses arrays.
        if (sources is IOptionsChangeTokenSource<TOptions>[] sourcesArray)
        {
            foreach (IOptionsChangeTokenSource<TOptions> source in sourcesArray)
            {
                RegisterSource(source);
            }
        }
        else
        {
            foreach (IOptionsChangeTokenSource<TOptions> source in sources)
            {
                RegisterSource(source);
            }
        }
    }
    
    private event EventHandler<EventArgs<(TOptions options, string Name)>>? Changed;

    /// <inheritdoc/>
    public TOptions CurrentValue
        => Get(string.Empty);

    /// <inheritdoc/>
    public TOptions Get(string? name)
    {
        name ??= string.Empty;
        IOptionsFactory<TOptions> factory = _factory;

        return _cache.GetOrAdd(name, () => factory.Create(name));
    }

    /// <inheritdoc/>
    public IDisposable OnChange(Action<TOptions, string?> listener)
    {
        var disposable = new ChangeTracker(this, listener);

        Changed += disposable.OnChanged;

        return disposable;
    }

    /// <inheritdoc/>
    public void Save(string? name)
    {
        name ??= string.Empty;

        JsonNode updateSection = JsonSerializer.SerializeToNode(Get(name)) ?? new JsonObject();
        string path = GetOptionsPath(_fileNameMap[name]);

        JsonNode? optionsFileNode = null;

        if (File.Exists(path))
            optionsFileNode = JsonNode.Parse(File.ReadAllText(path));

        optionsFileNode = optionsFileNode.MergeNodes(updateSection, _sectionNameMap[name]);
      
        File.WriteAllText(path,
                          optionsFileNode.ToJsonString(new JsonSerializerOptions
                                                       {
                                                           WriteIndented = true
                                                       }));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        foreach (IDisposable registration in _registrations)
        {
            registration.Dispose();
        }

        _registrations.Clear();

        _disposed = true;
    }

    private string GetOptionsPath(string fileName)
    {
        IFileInfo info = _environment.ContentRootFileProvider.GetFileInfo(fileName);
        string? path;

        if (info is NotFoundFileInfo)
        {
            string? directoryName = Path.GetDirectoryName(fileName);

            if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            path = fileName;
        }
        else
            path = info.PhysicalPath;            

        if (string.IsNullOrEmpty(path))
            throw new InvalidOperationException(Strings.OptionsFileNotAccessible);

        return path;
    }

    private void RegisterSource(IOptionsChangeTokenSource<TOptions> source)
    {
        IDisposable registration = ChangeToken.OnChange(source.GetChangeToken,
                                                        ConsumeChange,
                                                        source.Name);
        _registrations.Add(registration);

        void ConsumeChange(string? name)
        {
            name ??= string.Empty;

            _cache.TryRemove(name);

            TOptions options = Get(name);

            Changed?.Invoke(this, new EventArgs<(TOptions options, string Name)>((options, name)));
        }
    }

    /// <summary>
    /// Provides a change tracker that will listen for changes until disposed.
    /// </summary>
    internal sealed class ChangeTracker : IDisposable
    {
        private readonly Action<TOptions, string> _listener;
        private readonly WritableOptions<TOptions> _options;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTracker"/> class.
        /// </summary>
        /// <param name="options">The options instance whose changes are tracked.</param>
        /// <param name="listener">The action to be invoked when a configured options instance has changed.</param>
        public ChangeTracker(WritableOptions<TOptions> options, Action<TOptions, string> listener)
        {
            _listener = listener;
            _options = options;
        }

        /// <summary>
        /// Raised when the <see cref="WritableOptions{TOptions}.Changed"/> event fires, executing the action provided
        /// at initialization.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains the event data.</param>
        public void OnChanged(object? sender, EventArgs<(TOptions Options, string Name)> e)
            => _listener(e.Data.Options, e.Data.Name);

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_disposed)
                return;

            _options.Changed -= OnChanged;

            _disposed = true;
        }
    }
}
