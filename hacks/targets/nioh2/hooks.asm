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
// [rcx+20]: Current health.
// [rcx+18]: Maximum health.
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


// Gets the player's stamina (Ki) structure.
// Polls player stamina exclusively. No filtering required.
// Unlike health, stamina is stored as a float.
// [rcx+8]: Current stamina.
// [rcx+C]: Maximum stamina.
// UNIQUE AOB: F3 0F 58 41 08 C3 CC CC CC CC CC 48
define(omniPlayerStaminaHook,"nioh2.exe"+9F0125)

assert(omniPlayerStaminaHook,F3 0F 58 41 08)
alloc(getPlayerStamina,$1000,omniPlayerStaminaHook)
alloc(playerStamina,8)

registersymbol(playerStamina)
registersymbol(omniPlayerStaminaHook)

getPlayerStamina:
    mov [playerStamina],rcx
getPlayerStaminaOriginalCode:
    addss xmm0,[rcx+08]
    jmp getPlayerStaminaReturn

omniPlayerStaminaHook:
    jmp getPlayerStamina
getPlayerStaminaReturn:


// Initiates the Apocalypse system.
// This is Nioh 2's damage application code.
// [rbx+10]: Working health.
// edi: Damage amount.
// rbx: Target health structure.
// UNIQUE AOB: 8B 43 10 2B C7
define(omnifyApocalypseHook,"nioh2.exe"+9C4E10)

assert(omnifyApocalypseHook,8B 43 10 2B C7)
alloc(initiateApocalypse,$1000,omnifyApocalypseHook)

registersymbol(omnifyApocalypseHook)

initiateApocalypse:
    pushf

initiateApocalypseOriginalCode:
    popf
    mov eax,[rbx+10]
    sub eax,edi
    jmp initiateApocalypseReturn

omnifyApocalypseHook:
    jmp initiateApocalypse
initiateApocalypseReturn:


[DISABLE]

// Cleanup of omniPlayerHealthHook
omniPlayerHealthHook:
    db 48 6B 41 20 64

unregistersymbol(omniPlayerHealthHook)
unregistersymbol(playerHealth)

dealloc(playerHealth)
dealloc(getPlayerHealth)


// Cleanup of omniPlayerStaminaHook
omniPlayerStaminaHook:
    db F3 0F 58 41 08

unregistersymbol(omniPlayerStaminaHook)
unregistersymbol(playerStamina)

dealloc(playerStamina)
dealloc(getPlayerStamina)


// Cleanup of omnifyApocalypseHook
omnifyApocalypseHook:
    db 8B 43 10 2B C7

unregistersymbol(omnifyApocalypseHook)

dealloc(initiateApocalypse)