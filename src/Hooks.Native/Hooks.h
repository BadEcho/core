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

#pragma once
#define WIN32_LEAN_AND_MEAN

#include <windows.h>

/**
 * Specifies a type of hook procedure.
 */
enum HookType : unsigned char
{	
	/**
	 * Monitors \c WH_CALLWNDPROC messages before the system sends them to the destination window
	 * procedure.
	 */
	CallWindowProcedure,
	/**
	 * Monitors \c WH_CALLWNDPROCRET messages after they have been processed by the destination
	 * window procedure.
	 */
	CallWindowProcedureReturn,
	/**
	 * Monitors \c WH_GETMESSAGE messages posted to a message queue prior to their retrieval.
	 * @remarks This is named \c GetMessages to avoid conflicting with the ever-present \c GetMessage Win32 macro.
	 */
	GetMessages,
	/**
	 * Monitors \c WH_KEYBOARD keystroke messages.
	 */
	Keyboard,
	/**
	 * Monitors \c WH_KEYBOARD_LL low-level keyboard input events.
	 */
	LowLevelKeyboard
};

#define HOOKS_API extern "C" __declspec(dllexport)

/**
 * Installs a new Win32 hook procedure into the specified thread.
 * @param hookType The type of hook procedure to install.
 * @param destination A handle to the window that will receive messages sent to the hook procedure.
 * @param threadId The identifier of the thread with which the hook procedure is to be associated.
 * @return True if successful; otherwise, false.
 */
HOOKS_API bool __cdecl AddHook(HookType hookType, HWND destination, int threadId);

/**
 * Uninstalls a Win32 hook procedure from the specified thread.
 * @param hookType The type of hook procedure to uninstall.
 * @param threadId The identifier of the thread to remove the hook procedure from.
 * @return True if successful; otherwise, false.
 */
HOOKS_API bool __cdecl RemoveHook(HookType hookType, int threadId);

/**
 * Changes the details of a hook message currently being intercepted.
 * @param message The message identifier to use.
 * @param wParam Additional information about the message to use.
 * @param lParam Additional information about the message to use.
 * @note
 * This function should only be called from window procedures that handle hook types supporting
 * mutable messages.
 */
HOOKS_API void __cdecl ChangeMessageDetails(UINT message, WPARAM wParam, LPARAM lParam);

// Installable hook procedures.

LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK CallWndProcRet(int nCode, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam);

/**
 * Interprets the data at a specified address (which is typically what is provided by LPARAM in window messages) as a type.
 * @tparam T The type to interpret the data as.
 * @param address The address in memory where the data being interpreted resides.
 * @return A value of type \c T.
 */
template<typename T>
T* PointTo(uintptr_t address)
{
	char* p = nullptr;
	p += address;

	return reinterpret_cast<T*>(p);
}