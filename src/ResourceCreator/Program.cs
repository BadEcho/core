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
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine(Strings.Usage.CulturedFormat(Process.GetCurrentProcess().ProcessName));
            return;
        }

        var resourcesDirectory = args[0];

        if (!Directory.Exists(resourcesDirectory))
        {
            Console.WriteLine(Strings.ResourcesDirectoryNotFound);
            return;
        }

        var outputFile = args[1];

        CreateResources(resourcesDirectory, outputFile);

        Console.WriteLine(Strings.CreatedResources.CulturedFormat(outputFile));
    }

    private static void CreateResources(string resourcesDirectory, string outputFile)
    {
        using (var resourceWriter = new ResourceWriter(Path.Combine(resourcesDirectory, outputFile)))
        {
            foreach (string resourceFile in Directory.GetFiles(resourcesDirectory))
            {
                if (Path.GetFileName(resourceFile).Equals(outputFile, StringComparison.OrdinalIgnoreCase))
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
    }
}
