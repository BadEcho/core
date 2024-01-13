//-----------------------------------------------------------------------
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

#pragma once
#define WIN32_LEAN_AND_MEAN

#include <windows.h>

/**
 * Specifies a type of hook procedure.
 */
enum HookType
{	
	/**
	 * Monitors \c WH_CALLWNDPROC messages before the system sends them to a destination window
	 * procedure.
	 */
	WindowProcPreview,
	/**
	 * Monitors \c WH_CALLWNDPROCRET messages after they have been processed by the destination
	 * window procedure.
	 */
	WindowProcReturn,
	/**
	 * Monitors \c WH_GETMESSAGE messages posted to a message queue prior to their retrieval.
	 */
	MessageQueueRead
};

enum
{
	SharedMemorySize = 1120,
	MaxThreads = 20
};

#define HOOKS_API extern "C" __declspec(dllexport)

/**
 * Installs a new Win32 hook procedure into the specified thread.
 * @param hookType The type of hook procedure to install.
 * @param threadId The identifier of the thread with which the hook procedure is to be associated.
 * @param destination A handle to the window that will receive messages sent to the hook procedure.
 * @return True if successful; otherwise, false.
 */
HOOKS_API bool __cdecl AddHook(HookType hookType, int threadId, HWND destination);

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
LRESULT CALLBACK GetMsgProc(int code, WPARAM wParam, LPARAM lParam);

/**
 * Represents configurations settings for a hook procedure.
 */
struct HookData
{
	/**
	 * A handle to the hook procedure.
	 */
	HHOOK Handle;
	/**
	 * A handle to the window that hook messages will be sent to.
	 */
	HWND Destination;
};

/**
 * Represents shared hook data specific to a thread.
 */
struct ThreadData
{
	/**
	 * The thread the data is associated with.
	 */
	int ThreadId;
	/**
	 * The installed \c WH_CALLWNDPROC hook procedure for the thread, if one exists.
	 */
	HookData CallWndProcHook;
	/**
	 * The installed \c WH_CALLWNDPROCRET hook procedure for the thread, if one exists.
	 */
	HookData CallWndProcRetHook;
	/**
	 * The installed \c WH_GETMESSAGE hook procedure for the thread, if one exists.
	 */
	HookData GetMessageHook;
};

inline ThreadData* SharedData;
inline LPVOID SharedMemory = nullptr;
inline HANDLE FileMapping = nullptr;
inline HANDLE SharedSectionMutex = nullptr;

// Add a data section to our binary file for variables we want shared across all injected
// processes.
// The variables that are shared mainly deal with the number of active hooks and message
// parameters up for modification.
#pragma data_seg(".shared")
inline bool ChangeMessage = false;
inline UINT ChangedMessage = 0;
inline WPARAM ChangedWParam = 0;
inline LPARAM ChangedLParam = 0;
inline int ThreadCount = 0;
#pragma data_seg()
#pragma comment(linker, "/SECTION:.shared,RWS")

inline HINSTANCE Instance;

template<typename T>
T* PointTo(uintptr_t address)
{
	char* p = nullptr;
	p += address;

	return reinterpret_cast<T*>(p);
}