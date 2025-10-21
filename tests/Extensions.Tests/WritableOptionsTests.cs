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

namespace BadEcho.Extensions.Tests;

[Collection("WritableTests")]
public class WritableOptionsTests : IDisposable
{
    private static readonly PrimaryFirstOptions _ExpectedPrimaryFirst
        = new()
          {
              OptionA = "First one",
              OptionB = "Second one",
              OptionNumber = 2
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

    private static readonly ArrayOptions _ExpectedArray
        = [new ArrayItem { OptionA = "Item1a", OptionB = "Item1b" }, new ArrayItem { OptionA = "Item2a", OptionB = "Item2b" }];

    private readonly ManualResetEventSlim _mre = new();

    private readonly IWritableOptions<PrimaryFirstOptions> _primaryFirstOptions;
    private readonly IWritableOptions<PrimarySecondOptions> _primarySecondOptions;
    private readonly IWritableOptions<PrimaryNoSectionOptions> _primaryNoSectionOptions;
    private readonly IWritableOptions<SecondaryOptions> _secondaryOptions;
    private readonly IWritableOptions<NonexistentOptions> _nonexistentOptions;
    private readonly IWritableOptions<MissingOptions> _missingOptions;
    private readonly IWritableOptions<ArrayOptions> _arrayOptions;
    private readonly IDisposable? _primaryFirstChange;
    private readonly IDisposable? _primarySecondChange;
    private readonly IDisposable? _primaryNoSectionChange;
    private readonly IDisposable? _secondaryChange;

    public WritableOptionsTests(IWritableOptions<PrimaryFirstOptions> primaryFirstOptions, 
                                IWritableOptions<PrimarySecondOptions> primarySecondOptions,
                                IWritableOptions<PrimaryNoSectionOptions> primaryNoSectionOptions,
                                IWritableOptions<SecondaryOptions> secondaryOptions,
                                IWritableOptions<NonexistentOptions> nonexistentOptions,
                                IWritableOptions<MissingOptions> missingOptions,
                                IWritableOptions<ArrayOptions> arrayOptions)
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
        _missingOptions = missingOptions;
        _arrayOptions = arrayOptions;
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
    public void Load_Array_ReturnsValid()
    {
        ArrayOptions actual = _arrayOptions.Get(null);

        Assert.NotNull(actual);

        ValidateArrays(_ExpectedArray, actual);
    }

    [Fact]
    public async Task Save_PrimaryFirst_UpdatesFile()
    {
        Assert.NotNull(_primaryFirstChange);

        var options = _primaryFirstOptions.Get(null);
        options.OptionNumber = 3;

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

    [Fact]
    public async Task Save_InitiallyMissingTwice_UpdatesFile()
    {
        const string fileName = "appsettings.primary.json";

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        string id = Guid.NewGuid().ToString();
        string backupFilePath = $"{filePath}-{id}";

        File.Copy(filePath, backupFilePath, true);

        await ValidateUpdatedOptions(_missingOptions,
                                     fileName,
                                     "Missing",
                                     backupFile: false);

        await ValidateUpdatedOptions(_missingOptions,
                                     fileName,
                                     "Missing",
                                     backupFile: false);

        string originalFile = await File.ReadAllTextAsync(backupFilePath);

        await File.WriteAllTextAsync(filePath, originalFile);
    }

    [Fact]
    public async Task Save_Array_UpdatesFile()
    {
        await ValidateUpdatedOptions(_arrayOptions,
                                     "appsettings.primary.json",
                                     "Array",
                                     propertyChanger: o => o[0].OptionA = "Changed",
                                     changedPropertySelector: o => o[0].OptionA);
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
    
    private static void ValidateArrays(ArrayOptions expected, ArrayOptions actual)
    {
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

    /// <suppression>
    /// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
    /// </suppression>
    private async Task ValidateUpdatedOptions<TOptions>(IWritableOptions<TOptions> options,
                                                        string fileName,
                                                        string sectionName,
                                                        string? name = null,
                                                        bool backupFile = true,
                                                        Action<TOptions>? propertyChanger = null,
                                                        Func<TOptions, string>? changedPropertySelector = null)
        where TOptions : class, ITestOptions
    {
        propertyChanger ??= o => o.OptionA = "Changed";
        changedPropertySelector ??= o => o.OptionA;

        TOptions actual = options.Get(name);
        Assert.NotNull(actual);
        
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        string id = Guid.NewGuid().ToString();
        string backupFilePath = $"{filePath}-{id}";

        if (backupFile)
            File.Copy(filePath, backupFilePath, true);

        propertyChanger(actual);
        
        options.Save(name);

        string updatedFile = await File.ReadAllTextAsync(filePath);

        JsonNode? updatedNode = JsonNode.Parse(updatedFile);
        Assert.NotNull(updatedNode);

        JsonNode? updatedSectionNode = string.IsNullOrEmpty(sectionName) ? updatedNode : updatedNode[sectionName];
        Assert.NotNull(updatedSectionNode);

        var updated = updatedSectionNode.Deserialize<TOptions>();

        Assert.NotNull(updated);
        Assert.Equal("Changed", changedPropertySelector(updated));

        if (backupFile)
        {
            string originalFile = await File.ReadAllTextAsync(backupFilePath);

            await File.WriteAllTextAsync(filePath, originalFile);
        }

        _mre.Wait(TimeSpan.FromSeconds(3));
    }
}
