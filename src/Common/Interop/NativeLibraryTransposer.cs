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

using System.Reflection;
using System.Runtime.InteropServices;
using BadEcho.Extensions;
using BadEcho.Logging;
using BadEcho.Properties;

namespace BadEcho.Interop;

/// <summary>
/// Provides a native library imports resolver that can load platform-specific assets from directories other
/// than <see cref="AppContext.BaseDirectory"/>.
/// </summary>
public sealed class NativeLibraryTransposer
{
    private readonly string _baseDirectory;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NativeLibraryTransposer"/> class.
    /// </summary>
    /// <param name="assembly">The assembly to register the resolver for.</param>
    /// <param name="baseDirectory">The path to the base directory to use to probe for assemblies.</param>
    private NativeLibraryTransposer(Assembly assembly, string baseDirectory)
    {
        _baseDirectory = baseDirectory;

        NativeLibrary.SetDllImportResolver(assembly, ResolveLibrary);
    }

    /// <summary>
    /// Creates a native library resolver for the provided assembly using the specified directory to probe
    /// for assemblies.
    /// search path 
    /// </summary>
    /// <param name="assembly">The assembly to register the resolver for.</param>
    /// <param name="baseDirectory">The path to the base directory to use to probe for assemblies.</param>
    /// <returns>
    /// A <see cref="NativeLibraryTransposer"/> instance which may be discarded, as the runtime itself will maintain
    /// a strong reference to it for the lifetime of <c>assembly</c>.
    /// </returns>
    public static NativeLibraryTransposer Create(Assembly assembly, string baseDirectory)
    {
        Require.NotNull(baseDirectory, nameof(baseDirectory));

        return new NativeLibraryTransposer(assembly, baseDirectory);
    }

    private IntPtr ResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {   // Sometimes imports include the file extension, sometimes not.
        libraryName = $"{Path.GetFileNameWithoutExtension(libraryName)}.dll";

        string assetPath = $@"runtimes\{RuntimeInformation.RuntimeIdentifier}\native\{libraryName}";

        IntPtr hModule = Kernel32.LoadLibrary(Path.Combine(_baseDirectory, assetPath));

        if (hModule == IntPtr.Zero)
            Logger.Debug(Strings.NativeResolvedLoadFailed.InvariantFormat(Marshal.GetLastPInvokeErrorMessage()));

        return hModule;
    }
}
