//----------------------------------------------------------------------
// Hooks for Omnified Nioh 2
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC
//
// Bad Echo Technologies are licensed under a
// Creative Commons Attribution-NonCommercial 4.0 International License.
//
// See accompanying file LICENSE.md or a copy at:
// http://creativecommons.org/licenses/by-nc/4.0/
//----------------------------------------------------------------------

// Gets the player's health structure.
// Polls player health exclusively. No filtering required.
// [rcx+20]: Current HP.
// [rcx+18]: Maximum HP.
// UNIQUE AOB: 3B 48 6B 41 20 64
define(omniPlayerHealthHook,"nioh2.exe"+C700E0)

assert(omniPlayerHealthHook,48 6B 41 20 64)
alloc(getPlayerHealth,$1000,omniPlayerHealthHook)
alloc(playerHealth,8)

registersymbol(playerHealth)
registersymbol(omniPlayerHealthHook)

getPlayerHealth:
    mov [playerHealth],rcx
getPlayerHealthOriginalCode:
    imul rax,[rcx+20],64
    jmp getPlayerHealthReturn

omniPlayerHealthHook:
    jmp getPlayerHealth
getPlayerHealthReturn:


[DISABLE]

// Cleanup of omniPlayerHealthHook
omniPlayerHealthHook:
    db 48 6B 41 20 64

unregistersymbol(omniPlayerHealthHook)
unregistersymbol(playerHealth)

dealloc(playerHealth)
dealloc(getPlayerHealth)