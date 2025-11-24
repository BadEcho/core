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

#include "Hooks.h"

namespace {
#pragma data_seg(".shared")
    // Adds a data section to our binary file for variables we want shared across all processes.
    // The variables that are shared mainly deal with the number of active hooks and message
    // parameters up for modification.
    bool ChangeMessage = false;
    UINT ChangedMessage = 0;
    WPARAM ChangedWParam = 0;
    LPARAM ChangedLParam = 0;
    int ThreadCount = 0;
#pragma data_seg()
#pragma comment(linker, "/SECTION:.shared,RWS") 

    HINSTANCE Instance;
    ThreadData* SharedData;
    LPVOID SharedMemory = nullptr;
    HANDLE FileMapping = nullptr;
    // Mutex for synchronizing writes to shared memory, particularly for message parameter modification by message queue hook procedures.
    HANDLE SharedSectionMutex = nullptr; 

    ThreadData* GetLocalData(int threadId, bool addEntry)
    {
        int index;

        for (index = 0; index < ThreadCount; index++)
        {
            if (SharedData[index].ThreadId == threadId)
                break;
        }

        // Thread not registered -- attempt to initialize data.
        if (index == ThreadCount)
        {
            if (!addEntry || ThreadCount == MaxThreads)
                return nullptr;

            SharedData[index].ThreadId = threadId;

            SharedData[index].CallWndProcHook.Handle = nullptr;
            SharedData[index].CallWndProcHook.Destination = nullptr;
            SharedData[index].CallWndProcRetHook.Handle = nullptr;
            SharedData[index].CallWndProcRetHook.Destination = nullptr;
            SharedData[index].GetMessageHook.Handle = nullptr;
            SharedData[index].GetMessageHook.Destination = nullptr;

            // Synchronization is required as multiple processes may be attempting to increment the
            // thread count.
            WaitForSingleObject(SharedSectionMutex, INFINITE);
            ThreadCount++;
            ReleaseMutex(SharedSectionMutex);
        }

        return &SharedData[index];
    }
}

BOOL APIENTRY DllMain(HINSTANCE instance, DWORD reason, LPVOID)  // NOLINT(misc-use-internal-linkage) 'static' is ignored for DllMain by compiler
{
    BOOL init;

    switch (reason)
    {
    	case DLL_PROCESS_ATTACH:        
            Instance = instance;
            FileMapping = CreateFileMapping(
                INVALID_HANDLE_VALUE,
                nullptr,
                PAGE_READWRITE,
                0,
                SharedMemorySize,
                TEXT("BadEcho.Hooks.FileMappingObject"));            

            if (FileMapping == nullptr)
                return FALSE;

            init = GetLastError() != ERROR_ALREADY_EXISTS;

            SharedMemory
    			= MapViewOfFile(FileMapping, FILE_MAP_WRITE, 0, 0, 0);

            if (SharedMemory == nullptr)
                return FALSE;

            SharedSectionMutex
    			= CreateMutex(nullptr, FALSE, TEXT("BadEcho.Hooks.MutexObject"));

            if (SharedSectionMutex == nullptr)
                return FALSE;

            if (init)
                memset(SharedMemory, '\0', SharedMemorySize);

            SharedData = static_cast<ThreadData*>(SharedMemory);
            break;

        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
            break;   	
    	case DLL_PROCESS_DETACH:
            UnmapViewOfFile(SharedMemory);
            CloseHandle(FileMapping);
            CloseHandle(SharedSectionMutex);
            break;
    	default:
            return FALSE;
    }

	return TRUE;    
}

bool __cdecl AddHook(HookType hookType, int threadId, HWND destination)
{
    ThreadData* localData = GetLocalData(threadId, true);

    if (localData == nullptr)
        return false;
    
    HookData* hookData;
    int idHook;
    HOOKPROC lpfn;

    switch (hookType)
    {
    	case CallWindowProcedure:
            hookData = &localData->CallWndProcHook;
            idHook = WH_CALLWNDPROC;
            lpfn = CallWndProc;
            break;

    	case CallWindowProcedureReturn:
            hookData = &localData->CallWndProcRetHook;
            idHook = WH_CALLWNDPROCRET;
            lpfn = CallWndProcRet;
            break;

    	case GetMessages:
            hookData = &localData->GetMessageHook;
            idHook = WH_GETMESSAGE;
            lpfn = GetMsgProc;
            break;

		default:
            return false;
    }

    HHOOK hook = SetWindowsHookEx(idHook, lpfn, Instance, threadId);

    if (hook == nullptr)
        return false;

    hookData->Handle = hook;
    hookData->Destination = destination;

    return true;
}

bool __cdecl RemoveHook(HookType hookType, int threadId)
{
    ThreadData* localData = GetLocalData(threadId, false);

    if (localData == nullptr)
        return false;

    HookData* hookData;

    switch (hookType)
    {
		case CallWindowProcedure:
            hookData = &localData->CallWndProcHook;
            break;
		case CallWindowProcedureReturn:
            hookData = &localData->CallWndProcRetHook;
            break;
		case GetMessages:
            hookData = &localData->GetMessageHook;
            break;
		default:
            return false;
    }

    if (hookData->Handle == nullptr)
        return false;

    bool result = UnhookWindowsHookEx(hookData->Handle);

    hookData->Handle = nullptr;
    hookData->Destination = nullptr;

    return result;
}

void __cdecl ChangeMessageDetails(UINT message, WPARAM wParam, LPARAM lParam)
{
    ChangedMessage = message;
    ChangedWParam = wParam;
    ChangedLParam = lParam;
    ChangeMessage = true;
}


LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    int threadId = static_cast<int>(GetCurrentThreadId());

    if (ThreadData* localData = GetLocalData(threadId, false); nCode == HC_ACTION)
    {   
        HWND destination = localData->CallWndProcHook.Destination;

        auto messageParameters = PointTo<CWPSTRUCT>(lParam);

        if (destination != nullptr)
            SendMessage(destination, messageParameters->message, messageParameters->wParam, messageParameters->lParam);
    }    

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

LRESULT CALLBACK CallWndProcRet(int nCode, WPARAM wParam, LPARAM lParam)
{
    int threadId = static_cast<int>(GetCurrentThreadId());    

    if (ThreadData* localData = GetLocalData(threadId, false); nCode == HC_ACTION)
    {
        HWND destination = localData->CallWndProcRetHook.Destination;

        auto messageParameters = PointTo<CWPRETSTRUCT>(lParam);

        if (destination != nullptr)
            SendMessage(destination, messageParameters->message, messageParameters->wParam, messageParameters->lParam);
    }

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

LRESULT CALLBACK GetMsgProc(int code, WPARAM wParam, LPARAM lParam)
{
    int threadId = static_cast<int>(GetCurrentThreadId());    

    if (ThreadData* localData = GetLocalData(threadId, false); code == HC_ACTION)
    {   
        if (HWND destination = localData->GetMessageHook.Destination; destination != nullptr)
        {
            // Unlike some of these other hooks, we are able to modify messages of this hook type
            // before control is returned to the system.
            auto messageParameters = PointTo<MSG>(lParam);

            WaitForSingleObject(SharedSectionMutex, INFINITE);

            __try
            {
                ChangeMessage = false;

                SendMessage(
                    destination, 
                    messageParameters->message, 
                    messageParameters->wParam, 
                    messageParameters->lParam);

                if (ChangeMessage)
                {
                    messageParameters->message = ChangedMessage;
                    messageParameters->wParam = ChangedWParam;
                    messageParameters->lParam = ChangedLParam;
                }
            }
            __finally
            {
                ReleaseMutex(SharedSectionMutex);
            }
        }        
    }

    return CallNextHookEx(nullptr, code, wParam, lParam);
}