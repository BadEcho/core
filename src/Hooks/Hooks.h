//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
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

enum HookType
{
	WindowProcPreview,
	WindowProcReturn,
	MessageQueueRead
};

#define HOOKS_API extern "C" __declspec(dllexport)

HOOKS_API bool __cdecl AddHook(HookType hookType, int threadId, HWND destination);
HOOKS_API bool __cdecl RemoveHook(HookType hookType, int threadId);

LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK CallWndProcRet(int nCode, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK GetMsgProc(int code, WPARAM wParam, LPARAM lParam);

enum
{
	SharedMemorySize = 1120,
	MaxThreads = 20
};

struct HookData
{
	HHOOK Handle;
	HWND Destination;
};

struct ThreadData
{
	int ThreadId;
	HookData CallWndProcHook;
	HookData CallWndProcRetHook;
	HookData GetMessageHook;
};

inline ThreadData* SharedData;
static LPVOID SharedMemory = nullptr;
static HANDLE MapObject = nullptr;


// Add a data section to our binary file for variables we want shared across all injected processes.
// The variables that are shared mainly deal with the number of active hooks and message parameters up for modification.
#pragma data_seg(".shared")
inline bool ModifyMessage = false;
inline int CurrentMessage = 0;
inline int CurrentWParam = 0;
inline int CurrentLParam = 0;
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