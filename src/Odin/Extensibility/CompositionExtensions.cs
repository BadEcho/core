//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace BadEcho.Odin.Extensibility
{
    /// <summary>
    /// Provides a set of static methods intended to aid in matters related to general composition using the Managed
    /// Extensibility Framework.
    /// </summary>
    public static class CompositionExtensions
    {
        public static ContainerConfiguration WithDirectory(string path)
        {
            var assemblies = new List<Assembly>();
            string[] dllFiles = Directory.GetFiles(Path.GetFullPath(path), "*.dll");

            foreach (var dllFile in dllFiles)
            {
                try
                {
                    assemblies.Add(AssemblyLoadContext.Default.LoadFromAssemblyPath(dllFile));
                }
                catch (FileLoadException ex)
                {

                }
                catch (BadImageFormatException ex)
                {

                }
            }
        }
    }
}
