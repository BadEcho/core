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

namespace BadEcho.Interop;

/// <summary>
/// Specifies the type of string that is to be returned when searching for a file or protocol association.
/// </summary>
internal enum AssociationStringType
{
    /// <summary>
    /// A command string associated with a Shell verb.
    /// </summary>
    Command = 1,
    /// <summary>
    /// An executable from a Shell verb.
    /// </summary>
    Executable
}
