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
using System.Resources;
using BadEcho.Extensions;
using BadEcho.ResourceCreator.Properties;

namespace BadEcho.ResourceCreator;

internal class Program
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

        var resourcesPath 
            = new Argument<string>("RESOURCES_PATH", Strings.ResourcesPathDescription);

        resourceCreator.AddArgument(resourcesPath);
        
        var output 
            = new Option<string>("--output", () => "Untitled.resources", Strings.OutputDescription);

        output.AddAlias("-o");

        resourceCreator.AddOption(output);

        resourceCreator.SetHandler(CreateResources, resourcesPath, output);

        await resourceCreator.InvokeAsync(args).ConfigureAwait(false);
    }

    private static void CreateResources(string resourcesPath, string outputFile)
    {
        if (!Directory.Exists(resourcesPath))
        {
            Console.WriteLine(Strings.ResourcesDirectoryNotFound);
            return;
        }

        using (var resourceWriter = new ResourceWriter(Path.Combine(resourcesPath, outputFile)))
        {
            foreach (string resourceFile in Directory.GetFiles(resourcesPath))
            {   // Ignore any existing .resources files in the directory.
                if (Path.GetFileName(resourceFile).Contains(".resources", StringComparison.OrdinalIgnoreCase))
                    continue;

                // We typically should initialize streams within a using block, however we need to write the asset as a stream,
                // and the streams aren't actually read from until we actually generate the resource file.
                // Because this is a short running process, however, there is no harm and no foul here.
                var stream = new FileStream(resourceFile, FileMode.Open);
                string resourceName = Path.GetFileNameWithoutExtension(resourceFile);

                resourceWriter.AddResource(resourceName, stream);
            }

            resourceWriter.Generate();
        }

        Console.WriteLine(Strings.CreatedResources.CulturedFormat(outputFile));
    }
}
