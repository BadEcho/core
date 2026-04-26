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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BadEcho.Extensions.Tests;

#pragma warning disable CA2254

public class FileLoggerTests
{
    private readonly ILogger<FileLoggerTests> _logger;

    public FileLoggerTests(ILogger<FileLoggerTests> logger)
    {
        _logger = logger;
    }

    [Fact]
    public void LogWarning_Hosted_TextWritten()
    {
        const string text = "Testing Warning";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "test.log");

        _logger.LogWarning(text);

        Assert.True(IsTextWritten(path, text));
    }

    [Fact]
    public void LogInformation_Hosted_TextNotWritten()
    {
        const string text = "Information not here";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "test.log");

        _logger.LogInformation(text);

        Assert.False(IsTextWritten(path, text));
    }

    [Fact]
    public void LogWarning_NotHosted_TextWritten()
    {
        const string text = "Testing Warning";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "test-NotHosted.log");

        var services = new ServiceCollection();

        var b = new ConfigurationBuilder();
        IConfiguration configuration = b.AddJsonFile("appsettings.json")
                                        .Build();

        services.AddLogging(l =>
        {
            l.AddDebug();
            l.AddConfiguration(configuration.GetSection("Logging"));
            l.AddProvider(new TestLoggerProvider());
            l.AddFile(o =>
            {
                o.Path = path;
            });
        });

        var sp = services.BuildServiceProvider();

        var logger = sp.GetRequiredService<ILogger<FileLoggerTests>>();
        
        logger.LogWarning(text);

        Assert.True(IsTextWritten(path, text));
    }

    [Fact]
    public void LogException_Hosted_TextWritten()
    {
        const string text = "Test Exception";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "test.log");

        try
        {
            throw new BadImageFormatException("Very bad");
        }
        catch (BadImageFormatException ex)
        {
            _logger.LogError(ex, text);

        }

        Assert.True(IsTextWritten(path, text));
    }

    private static bool IsTextWritten(string path, string text)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs);

        string fileContents = sr.ReadToEnd();

        return fileContents.Contains(text);
    }
}
#pragma warning restore CA2254