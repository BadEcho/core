﻿//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
/// Provides the result of a hook procedure's processing of a message.
/// </summary>
/// <param name="LResult">The response to the particular message.</param>
/// <param name="Handled">A value that indicates whether the message was handled.</param>
public sealed record HookResult(IntPtr LResult, bool Handled);