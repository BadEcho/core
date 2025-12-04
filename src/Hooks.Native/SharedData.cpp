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

#include "SharedData.h"

namespace {
    ThreadData* SharedData = nullptr;
    LPVOID SharedMemory = nullptr;
    HANDLE FileMapping = nullptr;
    
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

        for (index = 0; index < ThreadCount; index++)
        {
            if (SharedData[index].ThreadId == threadId)
                break;
        }

        return index;
    }

    ThreadData* GetThreadData(HookType hookType, int threadId)
    {
        int index = FindThreadDataIndex(threadId);

        if (index == ThreadCount || threadId == 0)
        {
            if (int* globalId = GetGlobalId(hookType); globalId != nullptr)
                index = FindThreadDataIndex(*globalId);

            if (index == ThreadCount)
                return nullptr;
        }

        return &SharedData[index];
    }

    HookData* GetThreadHookData(HookType hookType, ThreadData* threadData)
    {
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
int ThreadCount = 0;
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
        TEXT("BadEcho.Hooks.FileMappingObject"));

    if (FileMapping == nullptr)
        return false;

    bool init = GetLastError() != ERROR_ALREADY_EXISTS;

    SharedMemory
        = MapViewOfFile(FileMapping, FILE_MAP_WRITE, 0, 0, 0);

    if (SharedMemory == nullptr)
        return false;

    SharedSectionMutex
        = CreateMutex(nullptr, FALSE, TEXT("BadEcho.Hooks.MutexObject"));

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

    int index = FindThreadDataIndex(threadId);

    if (index == ThreadCount)
    {   // Thread not registered -- attempt to initialize data.
        if (ThreadCount == MaxThreads)
        {   // We're at our storage limit -- check if any threads have been freed.
            for (index = 0; index < ThreadCount; index++)
            {
                if (SharedData[index].ThreadId == 0)
                    break;
            }

            if (index == MaxThreads)
                return nullptr;
        }

        SharedData[index].ThreadId = threadId;
        
        SharedData[index].CallWndProcHook.Handle = nullptr;
        SharedData[index].CallWndProcHook.Destination = nullptr;
        SharedData[index].CallWndProcRetHook.Handle = nullptr;
        SharedData[index].CallWndProcRetHook.Destination = nullptr;
        SharedData[index].GetMessageHook.Handle = nullptr;
        SharedData[index].GetMessageHook.Destination = nullptr;
        SharedData[index].KeyboardHook.Handle = nullptr;
        SharedData[index].KeyboardHook.Destination = nullptr;
        SharedData[index].LowLevelKeyboardHook.Handle = nullptr;
        SharedData[index].LowLevelKeyboardHook.Destination = nullptr;

        // Synchronization is required as multiple processes may be attempting to increment the
        // thread count.
        WaitForSingleObject(SharedSectionMutex, INFINITE);
        ThreadCount++;
        ReleaseMutex(SharedSectionMutex);
    }

    if (isGlobal)
        UpdateGlobalId(hookType, threadId);
    
    return GetThreadHookData(hookType, &SharedData[index]);
}

HookData* GetHookData(HookType hookType, int threadId)
{    
    ThreadData* threadData = GetThreadData(hookType, threadId);

    return GetThreadHookData(hookType, threadData);
}

void RemoveHookData(HookType hookType, int threadId)
{
    ThreadData* threadData = GetThreadData(hookType, threadId);

    if (threadData == nullptr)
        return;

    HookData* hookData = GetThreadHookData(hookType, threadData);

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

    // "Free" the thread, as it no longer has any hooks associated with it.
    threadData->ThreadId = 0;
    
	WaitForSingleObject(SharedSectionMutex, INFINITE);
    ThreadCount--;
    ReleaseMutex(SharedSectionMutex);
}
