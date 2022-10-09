//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.CommandLine;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Resources;
using BadEcho.Extensions;
using BadEcho.ResourceCreator.Properties;

namespace BadEcho.ResourceCreator;

internal sealed class Program
{
    /// <summary>
    /// Entry point for the resource creator application.
    /// </summary>
    /// <param name="args">An array of command line arguments passed to the application.</param>
    public static async Task Main(string[] args)
    {
        var resourceCreator = new RootCommand(Strings.RootDescription)
                              {
                                  Name = Process.GetCurrentProcess().ProcessName
                              };

        Argument<string> resourcesPath = CreateResourcesArgument();
        Option<string> output = CreateOutputOption();
        Option<string> filter = CreateFilterOption();

        resourceCreator.AddArgument(resourcesPath);
        resourceCreator.AddOption(output);
        resourceCreator.AddOption(filter);

        resourceCreator.SetHandler(CreateResources, resourcesPath, output, filter);
        
        await resourceCreator.InvokeAsync(args).ConfigureAwait(false);
    }

    private static void CreateResources(string resourcesPath, string outputPath, string filter)
    {
        if (!outputPath.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            outputPath = $"{outputPath}.resources";

        var filterParts = filter.Split(',')
                                .Select(part => part.TrimStart())
                                .ToList();

        using (var resourceWriter = new ResourceWriter(outputPath))
        {
            IEnumerable<string> resourceFiles = Directory.EnumerateFiles(resourcesPath)
                                                         .Where(IsResource);

            foreach (string resourceFile in resourceFiles)
            {   
                // We typically should initialize streams within a using block, however we need to write the
                // asset as a stream, and the streams aren't actually read from until we generate the resource
                // file. Because this is a short running process, however, there is no harm, no foul here.
                var stream = new FileStream(resourceFile, FileMode.Open);
                string resourceName = Path.GetFileNameWithoutExtension(resourceFile);

                resourceWriter.AddResource(resourceName, stream);
            }

            resourceWriter.Generate();
        }

        Console.WriteLine(Strings.CreatedResources.CulturedFormat(outputPath));

        bool IsResource(string filePath)
        {
            // Ignore any existing .resources files in the directory, to avoid creating resource
            // mountains out of resource molehills.
            if (filePath.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
                return false;

            // We apply the filter using Microsoft's own, rather involved, method for matching against wildcard
            // expressions.
            return filterParts
                .Any(filterPart => FileSystemName.MatchesSimpleExpression(filterPart, Path.GetFileName(filePath)));
        }
    }

    private static Argument<string> CreateResourcesArgument()
    {
        var resourcesPath
            = new Argument<string>("RESOURCES_PATH", Strings.ResourcesPathDescription);

        resourcesPath.AddValidator(result =>
                                   {
                                       string resourcesValue = result.Tokens[0].Value;

                                       if (!Directory.Exists(resourcesValue))
                                           result.ErrorMessage = Strings.ResourcesDirectoryNotFound;
                                   });

        return resourcesPath;
    }

    private static Option<string> CreateOutputOption()
    {
        var output
            = new Option<string>("--output", () => "out.resources", Strings.OutputDescription);

        output.AddAlias("-o");
        output.ArgumentHelpName = "OUTPUT_PATH";
        
        return output;
    }

    private static Option<string> CreateFilterOption()
    {
        var filter 
            = new Option<string>("--filter", () => "*.*", Strings.FilterDescription);

        filter.AddAlias("-f");
        filter.ArgumentHelpName = "SEARCH_PATTERN";

        return filter;
    }
}
