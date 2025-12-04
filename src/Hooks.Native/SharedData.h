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

#include "Hooks.h"

/**
 * Represents configuration settings for a hook procedure.
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
	HookData CallWndProcHook;  // NOLINT(clang-diagnostic-padded) Compiler will do the padding for us.
	/**
	 * The installed \c WH_CALLWNDPROCRET hook procedure for the thread, if one exists.
	 */
	HookData CallWndProcRetHook;
	/**
	 * The installed \c WH_GETMESSAGE hook procedure for the thread, if one exists.
	 */
	HookData GetMessageHook;
	/**
	 * The installed \c WH_KEYBOARD_LL hook procedure for the thread, if one exists.
	 */
	HookData LowLevelKeyboardHook;
	/**
	 * The installed \c WH_KEYBOARD hook procedure for the thread, if one exists.
	 */
	HookData KeyboardHook;
};

/**
 * The maximum number of threads that can be associated with one or more hook procedures.
 */
constexpr int MaxThreads = 20;
/**
 * The size allocated for the shared memory used to store hook data.
 */
constexpr int SharedMemorySize = 1760;

/**
 * Initializes various shared memory and synchronization objects used for communication between processes.
 * @return True if the shared data was successfully initialized; otherwise, false.
 */
bool InitializeSharedData();

/**
 * Cleans up the resources involved with the previously initialized shared memory and synchronization objects.
 */
void CloseSharedData();

/**
 * Associates a type of hook data with a thread.
 * @param hookType The type of hook data to add.
 * @param threadId The identifier of the thread to associate the hook data with.
 * @return A pointer to the hook data if successful; otherwise a \c nullptr if the limits on shared data storage have been exceeded.
 */
HookData* AddHookData(HookType hookType, int threadId);

/**
 * Retrieves hook data associated with a thread for a particular type of hook.
 * @param hookType The type of hook data to retrieve.
 * @param threadId The identifier of the thread associated with the hook data.
 * @return A pointer to the requested type of hook data, if one exists; otherwise, a \c nullptr.
 * @remarks If a global hook of the requested type has been installed, then that will be returned instead if no other hook data has been
 *			associated with the specified thread. This is done because most types of global hook procedures are called in the process
 *			context of every application on the desktop, so we will have no record of the threads executing them.
 */
HookData* GetHookData(HookType hookType, int threadId);

/**
 * Disassociates a type of hook data from a thread.
 * @param hookType The type of hook data to disassociate from the thread.
 * @param threadId The identifier of the thread to disassociate the hook data from.
 * @remarks A thread can have multiple types of hook data associated with it. Only when all hook types have been disassociated from a thread
 *			will the block of shared memory set aside for it be freed.
 */
void RemoveHookData(HookType hookType, int threadId);

/**
 * Handle to a mutex used to synchronize write access to any variable in the DLL's shared data segment.
 */
extern HANDLE SharedSectionMutex;

// Shared data segment variables.

/**
 * Value indicating if an intercepted message queue message's parameters have been modified.
 */
extern bool ChangeMessage;
/**
 * The updated message parameter for a message intercepted from a message queue.
 */
extern UINT ChangedMessage;
/**
 * The updated wParam parameter for a message intercepted from a message queue.
 */
extern WPARAM ChangedWParam;
/**
 * The updated lParam parameter for a message intercepted from a message queue.
 */
extern LPARAM ChangedLParam;
/**
 * The number of threads that currently have hook data associated with them.
 */
extern int ThreadCount;
/**
 * The identifier for the thread that installed a global \c CallWndProc hook procedure.
 */
extern int GlobalCallWndProcId;
/**
 * The identifier for the thread that installed a global \c CallWndProcRet hook procedure.
 */
extern int GlobalCallWndProcRetId;
/**
 * The identifier for the thread that installed a global \c GetMessage hook procedure.
 */
extern int GlobalGetMessageId;