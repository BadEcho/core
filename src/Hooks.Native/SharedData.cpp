// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

#include "SharedData.h"

namespace {
    ThreadData* SharedData = nullptr;
    LPVOID SharedMemory = nullptr;
    HANDLE FileMapping = nullptr;

    // The data written to the shared memory is laid out differently between 32-bit and 64-bit platforms;
    // they also have their own .shared segment, so each platform gets its own set of shared objects.
#ifdef _WIN64
    constexpr LPCTSTR FileMappingName = TEXT("BadEcho.Hooks.FileMappingObject.x64");
    constexpr LPCTSTR MutexName = TEXT("BadEcho.Hooks.MutexObject.x64");
#else
    constexpr LPCTSTR FileMappingName = TEXT("BadEcho.Hooks.FileMappingObject.x86");
    constexpr LPCTSTR MutexName = TEXT("BadEcho.Hooks.MutexObject.x86");
#endif
    
    int* GetGlobalId(HookType hookType)
    {
        switch (hookType)
        {
        case CallWindowProcedure:
            return &GlobalCallWndProcId;
        case CallWindowProcedureReturn:
            return &GlobalCallWndProcRetId;
        case GetMessages:
            return &GlobalGetMessageId;
        // Input related global hooks are executed in the context of the installing thread.
        case Keyboard:
        case LowLevelKeyboard:
        case Mouse:
        case LowLevelMouse:
        default:
            return nullptr;
        }
    }

    void UpdateGlobalId(HookType hookType, int threadId)
    {
        if (int* globalId = GetGlobalId(hookType); globalId != nullptr)
            *globalId = threadId;
    }

    int FindThreadDataIndex(int threadId)
    {
        int index;

        for (index = 0; index < MaxThreads; index++)
        {
            if (SharedData[index].ThreadId == threadId)
                break;
        }

        return index;
    }

    ThreadData* GetThreadData(HookType hookType, int threadId)
    {
        int index = threadId != 0 ? FindThreadDataIndex(threadId) : MaxThreads;

        if (index == MaxThreads)
        {
            if (int* globalId = GetGlobalId(hookType); globalId != nullptr && *globalId != 0)
                index = FindThreadDataIndex(*globalId);

            if (index == MaxThreads)
                return nullptr;
        }

        return &SharedData[index];
    }

    HookData* GetThreadHookData(HookType hookType, ThreadData* threadData)
    {
        if (threadData == nullptr)
            return nullptr;

        switch (hookType)
        {
	        case CallWindowProcedure:
	            return &threadData->CallWndProcHook;
	        case CallWindowProcedureReturn:
	            return &threadData->CallWndProcRetHook;
	        case GetMessages:
	            return &threadData->GetMessageHook;
	        case Keyboard:
	            return &threadData->KeyboardHook;
	        case LowLevelKeyboard:
	            return &threadData->LowLevelKeyboardHook;
            case Mouse:
                return &threadData->MouseHook;
            case LowLevelMouse:
                return &threadData->LowLevelMouseHook;				
        }

        return nullptr;
    }
}

// Mutex for synchronizing writes to shared memory, particularly for message parameter modification by message queue hook procedures.
HANDLE SharedSectionMutex = nullptr;

// Adds a data section to our binary file for variables we want shared across all processes.
#pragma data_seg(".shared")
bool ChangeMessage = false;
UINT ChangedMessage = 0;
WPARAM ChangedWParam = 0;
LPARAM ChangedLParam = 0;
int GlobalCallWndProcId = 0;
int GlobalCallWndProcRetId = 0;
int GlobalGetMessageId = 0;
#pragma data_seg()
#pragma comment(linker, "/SECTION:.shared,RWS") 


bool InitializeSharedData()
{
    FileMapping = CreateFileMapping(
        INVALID_HANDLE_VALUE,
        nullptr,
        PAGE_READWRITE,
        0,
        SharedMemorySize,
        FileMappingName);

    if (FileMapping == nullptr)
        return false;

    bool init = GetLastError() != ERROR_ALREADY_EXISTS;

    SharedMemory
        = MapViewOfFile(FileMapping, FILE_MAP_WRITE, 0, 0, 0);

    if (SharedMemory == nullptr)
        return false;

    SharedSectionMutex
        = CreateMutex(nullptr, FALSE, MutexName);

    if (SharedSectionMutex == nullptr)
        return false;

    if (init)
        memset(SharedMemory, '\0', SharedMemorySize);

    SharedData = static_cast<ThreadData*>(SharedMemory);

    return true;
}

void CloseSharedData()
{
    UnmapViewOfFile(SharedMemory);
    CloseHandle(FileMapping);
    CloseHandle(SharedSectionMutex);
}

HookData* AddHookData(HookType hookType, int threadId)
{
    bool isGlobal = threadId == 0;

    if (isGlobal)
        threadId = static_cast<int>(GetCurrentThreadId());
    
	// Synchronization is required as multiple processes may be attempting to claim a free slot.
    WaitForSingleObject(SharedSectionMutex, INFINITE);

    int index = FindThreadDataIndex(threadId);

    if (index == MaxThreads)
    {   // Thread not registered -- claim a free slot for it. Threads can be removed in any order, so a free slot
    	// may be anywhere in the array.
        index = FindThreadDataIndex(0);

        if (index == MaxThreads)
        {   // We're at our storage limit.            
            ReleaseMutex(SharedSectionMutex);
            return nullptr;
        }

        SharedData[index] = ThreadData{ };
        SharedData[index].ThreadId = threadId;
    }

    if (isGlobal)
        UpdateGlobalId(hookType, threadId);

    ReleaseMutex(SharedSectionMutex);
    
    return GetThreadHookData(hookType, &SharedData[index]);
}

HookData* GetHookData(HookType hookType, int threadId)
{    
    if (threadId == 0)
        threadId = static_cast<int>(GetCurrentThreadId());

    ThreadData* threadData = GetThreadData(hookType, threadId);

    if (threadData == nullptr)
        return nullptr;

    return GetThreadHookData(hookType, threadData);
}

void RemoveHookData(HookType hookType, int threadId)
{
    ThreadData* threadData = GetThreadData(hookType, threadId);

    if (threadData == nullptr)
        return;

    HookData* hookData = GetThreadHookData(hookType, threadData);

    if (hookData == nullptr)
        return;

    if (threadId == 0)
        UpdateGlobalId(hookType, 0);

    hookData->Handle = nullptr;
    hookData->Destination = nullptr;

    if (threadData->CallWndProcHook.Handle != nullptr)
        return;

    if (threadData->CallWndProcRetHook.Handle != nullptr)
        return;

    if (threadData->GetMessageHook.Handle != nullptr)
        return;

    if (threadData->KeyboardHook.Handle != nullptr)
        return;

    if (threadData->LowLevelKeyboardHook.Handle != nullptr)
        return;

    if (threadData->MouseHook.Handle != nullptr)
        return;

    if (threadData->LowLevelMouseHook.Handle != nullptr)
        return;

    // "Free" the thread, as it no longer has any hooks associated with it.
    threadData->ThreadId = 0;
}
