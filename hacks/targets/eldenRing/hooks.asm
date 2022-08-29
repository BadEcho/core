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
alloc(playerGameData,8)

registersymbol(playerGameData)
registersymbol(player) 
registersymbol(playerLocation)   
registersymbol(playerVitals)
registersymbol(omniPlayerHook)

getPlayer:
    pushf
    sub rsp,10
    movdqu [rsp],xmm0
    sub rsp,10
    movdqu [rsp],xmm1
    push rax
    push rbx    
    push rcx
    mov [playerVitals],rcx    
    mov rax,[rcx+8]
    mov [player],rax
    mov rbx,[rax+190]
    mov rcx,[rbx+68]
    mov [playerLocation],rcx
    mov rbx,[rax+570]
    mov [playerGameData],rbx
    mov rbx,teleport
    cmp [rbx],1
    jne getPlayerCleanup
    mov [rbx],0
    mov rbx,teleportX
    movss xmm0,[rbx]
    movss xmm1,[rax+6B0]
    subss xmm0,xmm1
    movss xmm1,[rcx+70]
    addss xmm0,xmm1
    movss [rcx+70],xmm0
    mov rbx,teleportY
    movss xmm0,[rbx]
    movss xmm1,[rax+6B4]
    subss xmm0,xmm1
    movss xmm1,[rcx+74]
    addss xmm0,xmm1
    movss [rcx+74],xmm0
    mov rbx,teleportZ
    movss xmm0,[rbx]
    movss xmm1,[rax+6B8]
    subss xmm0,xmm1
    movss xmm1,[rcx+78]
    addss xmm0,xmm1
    movss [rcx+78],xmm0  
getPlayerCleanup:    
    pop rcx
    pop rbx
    pop rax
    movdqu xmm1,[rsp]
    add rsp,10
    movdqu xmm0,[rsp]
    add rsp,10
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
// [rsp+50] | {[rsp]+8A} Source of damage's instance struct .
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
    // Check if we're already dead. Don't want the Apocalypse log polluted
    cmp [rdi+138],0
    jle abortApocalypse
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
    jne abortApocalypse
    cmp rdx,0
    jne abortApocalypse
    mov rax,[rsp+8A]
    mov rcx,player
    cmp rax,[rcx]
    jne abortApocalypse    
    call executeEnemyApocalypse
    jmp initiateApocalypseUpdateDamage
abortApocalypse:
    // Adjust the stack to account for the two common parameters that we don't need
    // to actually push.
    add rsp,10
    jmp initiateApocalypseCleanup  
initiateApocalypseUpdateDamage:
    // Convert the outputs, and set sail my friend! 
    // We'll prime the working health first since ebx needs to hold the damage amount when we're done.
    movd xmm0,ebx
    cvtss2si ecx,xmm0
    mov [rdi+138],ecx
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


teleportitisDisplacementX:
    dd (float)4.0

[DISABLE]

// Cleanup of omniPlayerHook
omniPlayerHook:
    db 66 0F 6E 89 38 01 00 00

unregistersymbol(omniPlayerHook)
unregistersymbol(playerVitals)
unregistersymbol(playerLocation)
unregistersymbol(player)
unregistersymbol(playerGameData)

dealloc(playerGameData)
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