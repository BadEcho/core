//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies flags that indicate how the values in an activation context structure are to be used.
/// </summary>
[Flags]
internal enum ActivationContextFlags
{
    /// <summary>
    /// The <see cref="ActivationContext.Source"/> member is valid.
    /// </summary>
    SourceValid = 0x0,
    /// <summary>
    /// The <see cref="ActivationContext.ProcessorArchitecture"/> member is valid.
    /// </summary>
    ProcessorArchitectureValid = 0x1,
    /// <summary>
    /// The <see cref="ActivationContext.LanguageId"/> member is valid.
    /// </summary>
    LanguageIdValid = 0x2,
    /// <summary>
    /// The <see cref="ActivationContext.AssemblyDirectory"/> member is valid.
    /// </summary>
    AssemblyDirectoryValid = 0x4,
    /// <summary>
    /// The <see cref="ActivationContext.ResourceName"/> member is valid.
    /// </summary>
    ResourceNameValid = 0x8,
    /// <summary>
    /// The current process or executable should be used.
    /// </summary>
    SetProcessDefault = 0x10,
    /// <summary>
    /// The <see cref="ActivationContext.ApplicationName"/> member is valid.
    /// </summary>
    ApplicationNameValid = 0x20,
    /// <summary>
    /// The <see cref="ActivationContext.Module"/> member is valid.
    /// </summary>
    ModuleValid = 0x80
}
