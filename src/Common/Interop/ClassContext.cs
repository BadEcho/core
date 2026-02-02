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

namespace BadEcho.Interop;

/// <summary>
/// Specifies execution context in which an object is to be run.
/// </summary>
[Flags]
internal enum ClassContext
{
    /// <summary>
    /// The code that creates and manages objects of this class runs in the same process as the caller of the function
    /// specifying this context.
    /// </summary>
    InProcServer = 0x1,
    /// <summary>
    /// The code that manages objects of this class is an in-process handler.
    /// </summary>
    InProcHandler = 0x2,
    /// <summary>
    /// The code that creates and manages objects of this class runs on the same machine but is loaded in a separate process space.
    /// </summary>
    LocalServer = 0x4,
    /// <summary>
    /// A remote context.
    /// </summary>
    RemoteServer = 0x10,
    /// <summary>
    /// All server contexts.
    /// </summary>
    Server = InProcServer | LocalServer | RemoteServer,
    /// <summary>
    /// All contexts.
    /// </summary>
    All = Server | InProcHandler
}
