﻿//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Threading;

/// <summary>
/// A callback to use for thread executor operations.
/// </summary>
/// <param name="argument">An argument to pass to the callback.</param>
/// <returns>The result of the callback.</returns>
internal delegate object? ThreadExecutorOperationCallback(object? argument);