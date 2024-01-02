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

#include "Hooks.h"

BOOL APIENTRY DllMain(HINSTANCE instance, DWORD reason, LPVOID reserved)
{
    BOOL init;

    switch (reason)
    {
    	case DLL_PROCESS_ATTACH:        
            Instance = instance;
            MapObject = CreateFileMapping(
                INVALID_HANDLE_VALUE,
                nullptr,
                PAGE_READWRITE,
                0,
                SharedMemorySize,
                TEXT("dllmemfilemap"));

            if (MapObject == nullptr)
                return false;

            init = GetLastError() != ERROR_ALREADY_EXISTS;

            SharedMemory = MapViewOfFile(
                MapObject,
                FILE_MAP_WRITE,
                0,
                0,
                0);

            if (SharedMemory == nullptr)
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
            CloseHandle(MapObject);
            break;
    	default:
            return false;
    }

	return TRUE;    
}

ThreadData *GetLocalData(int threadId, bool addEntry)
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
        ThreadCount++;
    }

    return &SharedData[index];
}

HOOKS_API bool AddHook(HookType hookType, int threadId, HWND destination)
{
    ThreadData* localData = GetLocalData(threadId, true);

    if (localData == nullptr)
        return false;
    
    HookData* hookData;
    int idHook;
    HOOKPROC lpfn;

    switch (hookType)
    {
    	case WindowProcPreview:
            hookData = &localData->CallWndProcHook;
            idHook = WH_CALLWNDPROC;
            lpfn = CallWndProc;
            break;

    	case WindowProcReturn:
            hookData = &localData->CallWndProcRetHook;
            idHook = WH_CALLWNDPROCRET;
            lpfn = CallWndProcRet;
            break;

    	case MessageQueueRead:
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

HOOKS_API bool RemoveHook(HookType hookType, int threadId)
{
    ThreadData* localData = GetLocalData(threadId, false);

    if (localData == nullptr)
        return false;

    HookData* hookData;

    switch (hookType)
    {
		case WindowProcPreview:
            hookData = &localData->CallWndProcHook;
            break;
		case WindowProcReturn:
            hookData = &localData->CallWndProcRetHook;
            break;
		case MessageQueueRead:
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


LRESULT CALLBACK CallWndProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    int threadId = static_cast<int>(GetCurrentThreadId());

    if (ThreadData* localData = GetLocalData(threadId, false); localData != nullptr && nCode == HC_ACTION)
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

    if (ThreadData* localData = GetLocalData(threadId, false); localData != nullptr && nCode == HC_ACTION)
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

    if (ThreadData* localData = GetLocalData(threadId, false); localData != nullptr && code == HC_ACTION)
    {   
        if (HWND destination = localData->GetMessageHook.Destination; destination != nullptr)
        {
            // Unlike some of these other hooks, we are able to modify messages of this hook type before control is returned to the system.
            auto messageParameters = PointTo<MSG>(lParam);

            ModifyMessage = false;

            SendMessage(destination, messageParameters->message, messageParameters->wParam, messageParameters->lParam);

            if (ModifyMessage)
            {
                messageParameters->message = CurrentMessage;
                messageParameters->wParam = CurrentWParam;
                messageParameters->lParam = CurrentLParam;
            }
        }        
    }

    return CallNextHookEx(nullptr, code, wParam, lParam);
}