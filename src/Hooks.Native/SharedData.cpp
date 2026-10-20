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
#include <sddl.h>
#include <strsafe.h>

namespace {
    ThreadData* SharedData = nullptr;
    LPVOID SharedMemory = nullptr;
    HANDLE FileMapping = nullptr;
    // Mutex for synchronizing the claiming of thread slots in shared memory.
    HANDLE SharedSectionMutex = nullptr;

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
    {   // A thread ID of 0 indicates a global hook. We don't want to ever search for zero here, since free slots will have an identifier of zero.
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

    /**
     * Creates the security descriptor applied to the shared objects used by this library.
     * @return The security descriptor, or \c nullptr if one could not be created.
     * @remarks
     * This grants full access to SYSTEM and the current user, with objects labeled at medium integrity.
     * This means any process running as the same user at medium integrity or higher can access these objects.
     * A custom security descriptor is used to prevent the scenario where a hook, initially installed by an elevated
     * process, will result in shared objects that cannot be accessed by normal, medium integrity processes created 
     * by the same user.
     */
    PSECURITY_DESCRIPTOR CreateSharedSecurityDescriptor()
    {
        HANDLE token;

        if (!OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &token))
            return nullptr;

        // This is typed as a BYTE array, however we need to it to start at a memory address suitable for a TOKEN_USER.
        // Two things get written into this buffer: the fixed-sized TOKEN_USER struct, followed by the user's SID, so we size for both.
        alignas(TOKEN_USER) BYTE tokenUser[sizeof(TOKEN_USER) + SECURITY_MAX_SID_SIZE] {};
        DWORD returnLength;

        BOOL hasUser = GetTokenInformation(token, TokenUser, tokenUser, sizeof(tokenUser), &returnLength);

        CloseHandle(token);

        if (!hasUser)
            return nullptr;

        LPTSTR userSid;

        // We'll be building a SDDL string, the most compact way to define a security descriptor, so we prep the SID for insertion.
        if (!ConvertSidToStringSid(reinterpret_cast<TOKEN_USER*>(tokenUser)->User.Sid, &userSid))
            return nullptr;

        TCHAR sddl[256];

        // This SDDL essentially disables inheritance, grants all access to SYSTEM and the local user, prevents lower integrity
        // processes from accessing it, and allows medium or higher.
        HRESULT formatResult = StringCchPrintf(sddl, ARRAYSIZE(sddl), TEXT("D:P(A;;GA;;;SY)(A;;GA;;;%s)S:(ML;;NWNR;;;ME)"), userSid);

        LocalFree(userSid);

        if (FAILED(formatResult))
            return nullptr;

        PSECURITY_DESCRIPTOR descriptor;

        if (!ConvertStringSecurityDescriptorToSecurityDescriptor(sddl, SDDL_REVISION_1, &descriptor, nullptr))
            return nullptr;

        return descriptor;
    }
}

// Adds a data section to our binary file for variables we want shared across all processes.
#pragma data_seg(".shared")
int GlobalCallWndProcId = 0;
int GlobalCallWndProcRetId = 0;
int GlobalGetMessageId = 0;
#pragma data_seg()
#pragma comment(linker, "/SECTION:.shared,RWS") 

bool InitializeSharedData()
{
    PSECURITY_DESCRIPTOR descriptor = CreateSharedSecurityDescriptor();
    SECURITY_ATTRIBUTES attributes{ sizeof(SECURITY_ATTRIBUTES), descriptor, false };
    LPSECURITY_ATTRIBUTES securityAttributes = descriptor != nullptr ? &attributes : nullptr;

    FileMapping = CreateFileMapping(
        INVALID_HANDLE_VALUE,
        securityAttributes,
        PAGE_READWRITE,
        0,
        SharedMemorySize,
        FileMappingName);
    
    bool init = false;

    if (FileMapping != nullptr)
    {
        init = GetLastError() != ERROR_ALREADY_EXISTS;

        SharedSectionMutex
            = CreateMutex(securityAttributes, FALSE, MutexName);
    }

    if (descriptor != nullptr)
        LocalFree(descriptor);

    if (FileMapping == nullptr || SharedSectionMutex == nullptr)
        return false;

    SharedMemory
        = MapViewOfFile(FileMapping, FILE_MAP_WRITE, 0, 0, 0);

    if (SharedMemory == nullptr)
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

MessageChanges* GetMessageChanges(int threadId)
{
    if (threadId == 0)
        return nullptr;

    int index = FindThreadDataIndex(threadId);

    return index != MaxThreads ? &SharedData[index].MessageChanges : nullptr;
}