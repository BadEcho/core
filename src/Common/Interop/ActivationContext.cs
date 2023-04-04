//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Provides the configuration for creating an activation context.
/// </summary>
/// <remarks>
/// Activation contexts are data structures in memory containing information that the system can use to redirect
/// an application to load a particular DLL version, typically through the leveraging of side-by-side version
/// information found in embedded assembly manifests.
/// </remarks>
internal sealed class ActivationContext
{
    public ActivationContext(ActivationContextFlags flags) 
        => Flags = flags;

    /// <summary>
    /// Gets flags that indicate how the values included in the structure are to be used.
    /// </summary>
    public ActivationContextFlags Flags
    { get; }

    /// <summary>
    /// Gets or sets the path of the manifest file or PE image to be used to create the activation context.
    /// </summary>
    public string? Source
    { get; set; }

    /// <summary>
    /// Gets or sets the type of processor used.
    /// </summary>
    public ProcessorArchitecture ProcessorArchitecture
    { get; set; }

    /// <summary>
    /// Gets or sets the language manifest that should be used.
    /// </summary>
    public ushort LanguageId
    { get; set; }

    /// <summary>
    /// Gets or sets the base directory in which to perform private assembly probing if assemblies in the
    /// activation context are not present in the system-wide store.
    /// </summary>
    public string? AssemblyDirectory
    { get; set; }

    /// <summary>
    /// Gets or sets a pointer to a string that contains the resource name to be loaded from the PE specified
    /// in <see cref="Module"/> or <see cref="Source"/>.
    /// </summary>
    public IntPtr ResourceName
    { get; set; }

    /// <summary>
    /// Gets or sets the name of the current application.
    /// </summary>
    public string? ApplicationName
    { get; set; }

    /// <summary>
    /// Gets or sets a handle to the module to use to create the activation context.
    /// </summary>
    public IntPtr Module
    { get; set; }
}
