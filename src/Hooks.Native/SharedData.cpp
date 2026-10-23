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

    int ResolveThreadId(int threadId)
    {   // Global hook data is registered to the thread that installed the hook. 
        // Fallback to the current thread if a thread ID of 0 (i.e., global) is specified.
        return threadId != 0 ? threadId : static_cast<int>(GetCurrentThreadId());
    }

    bool HasHooks(const ThreadData& threadData)
    {
        return threadData.CallWndProcHook.Handle != nullptr
            || threadData.CallWndProcRetHook.Handle != nullptr
            || threadData.GetMessageHook.Handle != nullptr
            || threadData.KeyboardHook.Handle != nullptr
            || threadData.LowLevelKeyboardHook.Handle != nullptr
            || threadData.MouseHook.Handle != nullptr
            || threadData.LowLevelMouseHook.Handle != nullptr;
    }

    bool IsLive(const HookData* hookData)
    {
        return hookData != nullptr && hookData->Handle != nullptr && IsWindow(hookData->Destination);
    }

    bool IsGlobalHookTaken(HookType hookType, int threadId)
    {
        int* globalId = GetGlobalId(hookType);

        if (globalId == nullptr || *globalId == 0 || *globalId == threadId)
            return false;

        int ownerIndex = FindThreadDataIndex(*globalId);

        return ownerIndex != MaxThreads && IsLive(GetThreadHookData(hookType, &SharedData[ownerIndex]));
    }
}

SharedDataLock::SharedDataLock()
    : _acquired(false)
{
    if (SharedSectionMutex == nullptr)
        return;

    DWORD waitResult = WaitForSingleObject(SharedSectionMutex, INFINITE);

    // An abandoned mutex is still one that has been acquired; it's previous owned exited without releasing it.
    _acquired = waitResult == WAIT_OBJECT_0 || waitResult == WAIT_ABANDONED;
}

SharedDataLock::~SharedDataLock()
{
    if (_acquired)
        ReleaseMutex(SharedSectionMutex);
}

bool SharedDataLock::IsAcquired() const
{
    return _acquired;
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

    threadId = ResolveThreadId(threadId);

    if (isGlobal && IsGlobalHookTaken(hookType, threadId))
    {
        SetLastError(ERROR_ALREADY_REGISTERED);
        return nullptr;
    }

    int index = FindThreadDataIndex(threadId);

    if (index == MaxThreads)
    {   // Thread not registered -- claim a free slot for it. Threads can be removed in any order, so a free slot
        // may be anywhere in the array.
        index = FindThreadDataIndex(0);

        if (index == MaxThreads)
        {   // We're at our storage limit.
            SetLastError(ERROR_OUT_OF_STRUCTURES);
            return nullptr;
        }

        SharedData[index] = ThreadData{ };
        // Read-only operations won't be locking, so we need to guarantee these writes are in order.
        MemoryBarrier();
        SharedData[index].ThreadId = threadId;
    }
    else if (IsLive(GetThreadHookData(hookType, &SharedData[index])))
    {   // Only one hook procedure of each type can be registered to a thread.
        SetLastError(ERROR_ALREADY_REGISTERED);
        return nullptr;
    }

    if (isGlobal)
        UpdateGlobalId(hookType, threadId);

    return GetThreadHookData(hookType, &SharedData[index]);
}

HookData* GetHookData(HookType hookType, int threadId, HookDataScope scope)
{   
    HookData* hookData = nullptr;

    if (int index = FindThreadDataIndex(ResolveThreadId(threadId)); index != MaxThreads)
        hookData = GetThreadHookData(hookType, &SharedData[index]);

    // Return the thread's own entry if the caller only wants the thread's registration...
    if (scope == HookDataScope::Registered)
        return hookData;

    // ...or if that entry does have a hook installed, which takes precedence over a global one of the same type.
    if (hookData != nullptr && hookData->Handle != nullptr)
        return hookData;

    int* globalId = GetGlobalId(hookType);

    if (globalId == nullptr || *globalId == 0)
        return nullptr;

    int globalIndex = FindThreadDataIndex(*globalId);

    return globalIndex != MaxThreads ? GetThreadHookData(hookType, &SharedData[globalIndex]) : nullptr;
}

void RemoveHookData(HookType hookType, int threadId)
{
    bool isGlobal = threadId == 0;

    threadId = ResolveThreadId(threadId);

    int index = FindThreadDataIndex(threadId);

    if (index == MaxThreads)
        return;

    ThreadData* threadData = &SharedData[index];
    HookData* hookData = GetThreadHookData(hookType, threadData);

    if (hookData == nullptr)
        return;

    if (isGlobal)
    {   // Global registrations are only cleared if its being done from the thread that installed it.
        if (int* globalId = GetGlobalId(hookType); globalId != nullptr && *globalId == threadId)
            *globalId = 0;
    }

    hookData->Destination = nullptr;
    hookData->Handle = nullptr;

    if (HasHooks(*threadData))
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