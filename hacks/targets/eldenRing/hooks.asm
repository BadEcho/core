//----------------------------------------------------------------------
// Hooks for Omnified Elden Ring
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2022 Bad Echo LLC
//
// Bad Echo Technologies are licensed under a
// Creative Commons Attribution-NonCommercial 4.0 International License.
//
// See accompanying file LICENSE.md or a copy at:
// http://creativecommons.org/licenses/by-nc/4.0/
//----------------------------------------------------------------------

// Gets the player's health.
define(omniPlayerHook,"start_protected_game.exe"+598262)

assert(omniPlayerHook,66 0F 6E 89 38 01 00 00)
alloc(getPlayer,$1000,omniPlayerHook)
alloc(player,8)
alloc(playerLocation,8)
alloc(playerVitals,8)

registersymbol(player) 
registersymbol(playerLocation)   
registersymbol(playerVitals)
registersymbol(omniPlayerHook)

getPlayer:
    pushf
    push rax
    push rbx    
    mov [playerVitals],rcx    
    mov rax,[rcx+8]
    mov [player],rax
    mov rbx,[rax+190]
    mov rax,[rbx+68]
    mov [playerLocation],rax
    pop rbx
    pop rax
getPlayerOriginalCode:
    popf
    movd xmm1,[rcx+00000138]
    jmp getPlayerReturn

omniPlayerHook:
    jmp getPlayer
    nop 3
getPlayerReturn:


// Increments the death counter when we ded.
// rcx: The target entity's vital structure.
// r8: 0 if dead, 1 if not.
define(omniDeathCounterHook,"start_protected_game.exe"+432702)

assert(omniDeathCounterHook,89 81 38 01 00 00)
alloc(incrementDeathCounter,$1000,omniDeathCounterHook)
alloc(deathCounter,8)

registersymbol(deathCounter)
registersymbol(omniDeathCounterHook)

incrementDeathCounter:
    pushf
    push rax
    mov rax,playerVitals
    cmp [rax],rcx
    pop rax
    jne incrementDeathCounterOriginalCode
    cmp r8,0    
    jne incrementDeathCounterOriginalCode
    cmp rax,0
    jne incrementDeathCounterOriginalCode
    inc [deathCounter]    
incrementDeathCounterOriginalCode:
    popf
    mov [rcx+00000138],eax
    jmp incrementDeathCounterReturn

omniDeathCounterHook:
    jmp incrementDeathCounter
    nop 
incrementDeathCounterReturn:


deathCounter:
    dd 0


// Initiates the Apocalypse system.
// rdi: Target entity's vitals structure.
// ebx: Two's complemented damage value being added to the health.
// Player is damage source when rax, rdx == 0
// (rsi and rcx and r12 also observed to be zeroed).
define(omnifyApocalypseHook,"start_protected_game.exe"+431D8F)

assert(omnifyApocalypseHook,03 9F 38 01 00 00)
alloc(initiateApocalypse,$1000,omnifyApocalypseHook)

registersymbol(omnifyApocalypseHook)

initiateApocalypse:
    pushf
    // Ensure the required player data structres are initialized.
    push rax
    mov rax,player
    cmp [rax],0
    pop rax
    je initiateApocalypseOriginalCode
    // Need one SSE register to hold converted values
    sub rsp,10
    movdqu [rsp],xmm0
    // ...and a few registers for holding stuff.
    push rax     
    push rcx       
    push rsi
    // Back up rax so we can use it later to check if the player is the damage source.
    mov rcx,rax
    // Back up rbx so we can manipulate the value safely whether the Apocalypse system runs or not.
    mov rsi,rbx
    // Make the negative damage positive and load it.
    neg esi
    cvtsi2ss xmm0,esi
    sub rsp,8
    movd [rsp],xmm0
    // Then load the target's current health.
    mov rax,[rdi+138]
    cvtsi2ss xmm0,eax
    sub rsp,8
    movd [rsp],xmm0
    // Let's now see if the affected entity is the player or an NPC.
    mov rax,playerVitals
    cmp [rax],rdi    
    jne initiateEnemyApocalypse    
initiatePlayerApocalypse:
    // Load the player's maximum health as the next parameter.
    mov rax,[rdi+13C]
    cvtsi2ss xmm0,eax
    sub rsp,8
    movd [rsp],xmm0
    // Finally, load a pointer to where our x-coordinate resides for the player.
    mov rax,playerLocation
    mov rsi,[rax]
    lea rax,[rsi+70]
    push rax
    call executePlayerApocalypse
    jmp initiateApocalypseUpdateDamage
initiateEnemyApocalypse:
    // We only care about damage done to enemies that the player is responsible for.
    cmp rcx,0
    jne abortEnemyApocalypse
    cmp rdx,0
    jne abortEnemyApocalypse
    call executeEnemyApocalypse
abortEnemyApocalypse:
    // Adjust the stack to account for the two common parameters that we don't need
    // to actually push.
    add rsp,10
    jmp initiateApocalypseCleanup  
initiateApocalypseUpdateDamage:
    // Convert the outputs, and set sail my friend! 
    // We'll prime the working health first since ebx needs to hold the damage amount when we're done.
    movd xmm0,ebx
    cvtss2si eax,xmm0
    mov [rdi+138],eax
    movd xmm0,eax
    cvtss2si ebx,xmm0    
    neg ebx
initiateApocalypseCleanup:    
    pop rsi
    pop rcx
    pop rax
    movdqu xmm0,[rsp]
    add rsp,10
initiateApocalypseOriginalCode:
    popf
    add ebx,[rdi+00000138]
    jmp initiateApocalypseReturn

omnifyApocalypseHook:
    jmp initiateApocalypse
    nop 
initiateApocalypseReturn:


[DISABLE]

// Cleanup of omniPlayerHook
omniPlayerHook:
    db 66 0F 6E 89 38 01 00 00

unregistersymbol(omniPlayerHook)
unregistersymbol(playerVitals)
unregistersymbol(playerLocation)
unregistersymbol(player)

dealloc(player)
dealloc(playerLocation)
dealloc(playerVitals)
dealloc(getPlayer)


// Cleanup of omniDeathCounterHook
omniDeathCounterHook:
    db 89 81 38 01 00 00

unregistersymbol(omniDeathCounterHook)
unregistersymbol(deathCounter)

dealloc(deathCounter)
dealloc(incrementDeathCounter)


// Cleanup of omnifyApocalypseHook
omnifyApocalypseHook:
    db 03 9F 38 01 00 00

unregistersymbol(omnifyApocalypseHook)

dealloc(initiateApocalypse)