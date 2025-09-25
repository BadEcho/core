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

using System.Text.Json;
using System.Text.Json.Nodes;

namespace BadEcho.Extensions.Options.Tests;

[Collection("WritableTests")]
public class WritableOptionsTests : IDisposable
{
    private static readonly TestOptions _ExpectedPrimaryFirst
        = new()
          {
              OptionA = "First one",
              OptionB = "Second one"
          };

    private static readonly TestOptions _ExpectedNamedPrimaryFirst
        = new()
          {
              OptionA = "Second first one",
              OptionB = "Second second one"
          };

    private static readonly TestOptions _ExpectedPrimarySecond
        = new()
          {
              OptionA = "First second one",
              OptionB = "Second second one"
          };

    private static readonly TestOptions _ExpectedPrimaryNoSection
        = new()
          {
              OptionA = "First sectionless one",
              OptionB = "Second sectionless one"
          };

    private static readonly TestOptions _ExpectedSecondary
        = new()
          {
              OptionA = "First secondary one",
              OptionB = "Second secondary one"
          };

    private readonly ManualResetEventSlim _mre = new();

    private readonly IWritableOptions<PrimaryFirstOptions> _primaryFirstOptions;
    private readonly IWritableOptions<PrimarySecondOptions> _primarySecondOptions;
    private readonly IWritableOptions<PrimaryNoSectionOptions> _primaryNoSectionOptions;
    private readonly IWritableOptions<SecondaryOptions> _secondaryOptions;
    private readonly IWritableOptions<NonexistentOptions> _nonexistentOptions;
    private readonly IDisposable? _primaryFirstChange;
    private readonly IDisposable? _primarySecondChange;
    private readonly IDisposable? _primaryNoSectionChange;
    private readonly IDisposable? _secondaryChange;

    public WritableOptionsTests(IWritableOptions<PrimaryFirstOptions> primaryFirstOptions, 
                                IWritableOptions<PrimarySecondOptions> primarySecondOptions,
                                IWritableOptions<PrimaryNoSectionOptions> primaryNoSectionOptions,
                                IWritableOptions<SecondaryOptions> secondaryOptions,
                                IWritableOptions<NonexistentOptions> nonexistentOptions)
    {
        _primaryFirstOptions = primaryFirstOptions;
        _primaryFirstChange = _primaryFirstOptions.OnChange((_,_) => _mre.Set());

        _primarySecondOptions = primarySecondOptions;
        _primarySecondChange = _primarySecondOptions.OnChange((_, _) => _mre.Set());

        _primaryNoSectionOptions = primaryNoSectionOptions;
        _primaryNoSectionChange = _primaryNoSectionOptions.OnChange((_, _) => _mre.Set());

        _secondaryOptions = secondaryOptions;
        _secondaryChange = _secondaryOptions.OnChange((_, _) => _mre.Set());

        _nonexistentOptions = nonexistentOptions;
    }

    [Fact]
    public void Load_PrimaryFirst_ReturnsValid()
    {
        ValidateOptions(_primaryFirstOptions,
                        _ExpectedPrimaryFirst);
    }

    [Fact]
    public void Load_NamedPrimaryFirst_ReturnsValid()
    {
        ValidateOptions(_primaryFirstOptions,
                        _ExpectedNamedPrimaryFirst,
                        "Two");
    }

    [Fact]
    public void Load_PrimarySecond_ReturnsValid()
    {
        ValidateOptions(_primarySecondOptions,
                        _ExpectedPrimarySecond);
    }

    [Fact]
    public void Load_NoSection_ReturnsValid()
    {
        ValidateOptions(_primaryNoSectionOptions,
                        _ExpectedPrimaryNoSection);
    }

    [Fact]
    public void Load_Secondary_ReturnsValid()
    {
        ValidateOptions(_secondaryOptions,
                        _ExpectedSecondary);
    }

    [Fact]
    public async Task Save_PrimaryFirst_UpdatesFile()
    {
        Assert.NotNull(_primaryFirstChange);

        await ValidateUpdatedOptions(_primaryFirstOptions,
                                     "appsettings.primary.json",
                                     "PrimaryFirst");
    }

    [Fact]
    public async Task Save_NamedPrimaryFirst_UpdatesFile()
    {
        await ValidateUpdatedOptions(_primaryFirstOptions,
                                     "appsettings.primary.json",
                                     "PrimaryFirst2",
                                     "Two");
    }

    [Fact]
    public async Task Save_PrimarySecond_UpdatesFile()
    {
        await ValidateUpdatedOptions(_primarySecondOptions,
                                     "appsettings.primary.json",
                                     "PrimarySecond");
    }

    [Fact]
    public async Task Save_NoSection_UpdatesFile()
    {
        await ValidateUpdatedOptions(_primaryNoSectionOptions,
                                     "appsettings.primary.json",
                                     "");
    }

    [Fact]
    public async Task Save_Secondary_UpdatesFile()
    {
        await ValidateUpdatedOptions(_secondaryOptions,
                                     "appsettings.secondary.json",
                                     "Secondary");
    }

    [Fact]
    public async Task Save_Nonexistent_CreateFile()
    {
        const string appSettingsNonexistent = "appsettings.nonexistent.json";
        
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), appSettingsNonexistent);

        Assert.False(File.Exists(filePath));

        await ValidateUpdatedOptions(_nonexistentOptions,
                                     appSettingsNonexistent,
                                     NonexistentOptions.SectionName,
                                     backupFile: false);

        Assert.True(File.Exists(filePath));

        File.Delete(filePath);
    }

    public void Dispose()
    {
        _primaryFirstChange?.Dispose();
        _primarySecondChange?.Dispose();
        _primaryNoSectionChange?.Dispose();
        _secondaryChange?.Dispose();
        _mre.Dispose();
    }

    private static void ValidateOptions<TOptions>(IWritableOptions<TOptions> options, 
                                                  TestOptions expected,
                                                  string? name = null)
        where TOptions : TestOptions
    {
        TestOptions actual = options.Get(name);

        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    /// <suppression>
    /// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
    /// </suppression>
    private async Task ValidateUpdatedOptions<TOptions>(IWritableOptions<TOptions> options,
                                                        string fileName,
                                                        string sectionName,
                                                        string? name = null,
                                                        bool backupFile = true)
        where TOptions : TestOptions
    {
        TestOptions actual = options.Get(name);
        Assert.NotNull(actual);
        
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        string id = Guid.NewGuid().ToString();
        string backupFilePath = $"{filePath}-{id}";

        if (backupFile)
            File.Copy(filePath, backupFilePath, true);

        actual.OptionA = "Changed";
        
        options.Save(name);

        string updatedFile = await File.ReadAllTextAsync(filePath);

        JsonNode? updatedNode = JsonNode.Parse(updatedFile);
        Assert.NotNull(updatedNode);

        JsonNode? updatedSectionNode = updatedNode[sectionName];
        Assert.NotNull(updatedSectionNode);

        var updated = updatedSectionNode.Deserialize<TOptions>();

        Assert.NotNull(updated);
        Assert.Equal("Changed", updated.OptionA);

        if (backupFile)
        {
            string originalFile = await File.ReadAllTextAsync(backupFilePath);

            await File.WriteAllTextAsync(filePath, originalFile);
        }

        _mre.Wait(TimeSpan.FromSeconds(3));
    }
}
