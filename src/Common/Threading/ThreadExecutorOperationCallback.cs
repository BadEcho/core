//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Threading;

/// <summary>
/// A callback to use for thread executor operations.
/// </summary>
/// <param name="argument">An argument to pass to the callback.</param>
/// <returns>The result of the callback.</returns>
internal delegate object? ThreadExecutorOperationCallback(object? argument);
