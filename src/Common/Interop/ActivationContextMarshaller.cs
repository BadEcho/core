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

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace BadEcho.Interop;

/// <summary>
/// Provides a custom marshaller for an activation context's configuration.
/// </summary>
[CustomMarshaller(typeof(ActivationContext), MarshalMode.ManagedToUnmanagedRef, typeof(ActivationContextMarshaller))]
internal static unsafe class ActivationContextMarshaller
{
    /// <summary>
    /// Converts a managed <see cref="ActivationContext"/> instance to its unmanaged counterpart.
    /// </summary>
    /// <param name="managed">A managed instance of an activation context's configuration.</param>
    /// <returns>An <see cref="ACTCTX"/> equivalent of <c>managed</c>.</returns>
    public static ACTCTX ConvertToUnmanaged(ActivationContext managed)
    {
        Require.NotNull(managed, nameof(managed));

        return new ACTCTX
               {
                   cbSize = Marshal.SizeOf<ACTCTX>(),
                   dwFlags = (uint) managed.Flags,
                   lpSource = Utf16StringMarshaller.ConvertToUnmanaged(managed.Source),
                   wProcessorArchitecture  = (ushort) managed.ProcessorArchitecture,
                   wLangId = managed.LanguageId,
                   lpAssemblyDirectory = Utf16StringMarshaller.ConvertToUnmanaged(managed.AssemblyDirectory),
                   lpResourceName = managed.ResourceName,
                   lpApplicationName = Utf16StringMarshaller.ConvertToUnmanaged(managed.ApplicationName),
                   hModule = managed.Module 
               };
    }

    /// <summary>
    /// Converts an unmanaged <see cref="ACTCTX"/> instance to its managed counterpart.
    /// </summary>
    /// <param name="unmanaged">An unmanaged instance of an activation context's configuration.</param>
    /// <returns>An <see cref="ActivationContext"/> equivalent of <c>unmanaged</c>.</returns>
    public static ActivationContext ConvertToManaged(ACTCTX unmanaged)
        => new((ActivationContextFlags) unmanaged.dwFlags)
           {
               Source = Utf16StringMarshaller.ConvertToManaged(unmanaged.lpSource),
               ProcessorArchitecture = (ProcessorArchitecture) unmanaged.wProcessorArchitecture,
               LanguageId = unmanaged.wLangId,
               AssemblyDirectory = Utf16StringMarshaller.ConvertToManaged(unmanaged.lpAssemblyDirectory),
               ResourceName = unmanaged.lpResourceName,
               ApplicationName = Utf16StringMarshaller.ConvertToManaged(unmanaged.lpApplicationName),
               Module = unmanaged.hModule
           };

    /// <summary>
    /// Represents the configuration for creating an activation context.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct ACTCTX
    {
        /// <summary>
        /// The size, in bytes, of this structure.
        /// </summary>
        public int cbSize;
        /// <summary>
        /// Flags that indicate how the values included in this structure are to be used.
        /// </summary>
        public uint dwFlags;
        /// <summary>
        /// The path of the manifest file or PE image to be used to create the activation context.
        /// </summary>
        public ushort* lpSource;
        /// <summary>
        /// Identifies the type of processor used.
        /// </summary>
        public ushort wProcessorArchitecture;
        /// <summary>
        /// Specifies the language manifest that should be used.
        /// </summary>
        public ushort wLangId;
        /// <summary>
        /// The base directory in which to perform private assembly probing if assemblies in the activation context are
        /// not present in the system-wide store.
        /// </summary>
        public ushort* lpAssemblyDirectory;
        /// <summary>
        /// A pointer to a string that contains the resource name to be loaded from the PE specified in <see cref="hModule"/>
        /// or <see cref="lpSource"/>.
        /// </summary>
        public IntPtr lpResourceName;
        /// <summary>
        /// The name of the current application.
        /// </summary>
        public ushort* lpApplicationName;
        /// <summary>
        /// A handle to the module to use to create the activation context.
        /// </summary>
        public IntPtr hModule;
    }
}
