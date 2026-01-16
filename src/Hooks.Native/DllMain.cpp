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
#include "SharedData.h"

namespace {
    HINSTANCE Instance;
    
    LRESULT SendHookMessage(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
    {
        return SendMessage(hWnd, message + WM_USER, wParam, lParam);
    }

    BOOL PostHookMessage(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
    {
        return PostMessage(hWnd, message + WM_USER, wParam, lParam);
    }
}

BOOL APIENTRY DllMain(HINSTANCE instance, DWORD reason, LPVOID)  // NOLINT(misc-use-internal-linkage) 'static' is ignored for DllMain by compiler
{
    switch (reason)
    {
    	case DLL_PROCESS_ATTACH:        
            Instance = instance;
            if (!InitializeSharedData())
                return FALSE;            
            break;
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
            break;   	
    	case DLL_PROCESS_DETACH:
            CloseSharedData();            
            break;
    	default:
            return FALSE;
    }

	return TRUE;    
}

bool __cdecl AddHook(HookType hookType, HWND destination, int threadId)
{
    HookData* hookData = AddHookData(hookType, threadId);

    if (hookData == nullptr)
        return false;

    int idHook;
    HOOKPROC lpfn;

    switch (hookType)
    {
    	case CallWindowProcedure:
            idHook = WH_CALLWNDPROC;
            lpfn = CallWndProc;
            break;

    	case CallWindowProcedureReturn:
            idHook = WH_CALLWNDPROCRET;
            lpfn = CallWndProcRet;
            break;
    	
		case GetMessages:
			idHook = WH_GETMESSAGE;
			lpfn = GetMsgProc;
			break;

        case Keyboard:
            idHook = WH_KEYBOARD;
            lpfn = KeyboardProc;
            break;

		case LowLevelKeyboard:
            idHook = WH_KEYBOARD_LL;
            lpfn = LowLevelKeyboardProc;
            break;

		default:
            return false;
    }

    HHOOK hook = SetWindowsHookEx(idHook, lpfn, Instance, threadId);

    if (hook == nullptr)
    {        
        return false;
    }

    hookData->Handle = hook;
    hookData->Destination = destination;

    return true;
}

bool __cdecl RemoveHook(HookType hookType, int threadId)
{    
    HookData* hookData = GetHookData(hookType, threadId);

    if (hookData == nullptr || hookData->Handle == nullptr)
        return false;

    bool result = UnhookWindowsHookEx(hookData->Handle);

    if (result)
        RemoveHookData(hookType, threadId);

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

    if (HookData* hookData = GetHookData(CallWindowProcedure, threadId); nCode == HC_ACTION && hookData != nullptr)
    {   
        HWND destination = hookData->Destination;

        auto messageParameters = PointTo<CWPSTRUCT>(lParam);

        if (destination != nullptr)
            SendHookMessage(destination, messageParameters->message, messageParameters->wParam, messageParameters->lParam);
    }    

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

LRESULT CALLBACK CallWndProcRet(int nCode, WPARAM wParam, LPARAM lParam)
{
    int threadId = static_cast<int>(GetCurrentThreadId());    

    if (HookData* hookData = GetHookData(CallWindowProcedureReturn, threadId); nCode == HC_ACTION && hookData != nullptr)
    {
        HWND destination = hookData->Destination;
        
        auto messageParameters = PointTo<CWPRETSTRUCT>(lParam);
        
        if (destination != nullptr)
            SendHookMessage(destination, messageParameters->message, messageParameters->wParam, messageParameters->lParam);
    }

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

LRESULT CALLBACK GetMsgProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    int threadId = static_cast<int>(GetCurrentThreadId());    

    if (HookData* hookData = GetHookData(GetMessages, threadId); nCode == HC_ACTION && hookData != nullptr)
    {   
        if (HWND destination = hookData->Destination; destination != nullptr)
        {
            // Unlike some of these other hooks, we are able to modify messages of this hook type
            // before control is returned to the system.
            auto messageParameters = PointTo<MSG>(lParam);

            WaitForSingleObject(SharedSectionMutex, INFINITE);

            __try
            {
                ChangeMessage = false;

                SendHookMessage(
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

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    int threadId = static_cast<int>(GetCurrentThreadId());

    if (HookData* hookData = GetHookData(Keyboard, threadId); nCode == HC_ACTION && hookData != nullptr)
    {
        HWND destination = hookData->Destination;

        WORD keyFlags = HIWORD(lParam);
        bool isKeyUp = (keyFlags & KF_UP) == KF_UP;
        
        if (destination != nullptr)
            SendHookMessage(destination, isKeyUp ? WM_KEYUP : WM_KEYDOWN, wParam, lParam);
    }

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}

LRESULT CALLBACK LowLevelKeyboardProc(int nCode, WPARAM wParam, LPARAM lParam)
{
    int threadId = static_cast<int>(GetCurrentThreadId());

    if (HookData* hookData = GetHookData(LowLevelKeyboard, threadId); nCode == HC_ACTION && hookData != nullptr)
    {
        HWND destination = hookData->Destination;

        auto keyboardInput = PointTo<KBDLLHOOKSTRUCT>(lParam);
        auto message = static_cast<unsigned int>(wParam);

        // Low-level keyboard hooks have very stringent execution requirements. To alleviate this burden on
        // our code, we asynchronously post the hook event to our listener.
        if (destination != nullptr)
            PostHookMessage(destination, message, keyboardInput->vkCode, keyboardInput->flags);
    }

    return CallNextHookEx(nullptr, nCode, wParam, lParam);
}