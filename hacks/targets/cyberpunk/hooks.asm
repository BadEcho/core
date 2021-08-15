//----------------------------------------------------------------------
// Hooks for Omnified Cyberpunk 2077
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC
// 
// Bad Echo Technologies are licensed under a
// Creative Commons Attribution-NonCommercial 4.0 International License.
//
// See accompanying file LICENSE.md or a copy at:
// http://creativecommons.org/licenses/by-nc/4.0/
//----------------------------------------------------------------------

// Gets the player's root and statistical structures.
// Magic numbers: 0x36A is Stamina's house address on Happy Stats Street.
define(omniPlayerHook,"Cyberpunk2077.exe"+1A03A28)

assert(omniPlayerHook,4C 8B 0E 48 C7 C5 FF FF FF FF)
alloc(getPlayer,$1000,omniPlayerHook)
alloc(player,8)
alloc(playerMaxStamina,8)
alloc(playerStaminaValue,8)

registersymbol(omniPlayerHook)
registersymbol(player)
registersymbol(playerMaxStamina)
registersymbol(playerStaminaValue)

getPlayer:
    pushf 
    // Isolate the player root structure through pinpointed comparing of comparable comaprables.
    cmp r13,0
    jne getPlayerOriginalCode
    // This is a constant identifying another statistic that is constantly queried for.
    // We're going to piggyback off of it in order to find the statistics we care about.
    cmp r14,0x1c9
    jne getPlayerOriginalCode
    push rax 
    push rbx
    push rcx
    push rdx
    push rdi
    mov rax,player
    mov [rax],rsi    
    // Load the "address book" for statistics.
    mov rax,[rsi]
    // Get the number of address book entries.
    mov ebx,[rsi+C]
    mov rcx,rbx    
    // This is the constant used to identify the stamina statistic.
    mov rdx,0x36A
searchStatistics:
    // Divide and conquer! Cleaveth the search areath in halfeth!
    mov rbx,rcx
    sar rbx,1
    // Check if the current element is what we're looking for.
    cmp [rax+rbx*4],edx
    // Save our position in case we need to search the upper half.
    lea rdi,[rax+rbx*4]
    jae moveToLowerHalf
    // The current element was below what we're looking for, so we move to the upper half.
    sub rcx,rbx
    sub rcx,0x1
    lea rax,[rdi+0x4]
    jmp isStatisticFound
moveToLowerHalf:
    mov rcx,rbx
isStatisticFound:
    test rcx,rcx
    // Continue the search unless we've found our match or have already looked everywhere
    // (leaving us with, yup, no match).
    jg searchStatistics
    // From my observations, the stamina statistic will always be found, additional logic
    // that will handle failed searches will be added if needed.
    mov rcx,[rsi]
    // rax holds the addressKey, we then have the base address of the address book subtracted from it.
    sub rax,rcx
    // Taking one half of this value gives us the index to the desired data in the statistics array.
    sar rax,2
    add rax,rax
    // Here's our statistics array.
    mov rcx,[rsi+0x10]    
    // And here's the address to our desired statistic. Hooray.
    lea rbx,[rcx+rax*8+0xC]
    // Let's create a pointer directly to the max stamina statistic.
    mov rax,playerMaxStamina
    // Is this a freshly allocated maximum stamina stat? If so, we'll load it and also set our current 
    // stamina value to reflect the maximum, as there is a very good chance that the active stamina structure 
    // has not been loaded yet, which leaves us with an ugly 0 current stamina value.
    cmp [rax],rbx
    je getPlayerCleanup
loadNewStamina:
    mov [rax],rbx
    mov rax,playerStaminaValue
    // Thankfully, no need to convert percentages here...whew...
    mov rcx,[rbx]
    mov [rax],rcx
getPlayerCleanup:
    pop rdi
    pop rdx
    pop rcx
    pop rbx
    pop rax    
getPlayerOriginalCode:
    popf
    mov r9,[rsi]
    mov rbp,FFFFFFFFFFFFFFFF
    jmp getPlayerReturn

omniPlayerHook:
    jmp getPlayer
    nop 5
getPlayerReturn:


// Gets the player's health structure.
// UNIQUE AOB: F3 0F 10 80 90 01 00 00 0F 54
define(omniPlayerHealthHook,"Cyberpunk2077.exe"+1B5F32B)

assert(omniPlayerHealthHook,F3 0F 10 80 90 01 00 00)
alloc(getPlayerHealth,$1000,omniPlayerHealthHook)
alloc(playerHealth,8)
alloc(playerHealthValue,8)

registersymbol(omniPlayerHealthHook)
registersymbol(playerHealth)
registersymbol(playerHealthValue)

getPlayerHealth:
    pushf
    // Additional health filters to support Dum Dum lol.
    // Maybe not specific to Dum Dum, but his ass is when it
    // all started.
    push rax
    mov rax,r14
    cmp eax,-1
    pop rax    
    je getPlayerHealthOriginalCode
    push rax
    mov rax,r15
    cmp eax,-1
    pop rax
    je getPlayerHealthOriginalCode
    push rbx
    mov rbx,[rax+F0]
    cmp rbx,0
    pop rbx
    je getPlayerHealthOriginalCode
    sub rsp,10
    movdqu [rsp],xmm0
    push rbx    
    mov rbx,playerHealth
    mov [rbx],rax
    // Convert the health percentage into a discrete value for display purposes.
    mov rbx,percentageDivisor    
    // We take the current health percentage and convert it into a useable percentage value (0-1).
    movss xmm0,[rax+190]
    divss xmm0,[rbx]
    // Multiplying this percentage by the maximum health gives us a discrete current health value.
    mulss xmm0,[rax+188]
    mov rbx,playerHealthValue
    movss [rbx],xmm0
    pop rbx
    movdqu xmm0,[rsp]
    add rsp,10
getPlayerHealthOriginalCode:
    popf
    movss xmm0,[rax+00000190]
    jmp getPlayerHealthReturn

omniPlayerHealthHook:
    jmp getPlayerHealth
    nop 3
getPlayerHealthReturn:


// Gets the player's stamina structure.
// UNIQUE AOB: F3 0F 10 80 90 01 00 00 F3 0F 11 45 E0
define(omniPlayerStaminaHook,"Cyberpunk2077.exe"+1B5BFF5)

assert(omniPlayerStaminaHook,F3 0F 10 80 90 01 00 00)
alloc(getPlayerStamina,$1000,omniPlayerStaminaHook)
alloc(playerStamina,8)

registersymbol(omniPlayerStaminaHook)
registersymbol(playerStamina)

getPlayerStamina:
    pushf    
    cmp r8,0    
    je getPlayerStaminaOriginalCode
    // Player data will have a non-zero value stored at this location.
    push rbx
    mov rbx,[rax+F0]
    cmp rbx,0
    pop rbx
    je getPlayerStaminaOriginalCode
    sub rsp,10
    movdqu [rsp],xmm0
    push rbx
    mov rbx,playerHealth
    cmp [rbx],rax
    je getPlayerStaminaCleanup
    mov rbx,playerStamina
    mov [rbx],rax
    // Convert the stamina percentage into a discrete value for display purposes.
    mov rbx,percentageDivisor
    // We take the current stamina percentage and convert it into a useable percentage value (0-1).
    movss xmm0,[rax+190]
    divss xmm0,[rbx]
    // Multiplying this percentage by the maximum stamina gives us a discrete current stamina value.
    mulss xmm0,[rax+188]
    mov rbx,playerStaminaValue
    movss [rbx],xmm0
getPlayerStaminaCleanup:
    pop rbx
    movdqu xmm0,[rsp]
    add rsp,10
getPlayerStaminaOriginalCode:
    popf
    movss xmm0,[rax+00000190]
    jmp getPlayerStaminaReturn

omniPlayerStaminaHook:
    jmp getPlayerStamina
    nop 3
getPlayerStaminaReturn:


// Get the player's location structure.
// Unique AOB: 0F 10 81 10 02 00 00 F2 0F 10 89 20 02 00 00 0F 11 02 F3
define(omniPlayerLocationHook,"PhysX3CharacterKinematic_x64.dll"+1EE0)

assert(omniPlayerLocationHook,0F 10 81 10 02 00 00)
alloc(getPlayerLocation,$1000,omniPlayerLocationHook)
alloc(playerLocation,8)

registersymbol(omniPlayerLocationHook)
registersymbol(playerLocation)

getPlayerLocation:
    push rax
    mov rax,playerLocation
    mov [rax],rcx
    pop rax
getPlayerLocationOriginalCode:
    movups xmm0,[rcx+00000210]
    jmp getPlayerLocationReturn

omniPlayerLocationHook:
    jmp getPlayerLocation
    nop 2
getPlayerLocationReturn:


// Gets a structure for the player's location that contains values normalized to the NPC coordinate plane.
// UNIQUE AOB: F3 0F 5C 83 08 01 00 00
define(omniPlayerLocationNormalizedHook,"Cyberpunk2077.exe"+4A3CED)

assert(omniPlayerLocationNormalizedHook,F3 0F 5C 83 08 01 00 00)
alloc(getPlayerLocationNormalized,$1000,omniPlayerLocationNormalizedHook)
alloc(playerLocationNormalized,8)

registersymbol(omniPlayerLocationNormalizedHook)
registersymbol(playerLocationNormalized)

getPlayerLocationNormalized:
    push rax
    mov rax,playerLocationNormalized
    mov [rax],rbx
    pop rax
getPlayerLocationNormalizedOriginalCode:
    subss xmm0,[rbx+00000108]
    jmp getPlayerLocationNormalizedReturn

omniPlayerLocationNormalizedHook:
    jmp getPlayerLocationNormalized
    nop 3
getPlayerLocationNormalizedReturn:


// Gets the player's magazine prior to a gun firing.
// UNIQUE AOB: 0F B7 8E 40 03 00 00
define(omniPlayerMagazineBeforeFireHook,"Cyberpunk2077.exe"+1AF82D3)

assert(omniPlayerMagazineBeforeFireHook,0F B7 8E 40 03 00 00)
alloc(getPlayerMagazine,$1000,omniPlayerMagazineBeforeFireHook)
alloc(playerMagazine,8)

registersymbol(omniPlayerMagazineBeforeFireHook)
registersymbol(playerMagazine)

getPlayerMagazine:
    pushf
    cmp r12,0
    jne getPlayerMagazineOriginalCode
    cmp r13,0
    jne getPlayerMagazineOriginalCode
    push rax
    mov rax,playerMagazine
    mov [rax],rsi
    pop rax
getPlayerMagazineOriginalCode:
    popf
    movzx ecx,word ptr [rsi+00000340]
    jmp getPlayerMagazineReturn

omniPlayerMagazineBeforeFireHook:
    jmp getPlayerMagazine
    nop 2
getPlayerMagazineReturn:


// Gets the player's magazine when swapping to a new weapon.
// UNIQUE AOB: 41 89 3F 48 8B BE C0 02 00 00
define(omniPlayerMagazineAfterSwapHook,"Cyberpunk2077.exe"+1AFADD8)

assert(omniPlayerMagazineAfterSwapHook,41 89 3F 48 8B BE C0 02 00 00)
alloc(getPlayerMagazineAfterSwap,$1000,omniPlayerMagazineAfterSwapHook)

registersymbol(omniPlayerMagazineAfterSwapHook)

getPlayerMagazineAfterSwap:
    pushf
    cmp r13d,-0x1
    je getPlayerMagazineAfterSwapOriginalCode
    push rax
    push rbx
    mov rax,playerMagazine
    mov rbx,r15
    sub rbx,0x340
    mov [rax],rbx
    pop rbx
    pop rax    
getPlayerMagazineAfterSwapOriginalCode:
    popf
    mov [r15],edi
    mov rdi,[rsi+000002C0]
    jmp getPlayerMagazineAfterSwapReturn

omniPlayerMagazineAfterSwapHook:
    jmp getPlayerMagazineAfterSwap
    nop 5
getPlayerMagazineAfterSwapReturn:


// Hooks into the vitals update code to perform tasks such as player melee hit detection and stamina
// discrete value updates.
// UNIQUE AOB: F3 0F 11 82 90 01 00 00
define(omnifyPlayerVitalsUpdateHook,"Cyberpunk2077.exe"+19CCF3A)

assert(omnifyPlayerVitalsUpdateHook,F3 0F 11 82 90 01 00 00)
alloc(playerVitalsUpdate,$1000,omnifyPlayerVitalsUpdateHook)

registersymbol(omnifyPlayerVitalsUpdateHook)

playerVitalsUpdate:
    pushf
    // Make sure stamina has been initialized.
    push rax
    mov rax,playerStamina
    cmp [rax],rdx
    pop rax
    jne playerVitalsUpdateOriginalCode    
    sub rsp,10
    movdqu [rsp],xmm1
    push rax
processStamina:    
    // Convert the stamina percentage into a discrete value for display purposes.
    mov rax,percentageDivisor
    // We take the current stamina percentage and convert it into a useable percentage value (0-1).
    movss xmm1,xmm0
    divss xmm1,[rax]
    // Multiplying this percentage by the maximum stamina gives us a discrete current stamina value.
    mulss xmm1,[rdx+188]
    mov rax,playerStaminaValue
    movss [rax],xmm1       
playerVitalsUpdateCleanup:
    pop rax
    movdqu xmm1,[rsp]
    add rsp,10
playerVitalsUpdateOriginalCode:
    popf
    movss [rdx+00000190],xmm0
    jmp playerVitalsUpdateReturn

omnifyPlayerVitalsUpdateHook:
    jmp playerVitalsUpdate
    nop 3
playerVitalsUpdateReturn:


// Hooks into the player's location update function, allowing us to set our speed as well as
// preventing the player physics system from interfering with teleportitis effects.
define(omnifyPlayerLocationUpdateHook,"PhysX3CharacterKinematic_x64.dll"+7B99)

assert(omnifyPlayerLocationUpdateHook,0F 11 86 08 02 00 00)
alloc(playerLocationUpdate,$1000,omnifyPlayerLocationUpdateHook)
alloc(movementFramesToSkip,8)

registersymbol(omnifyPlayerLocationUpdateHook)

playerLocationUpdate:
    pushf
    push rax
    mov rax,movementFramesToSkip
    cmp [rax],0
    pop rax    
    jg skipMovementFrame
    jmp checkForTeleported
skipMovementFrame:
    push rax
    mov rax,movementFramesToSkip
    dec [rax]
    pop rax
    movups xmm0,[rsi+208]
    jmp playerLocationUpdateOriginalCode
checkForTeleported:
    push rax
    mov rax,teleported
    cmp [rax],1
    pop rax    
    jne playerLocationUpdateOriginalCode
    push rax
    mov rax,teleported
    mov [rax],0    
    mov rax,movementFramesToSkip
    mov [rax],2
    pop rax
    jmp skipMovementFrame
playerLocationUpdateOriginalCode:
    popf
    movups [rsi+00000208],xmm0
    jmp playerLocationUpdateReturn

omnifyPlayerLocationUpdateHook:
    jmp playerLocationUpdate
    nop 2
playerLocationUpdateReturn:


// Gets the player's identifying attack structure.
define(omniPlayerAttackHook,"Cyberpunk2077.exe"+2D154B0)

assert(omniPlayerAttackHook,48 8B 01 FF 90 20 01 00 00)
alloc(getPlayerAttack,$1000,omniPlayerAttackHook)
alloc(playerAttack,8)

registersymbol(omniPlayerAttackHook)
registersymbol(playerAttack)

getPlayerAttack:
    push rax
    mov rax,playerAttack
    mov [rax],rcx
    pop rax
getPlayerAttackOriginalCode:
  mov rax,[rcx]
  call qword ptr [rax+00000120]
    jmp getPlayerAttackReturn

omniPlayerAttackHook:
    jmp getPlayerAttack
    nop 4
getPlayerAttackReturn:


// Initiates the Apocalypse system.
// xmm1: Damage percentage.
// [rcx+190]: Working health percentage.
// UNIQUE AOB: F3 0F 58 89 90 01 00 00 45
define(omnifyApocalypseHook,"Cyberpunk2077.exe"+19C9590)

assert(omnifyApocalypseHook,F3 0F 58 89 90 01 00 00)
alloc(initiateApocalypse,$1000,omnifyApocalypseHook)

registersymbol(omnifyApocalypseHook)

initiateApocalypse:
    pushf
    // Ensure the required player data structures are initialized.
    push rax    
    mov rax,playerHealth
    cmp [rax],0
    pop rax
    je initiateApocalypseOriginalCode
    push rax
    mov rax,playerLocation
    cmp [rax],0
    pop rax
    je initiateApocalypseOriginalCode    
    // Changes to stamina are also tallied here. We only care about health.
    push rax
    mov rax,playerStamina
    cmp [rax],rcx
    pop rax
    je initiateApocalypseOriginalCode
    sub rsp,10
    movdqu [rsp],xmm0
    sub rsp,10
    movdqu [rsp],xmm2
    push rax
    push rbx
    // Convert the damage percentage into a discrete value.    
    mov rax,percentageDivisor
    movss xmm2,xmm1
    divss xmm2,[rax]
    mulss xmm2,[rcx+188]
    mov rax,negativeOne
    mulss xmm2,[rax]       
    // Convert the working health percentage into a discrete value.
    movss xmm0,[rcx+190]    
    mov rax, percentageDivisor
    divss xmm0,[rax]
    // Multiply the percentage by the maximum health.
    mulss xmm0,[rcx+188]      
    // Check if the player is being damaged.
    mov rax,playerHealth
    cmp [rax],rcx
    je initiatePlayerApocalypse
    // The temporary working memory register at rdi points to an identifying
    // attack source.
    mov rax,[rdi+10]
    mov rbx,playerAttack
    cmp [rbx],rax
    jne initiateApocalypseCleanup    
    jmp initiateEnemyApocalypse
initiatePlayerApocalypse:
    // Load the damage amount parameter.
    sub rsp,8
    movd [rsp],xmm2
    // Load the working health amount parameter.
    sub rsp,8
    movd [rsp],xmm0
    // Load the maximum health amount parameter.
    push [rcx+188]
    // Align the player's location structure at the x-coordinate.
    mov rax,playerLocation
    mov rbx,[rax]
    lea rax,[rbx+210]
    push rax
    call executePlayerApocalypse    
    jmp initiateApocalypseUpdateDamage
initiateEnemyApocalypse:
    // Load the damage amount parameter.    
    sub rsp,8
    movd [rsp],xmm2
    // Load the working health amount parameter.
    sub rsp,8
    movd [rsp],xmm0
    call executeEnemyApocalypse
initiateApocalypseUpdateDamage:
    movd xmm1,eax
    divss xmm1,[rcx+188]
    mov rax,percentageDivisor
    mulss xmm1,[rax]
    mov rax,negativeOne
    mulss xmm1,[rax]
    movd xmm0,ebx
    divss xmm0,[rcx+188]
    mov rax,percentageDivisor
    mulss xmm0,[rax]
    movss [rcx+190],xmm0
initiateApocalypseCleanup:
    pop rbx
    pop rax
    movdqu xmm2,[rsp]
    add rsp,10
    movdqu xmm0,[rsp]
    add rsp,10
initiateApocalypseOriginalCode:
    popf
    addss xmm1,[rcx+00000190]
    jmp initiateApocalypseReturn

omnifyApocalypseHook:
    jmp initiateApocalypse
    nop 3
initiateApocalypseReturn:


damageThreshold:
    dd (float)0.2

coordinatesAreDoubles:
    dd 1

negativeVerticalDisplacementEnabled:
    dd 0

yIsVertical:
    dd 0

teleportitisDisplacementX:
    dd (float)0.5

verticalTeleportitisDisplacementX:
    dd (float)3.25


// Initiates the Predator system for NPCs.
// UNIQUE AOB: E0 0F 58 20 0F 28 D4
// [rax]: Working desired coordinates, xmm4: Movement offsets
define(omnifyPredatorHook,"Cyberpunk2077.exe"+1BF2A98)

assert(omnifyPredatorHook,0F 58 20 0F 28 D4)
alloc(initiatePredator,$1000,omnifyPredatorHook)
alloc(disablePredator,8)
alloc(identityValue,8)

registersymbol(omnifyPredatorHook)
registersymbol(disablePredator)

initiatePredator:
    // The normalized player location coordinates are required before we can engage the
    // Predator system.
    pushf
    push rax
    mov rax,playerLocationNormalized
    cmp [rax],0
    pop rax
    je initiatePredatorOriginalCode
    cmp [disablePredator],1
    je initiatePredatorOriginalCode
    // Make sure we backup the registers Predator writes to -- we'll need an additional SSE register
    // to pass the target's coordinates as parameters to the Predator function.
    sub rsp,10
    movdqu [rsp],xmm0
    push rax
    push rbx
    push rcx
    mov rbx,playerLocationNormalized
    mov rcx,[rbx]
    // The first parameter is our player's coordinates. These normalized coordinates, from which our x-coordinate
    // can be found at 0x108, are comparable to NPC coordinates as they exist on the NPC coordinate plane, which 
    // is a completely separate coordinate plane from whether the player's source-of-truth coordinates live.
    push [rcx+108]
    push [rcx+110]
    // The next parameter is the target NPC's coordinates. Note that these are not directly related to the
    // NPC's source-of-truth coordinates, but rather another set that I termed the "desired" coordinates. These
    // are independently tracked and managed by the game, and while not actually the source-of-truth, they essentially
    // become so indirectly as the game will always push changes to these as changes to the source-of-truth coordinates.
    push [rax]
    push [rax+8]    
    // The third parameter is the NPC's dimensional scales. If this game offers True Scaling, then we'll update this
    // to provide real values; until such a discovery is made, we settle for an identity matrix of 1's.
    movss xmm0,[identityValue]
    shufps xmm0,xmm0,0
    sub rsp,10
    movdqu [rsp],xmm0
    // The final parameter is the NPC's movement offsets. The Predator system expects coordinate-related parameters to 
    // be pushed to the stack as two m64 addresses. Therefore, we place the highwords onto the SSE we backed up, and then
    // push the NPC's movement offsets, in order, as quadwords to the stack.
    movhlps xmm0,xmm4
    sub rsp,8
    movq [rsp],xmm4
    sub rsp,8
    movq [rsp],xmm0
    call executePredator
    // The updated movement offsets needed to be loaded onto the xmm4 register. We'll do this
    // by dumping the Predator return values onto the stack and then loading them onto xmm4.
    sub rsp,10
    // We dump xmm4 onto the stack first so we can preserve the fourth double word.
    movups [rsp],xmm4
    mov [rsp],eax
    mov [rsp+4],ebx
    mov [rsp+8],ecx
    movups xmm4,[rsp]
    add rsp,10    
    pop rcx
    pop rbx
    pop rax
    movdqu xmm0,[rsp]
    add rsp,10
initiatePredatorOriginalCode:
    popf
    addps xmm4,[rax]
    movaps xmm2,xmm4
    jmp initiatePredatorReturn

omnifyPredatorHook:
    jmp initiatePredator
    nop
initiatePredatorReturn:


identityValue:
    dd (float)1.0

// This places the limits of the area of sketchiness at around 20 units, which is still well within the limits 
// of how far NPCs can spot the player. Around 10 units is where it seems like a good place to enable a charge of death.
aggroDistance:
    dd (float)10.0

// I'm able to hit things with my katana at ~1.1 units away from my player. We pad that number a bit in order to give the
// enemy the opportunity to come to a stop, and we have an ideal threat distance. Testing will be needed.
threatDistance:
    dd (float)1.5

disablePredator:
    dd 0


[DISABLE]


// Cleanup of omniPlayerLocationHook
omniPlayerLocationHook:
    db 0F 10 81 10 02 00 00

unregistersymbol(omniPlayerLocationHook)
unregistersymbol(playerLocation)

dealloc(playerLocation)
dealloc(getPlayerLocation)


// Cleanup of omniPlayerLocationNormalizedHook
omniPlayerLocationNormalizedHook:
    db F3 0F 5C 83 08 01 00 00

unregistersymbol(omniPlayerLocationNormalizedHook)
unregistersymbol(playerLocationNormalized)

dealloc(playerLocationNormalized)
dealloc(getPlayerLocationNormalized)


// Cleanup of omniPlayerMagazineBeforeFireHook
omniPlayerMagazineBeforeFireHook:
    db 0F B7 8E 40 03 00 00

unregistersymbol(omniPlayerMagazineBeforeFireHook)
unregistersymbol(playerMagazine)

dealloc(getPlayerMagazine)
dealloc(playerMagazine)


// Cleanup of omniPlayerMagazineAfterSwapHook
omniPlayerMagazineAfterSwapHook:
    db 41 89 3F 48 8B BE C0 02 00 00

unregistersymbol(omniPlayerMagazineAfterSwapHook)

dealloc(getPlayerMagazineAfterSwap)


// Cleanup of omnifyPlayerVitalsUpdateHook
omnifyPlayerVitalsUpdateHook:
    db F3 0F 11 82 90 01 00 00

unregistersymbol(omnifyPlayerVitalsUpdateHook)

dealloc(playerVitalsUpdate)


// Cleanup of omnifyPlayerLocationUpdateHook
omnifyPlayerLocationUpdateHook:
    db 0F 11 86 08 02 00 00

unregistersymbol(omnifyPlayerLocationUpdateHook)

dealloc(movementFramesToSkip)
dealloc(playerLocationUpdate)


// Cleanup of omnifyApocalypseHook
omnifyApocalypseHook:
    db F3 0F 58 89 90 01 00 00

unregistersymbol(omnifyApocalypseHook)

dealloc(initiateApocalypse)


// Cleanup of omnifyPredatorHook
omnifyPredatorHook:
    db 0F 58 20 0F 28 D4

unregistersymbol(omnifyPredatorHook)
unregistersymbol(disablePredator)

dealloc(identityValue)
dealloc(disablePredator)
dealloc(initiatePredator)


// Cleanup of omniPlayerHealthHook
omniPlayerHealthHook:
    db F3 0F 10 80 90 01 00 00

unregistersymbol(omniPlayerHealthHook)
unregistersymbol(playerHealth)
unregistersymbol(playerHealthValue)

dealloc(playerHealthValue)
dealloc(playerHealth)
dealloc(getPlayerHealth)


// Cleanup of omniPlayerStaminaHook
omniPlayerStaminaHook:
    db F3 0F 10 80 90 01 00 00

unregistersymbol(omniPlayerStaminaHook)
unregistersymbol(playerStamina)

dealloc(playerStamina)
dealloc(getPlayerStamina)


// Cleanup of omniPlayerHook
omniPlayerHook:
    db 4C 8B 0E 48 C7 C5 FF FF FF FF

unregistersymbol(omniPlayerHook)
unregistersymbol(player)
unregistersymbol(playerMaxStamina)
unregistersymbol(playerStaminaValue)

dealloc(player)
dealloc(playerMaxStamina)
dealloc(playerStaminaValue)
dealloc(getPlayer)



// Cleanup of omniPlayerAttackHook
omniPlayerAttackHook:
    db 48 8B 01 FF 90 20 01 00 00

unregistersymbol(omniPlayerAttackHook)
unregistersymbol(playerAttack)

dealloc(playerAttack)
dealloc(getPlayerAttack)