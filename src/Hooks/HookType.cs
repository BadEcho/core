using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadEcho.Hooks;

/// <summary>
/// Specifies a type of hook procedure.
/// </summary>
internal enum HookType
{
    /// <summary>
    /// Monitors <c>WH_CALLWNDPROC</c> messages before the system sends them to a destination window procedure.
    /// </summary>
    CallWindowProcedure,
    /// <summary>
    /// Monitors <c>WH_CALLWNDPROCRET</c> messages after they have been processed by the destination window
    /// procedure.
    /// </summary>
    CallWindowProcedureReturn,
    /// <summary>
    /// Monitors <c>WH_GETMESSAGE</c> messages posted to a message queue prior to their retrieval.
    /// </summary>
    GetMessage
}
