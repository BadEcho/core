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

// Gets the player's information.
// UNIQUE AOB: 24 48 8B 08 66 0F 6E 89 38 01 00 00
define(omniPlayerHook,"start_protected_game.exe"+5990E2)

assert(omniPlayerHook,66 0F 6E 89 38 01 00 00)
alloc(getPlayer,$1000,omniPlayerHook)
alloc(player,8)
alloc(playerLocation,8)
alloc(playerVitals,8)
alloc(playerGameData,8)
alloc(playerHavokProxy,8)

registersymbol(playerHavokProxy)
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
    // We start off working in the player's "vitals" structure, also known as the "player data module".
    mov [playerVitals],rcx    
    // The root player instance structure is found at 0x8, from there we will be mapping everything else.
    mov rax,[rcx+8]
    mov [player],rax
    // The collection of all modules for the player is found at 0x190 in the player root structure.
    mov rbx,[rax+190]
    // The physics module, containing the character's location, is at 0x68 in the module collection.
    mov rcx,[rbx+68]
    mov [playerLocation],rcx
    // In order to find the player's Havok character proxy, we must first find the character proxy in the
    // physics module.
    mov rbx,[rcx+98]
    // The player's Havok character proxy can be found at 0x88 inside the character proxy.
    mov rcx,[rbx+88]
    mov [playerHavokProxy],rcx
    mov rbx,[rax+570]
    mov [playerGameData],rbx
    mov rbx,teleport
    cmp [rbx],1
    jne getPlayerCleanup
    mov [rbx],0
    mov rcx,[playerLocation]
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

// Gets the horsey's information.
// UNIQUE AOB: 8B 8A 38 01 00 00 89 4B
define(omniHorseyHook,"start_protected_game.exe"+63F40E)

assert(omniHorseyHook,8B 8A 38 01 00 00)
alloc(getHorsey,$1000,omniHorseyHook)
alloc(horsey,8)
alloc(horseyVitals,8)
alloc(horseyHavokProxy,8)

registersymbol(horseyHavokProxy)
registersymbol(horseyVitals)
registersymbol(horsey)
registersymbol(omniHorseyHook)

getHorsey:
    pushf
    push rax
    push rbx
    push rcx
    // Horsey information retrieval is fairly identical to player information retrieval.
    mov [horseyVitals],rdx
    mov rax,[rdx+8]
    mov [horsey],rax
    // We take the same path to the horsey's havok proxy as we do with the player.
    mov rbx,[rax+190]
    mov rcx,[rbx+68]
    mov rbx,[rcx+98]
    mov rcx,[rbx+88]
    mov [horseyHavokProxy],rcx
    pop rcx
    pop rbx
    pop rax
getHorseyOriginalCode:
    popf
    mov ecx,[rdx+00000138]
    jmp getHorseyReturn

omniHorseyHook:
    jmp getHorsey
    nop 
getHorseyReturn:


// Increments the death counter when we ded.
// rcx: The target entity's vital structure.
// r8 is 0 and r11 is 1 when we're dead.
// UNIQUE AOB: 89 81 38 01 00 00 E8
define(omniDeathCounterHook,"start_protected_game.exe"+432782)

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
    cmp r11,1
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
// [rsp+50] | {[rsp]+8A} Source of damage's instance struct.
// UNIQUE AOB: 03 9F 38 01 00 00 85
define(omnifyApocalypseHook,"start_protected_game.exe"+431E0F)

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
    // Check if we're already dead. Don't want the Apocalypse log polluted.
    cmp [rdi+138],0
    jle abortApocalypse
    // We want to only execute the Apocalypse if the source of damage is an enemy.
    // Let's filter out environmental effects and fall damage.
    // First, see if a valid damage source pointer exists.
    mov rsi,[rsp+8A]
    cmp rsi,0
    jle abortApocalypse
    mov rax,[rsi]
    // Lower 2 bytes of poison damage source is 0x7F20.
    cmp ax,0x7F20
    je abortApocalypse
    // Fall damage is a "special effect", which has lower 2 bytes of 0xAAF8.    
    cmp ax,0xAAF8 
    je abortApocalypse
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


// Initiates the Predator system.
// rdi: Target's Havok chracter proxy.
// xmm6: Movement offsets being applied to current coordinates to form desired location coordinates.
// xmm0: Current working location coordinates.
// Player's Havok character proxy can be found by looking at offset 0x88 of the chracter's proxy which itself 
// is found at [playerLocation+0x98]
// UNIQUE AOB: F3 45 0F 5C D3 0F
define(omnifyPredatorHook,"start_protected_game.exe"+180F4A0)

assert(omnifyPredatorHook,F3 45 0F 5C D3)
alloc(initiatePredator,$1000,omnifyPredatorHook)
alloc(playerSpeedX,8)
alloc(playerVerticalX,8)
alloc(identityValue,8)

registersymbol(identityValue)
registersymbol(playerVerticalX)
registersymbol(playerSpeedX)
registersymbol(omnifyPredatorHook)

initiatePredator:
    pushf
    // This runs more frequently than our player struct polling function.
    push rax    
    mov rax,playerHavokProxy
    cmp rax,0
    pop rax
    je initiatePredatorOriginalCode
    sub rsp,10
    movdqu [rsp],xmm1
    sub rsp,10
    movdqu [rsp],xmm2
    push rax
    push rbx
    push rcx   
    mov rax,playerHavokProxy
    cmp [rax],rdi
    je applyPlayerSpeed
    mov rax,horseyHavokProxy
    cmp [rax],rdi
    je applyPlayerSpeed
initiatePredatorExecute:
    mov rax,playerLocation
    mov rbx,[rax]
    // Push the player's current coordinates.
    push [rbx+70]
    push [rbx+78]
    // Push the enemy's current coordinates.
    movhlps xmm1,xmm0
    sub rsp,8
    movq [rsp],xmm0
    sub rsp,8
    movq [rsp],xmm1
    // Push an identity matrix for the scaling parameters.
    movss xmm1,[identityValue]
    shufps xmm1,xmm1,0
    sub rsp,10
    movdqu [rsp],xmm1
    // Push the enemy's movement offsets.
    movhlps xmm1,xmm6
    sub rsp,8
    movq [rsp],xmm6
    sub rsp,8
    movq [rsp],xmm1
    call executePredator
initiatePredatorUpdateOffsets:
    sub rsp,10
    mov [rsp],eax
    mov [rsp+4],ebx
    mov [rsp+8],ecx
    movups xmm6,[rsp]
    add rsp,10
    jmp initiatePredatorCleanup
applyPlayerSpeed:    
    movss xmm1,[playerSpeedX]    
    movss xmm2,[playerVerticalX]    
    movlhps xmm1,xmm2    
    shufps xmm1,xmm1,8
    mulps xmm6,xmm1
initiatePredatorCleanup:
    pop rcx
    pop rbx
    pop rax
    movdqu xmm2,[rsp]
    add rsp,10
    movdqu xmm1,[rsp]
    add rsp,10
initiatePredatorOriginalCode:
    popf
    subss xmm10,xmm11
    // Followed by the movement application code:
    // addps xmm0,xmm6    
    jmp initiatePredatorReturn

omnifyPredatorHook:
    jmp initiatePredator
initiatePredatorReturn:


playerSpeedX:
    dd (float)1.0

playerVerticalX:
    dd (float)1.0

identityValue:
    dd (float)1.0

aggroDistance:
    dd (float)12.5

threatDistance:
    dd (float)3.5


// Initiates the Abomnification system.
// [rcx+8]: Player/EnemyIns, used as the identifying address.
// UNIQUE AOB: 44 8B 81 38 01 00 00 C7
define(omnifyAbomnificationHook,"start_protected_game.exe"+433B8C)

assert(omnifyAbomnificationHook,44 8B 81 38 01 00 00)
alloc(initiateAbomnification,$1000,omnifyAbomnificationHook)

registersymbol(omnifyAbomnificationHook)

initiateAbomnification:
    pushf
    // Backing up registers used as outputs for the Abomnification system.
    push rax
    push rbx
    push rcx    
    push [rcx+8]
    call executeAbomnification    
    pop rcx
    pop rbx
    pop rax
initiateAbomnificationOriginalCode:
    popf
    mov r8d,[rcx+00000138]
    jmp initiateAbomnificationReturn

omnifyAbomnificationHook:
    jmp initiateAbomnification
    nop 2
initiateAbomnificationReturn:


// Applies Abomnification generated scale multipliers on humanoid entities.
// [rsp+10] | {rsp+4A}: [[CSFD4LocationBodyScaleModifier+78]-640] == PlayerIns that owns
//                      the particular model transformation matrix being worked on here.
// xmm7: Width
// xmm4: Height
// xmm3: Depth
// UNIQUE AOB: 0F 29 3C 07 0F 29 64 07 10
define(omnifyApplyHumanAbomnificationHook,"start_protected_game.exe"+AD0E3A)

assert(omnifyApplyHumanAbomnificationHook,0F 29 3C 07 0F 29 64 07 10)
alloc(applyHumanAbomnification,$1000,omnifyApplyHumanAbomnificationHook)

registersymbol(omnifyApplyHumanAbomnificationHook)

applyHumanAbomnification:
    pushf
    // Backing up registers needed to perform scale multiplication.
    sub rsp,10
    movdqu [rsp],xmm0
    sub rsp,10
    movdqu [rsp],xmm1
    // Backing up registers used as outputs for the Abomnification system.
    push rax
    push rbx
    push rcx
    // Retrieving the entity's root structure.
    mov rax,[rsp+4A]
    mov rbx,[rax+78]
    sub rbx,640
    // Push the identifying address (the PlayerIns).    
    push rbx
    call getAbomnifiedScales
    // Apply width scaling.
    movd xmm0,eax
    shufps xmm0,xmm0,0
    mulps xmm7,xmm0
    // Apply height scaling.
    movd xmm0,ebx
    shufps xmm0,xmm0,0
    mulps xmm4,xmm0
    // Apply depth scaling.
    movd xmm0,ecx
    shufps xmm0,xmm0,0
    mulps xmm3,xmm0
    pop rcx
    pop rbx
    pop rax
    movdqu xmm1,[rsp]
    add rsp,10
    movdqu xmm0,[rsp]
    add rsp,10
applyHumanAbomnificationOriginalCode:
    popf
    movaps [rdi+rax],xmm7
    movaps [rdi+rax+10],xmm4
    jmp applyHumanAbomnificationReturn

omnifyApplyHumanAbomnificationHook:
    jmp applyHumanAbomnification
    nop 4
applyHumanAbomnificationReturn:


// Applies Abomnification generated scale multipliers on non-humanoid entities.
// [rsp+E0] | {rsp+11A}: [CSFD4LocationMtxx44ChrEntity+B0] = EnemyIns that owns the particular
//                       model transformation matrix being worked on here.
// xmm0: Width
// xmm1: Height
// xmm2: Depth
// UNIQUE AOB: 0F 29 07 0F 29 4F 10 0F 29 57 20 EB
define(omnifyApplyNonhumanAbomnificationHook,"start_protected_game.exe"+ACFDB3)

assert(omnifyApplyNonhumanAbomnificationHook,0F 29 07 0F 29 4F 10)
alloc(applyNonhumanAbomnification,$1000,omnifyApplyNonhumanAbomnificationHook)

registersymbol(omnifyApplyNonhumanAbomnificationHook)

applyNonhumanAbomnification:
    pushf
    // Backing up registers needed to perform scale multiplication.
    sub rsp,10
    movdqu [rsp],xmm3
    sub rsp,10
    movdqu [rsp],xmm4
    // Backing up registers used as outputs for the Abomnification system.
    push rax
    push rbx
    push rcx
    // Retrieving the entity's root structure.
    mov rbx,[rsp+11A]
    cmp rbx,0
    je applyNonhumanAbomnificationExit    
    lea rcx,[rbx]
    call checkBadPointer
    cmp rcx,0
    jne applyNonhumanAbomnificationExit 
    mov rax,[rbx]
    // Ensure that this is a 4x4 location matrix struct.
    cmp ax,0xA4E8
    jne applyNonhumanAbomnificationExit
    mov rax,[rbx+B0]
    // Push the identifying address (the EnemyIns).
    push rax
    call getAbomnifiedScales
    // Apply width scaling.
    movd xmm3,eax
    shufps xmm3,xmm3,0
    mulps xmm0,xmm3
    // Apply height scaling.
    movd xmm3,ebx
    shufps xmm3,xmm3,0
    mulps xmm1,xmm3
    // Apply depth scaling.
    movd xmm3,ecx
    shufps xmm3,xmm3,0
    mulps xmm2,xmm3
    nop
applyNonhumanAbomnificationExit:
    pop rcx
    pop rbx
    pop rax
    movdqu xmm4,[rsp]
    add rsp,10
    movdqu xmm3,[rsp]
    add rsp,10
applyNonhumanAbomnificationOriginalCode:
    popf
    movaps [rdi],xmm0
    movaps [rdi+10],xmm1
    jmp applyNonhumanAbomnificationReturn

omnifyApplyNonhumanAbomnificationHook:
    jmp applyNonhumanAbomnification
    nop 2
applyNonhumanAbomnificationReturn:


[DISABLE]

// Cleanup of omniPlayerHook
omniPlayerHook:
    db 66 0F 6E 89 38 01 00 00

unregistersymbol(omniPlayerHook)
unregistersymbol(playerVitals)
unregistersymbol(playerLocation)
unregistersymbol(player)
unregistersymbol(playerGameData)
unregistersymbol(playerHavokProxy)

dealloc(playerHavokProxy)
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


// Cleanup of omniHorseyHook
omniHorseyHook:
    db 8B 8A 38 01 00 00

unregistersymbol(omniHorseyHook)
unregistersymbol(horseyHavokProxy)
unregistersymbol(horseyVitals)
unregistersymbol(horsey)

dealloc(horsey)
dealloc(horseyVitals)
dealloc(horseyHavokProxy)
dealloc(getHorsey)


// Cleanup of omnifyApocalypseHook
omnifyApocalypseHook:
    db 03 9F 38 01 00 00

unregistersymbol(omnifyApocalypseHook)

dealloc(initiateApocalypse)


// Cleanup of omnifyPredatorHook
omnifyPredatorHook:
    db F3 45 0F 5C D3

unregistersymbol(omnifyPredatorHook)
unregistersymbol(playerSpeedX)
unregistersymbol(playerVerticalX)
unregistersymbol(identityValue)

dealloc(identityValue)
dealloc(playerVerticalX)
dealloc(playerSpeedX)
dealloc(initiatePredator)


// Cleanup of omnifyAbomnificationHook
omnifyAbomnificationHook:
    db 44 8B 81 38 01 00 00

unregistersymbol(omnifyAbomnificationHook)

dealloc(initiateAbomnification)


// Cleanup of omnifyApplyHumanAbomnificationHook
omnifyApplyHumanAbomnificationHook:
    db 0F 29 3C 07 0F 29 64 07 10

unregistersymbol(omnifyApplyHumanAbomnificationHook)

dealloc(applyHumanAbomnification)


// Cleanup of omnifyApplyNonhumanAbomnificationHook
omnifyApplyNonhumanAbomnificationHook:
    db 0F 29 07 0F 29 4F 10

unregistersymbol(omnifyApplyNonhumanAbomnificationHook)

dealloc(applyNonhumanAbomnification)