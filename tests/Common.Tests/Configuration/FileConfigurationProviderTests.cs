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

using BadEcho.Configuration;
using System.Reflection;
using Xunit;

namespace BadEcho.Tests.Configuration;

/// <suppressions>
/// ReSharper disable UnusedAutoPropertyAccessor.Local
/// ReSharper disable AccessToDisposedClosure
/// </suppressions>
public class FileConfigurationProviderTests : IDisposable
{
    private const string SETTINGS_FILE_NAME = "settings.json";

    private static readonly TimeSpan _Timeout = TimeSpan.FromSeconds(5);

    private readonly string _directory;
    private readonly string _settingsPath;

    public FileConfigurationProviderTests()
    {
        _directory = Path.Combine(Path.GetTempPath(), $"BadEcho.Tests.{Guid.NewGuid():N}");
        _settingsPath = Path.Combine(_directory, SETTINGS_FILE_NAME);

        Directory.CreateDirectory(_directory);
    }

    public void Dispose()
    {
        Directory.Delete(_directory, true);
    }

    [Fact]
    public void HandleWatcherChanged_MissingFile_DoesNotThrow()
    {
        using var provider = new TempFileProvider(_settingsPath);

        // The handler is invoked directly so the missing-file path is exercised deterministically.
        MethodInfo? handler = typeof(FileConfigurationProvider).GetMethod("HandleWatcherChanged",
                                                                          BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(handler);

        var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, _directory, SETTINGS_FILE_NAME);
        Exception? exception = Record.Exception(() => handler.Invoke(provider, [provider, args]));

        Assert.Null(exception);
    }

    [SkipOnGitHubFact]
    public void GetConfiguration_FileReplacedByRename_ReturnsUpdatedValue()
    {
        WriteSettings("before");

        using var provider = new TempFileProvider(_settingsPath);

        Assert.Equal("before", provider.GetConfiguration<FakeData>().SomeData);

        string tempPath = Path.Combine(_directory, $"{SETTINGS_FILE_NAME}.tmp");
        WriteSettings("after", tempPath);

        bool changed = WaitForChange(provider, () => File.Move(tempPath, _settingsPath, true));

        Assert.True(changed);
        Assert.Equal("after", provider.GetConfiguration<FakeData>().SomeData);
    }

    private void WriteSettings(string value, string? path = null)
        => File.WriteAllText(path ?? _settingsPath, $$"""{ "someData": "{{value}}" }""");

    private static bool WaitForChange(FileConfigurationProvider provider, Action action)
    {
        // A completion source needs no disposal, so a notification arriving after the wait cannot fault the raising thread.
        var changed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        provider.ConfigurationChanged += HandleConfigurationChanged;

        try
        {
            action();

            return changed.Task.Wait(_Timeout);
        }
        finally
        {
            provider.ConfigurationChanged -= HandleConfigurationChanged;
        }

        void HandleConfigurationChanged(object? sender, EventArgs e)
        {
            changed.TrySetResult();
        }
    }

    private class TempFileProvider : JsonConfigurationProvider
    {
        private readonly string _settingsFile;

        public TempFileProvider(string settingsFile)
            => _settingsFile = settingsFile;

        protected override string SettingsFile
            => _settingsFile;
    }

    private class FakeData
    {
        public string? SomeData { get; init; }
    }
}