//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin.Interop
{
    /// <summary>
    /// Defines a wrapper around an <c>HWND</c> of a window and the messages it receives.
    /// </summary>
    public interface IWindowWrapper
    {
        /// <summary>
        /// Adds a hook that will received messages sent to the wrapped window.
        /// </summary>
        /// <param name="hook">The hook to invoke when messages are sent to the wrapped window.</param>
        void AddHook(WindowProc hook);

        /// <summary>
        /// Removes a hook previously receiving messages sent to the wrapped window.
        /// </summary>
        /// <param name="hook">The hook to remove.</param>
        void RemoveHook(WindowProc hook);
    }
}
