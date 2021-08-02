// Hooks for Omnified Cyberpunk 2077
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC

// Gets the player's root structure. n
// UNIQUE AOB: 00 41 B8 0A 00 00 00 48 8B 01
define(omniPlayerHook,"Cyberpunk2077.exe"+1A5A190)

assert(omniPlayerHook,41 B8 0A 00 00 00)
alloc(getPlayer,$1000,omniPlayerHook)
alloc(player,8)

registersymbol(omniPlayerHook)
registersymbol(player)

getPlayer:
    push rax
    mov rax,player    
    pop rax
getPlayerOriginalCode:
    mov r8d,0000000A
    jmp getPlayerReturn

omniPlayerHook:
    jmp getPlayer
    nop
getPlayerReturn:


// Gets the player's health structure.
// UNIQUE AOB: F3 0F 10 80 90 01 00 00 0F 54
define(omniPlayerHealthHook,"Cyberpunk2077.exe"+1B5F32B)

assert(omniPlayerHealthHook,F3 0F 10 80 90 01 00 00)
alloc(getPlayerHealth,$1000,omniPlayerHealthHook)
alloc(playerHealth,8)

registersymbol(omniPlayerHealthHook)
registersymbol(playerHealth)

getPlayerHealth:
    push rbx
    mov rbx,playerHealth
    mov [rbx],rax
    pop rbx
getPlayerHealthOriginalCode:
    movss xmm0,[rax+00000190]
    jmp getPlayerHealthReturn

omniPlayerHealthHook:
    jmp getPlayerHealth
    nop 3
getPlayerHealthReturn:


// Gets the player's stamina structure.
// UNIQUE AOB: 48 89 44 24 70 48 89 7C 24 78 E8 B6
define(omniPlayerStaminaHook,"Cyberpunk2077.exe"+1B622E7)

assert(omniPlayerStaminaHook,F3 0F 10 BF 90 01 00 00)
alloc(getPlayerStamina,$1000,omniPlayerStaminaHook)
alloc(playerStamina,8)

registersymbol(omniPlayerStaminaHook)
registersymbol(playerStamina)

getPlayerStamina:
    pushf
    cmp r12,1
    jne getPlayerStaminaOriginalCode
    push rax
    mov rax,playerStamina
    mov [rax],rdi
    pop rax
getPlayerStaminaOriginalCode:
    popf
    movss xmm7,[rdi+00000190]
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
    pushf
    push rax
    mov rax,playerLocation
    mov [rax],rcx
    pop rax
getPlayerLocationOriginalCode:
    popf
    movups xmm0,[rcx+00000210]
    jmp getPlayerLocationReturn

omniPlayerLocationHook:
    jmp getPlayerLocation
    nop 2
getPlayerLocationReturn:


// Gets the player's magazine.
// UNIQUE AOB: 0F B7 8E 40 03 00 00
define(omniPlayerMagazineHook,"Cyberpunk2077.exe"+1AF82D3)

assert(omniPlayerMagazineHook,0F B7 8E 40 03 00 00)
alloc(getPlayerMagazine,$1000,omniPlayerMagazineHook)
alloc(playerMagazine,8)

registersymbol(omniPlayerMagazineHook)
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

omniPlayerMagazineHook:
    jmp getPlayerMagazine
    nop 2
getPlayerMagazineReturn:


// Hit detection globals.
alloc(playerAttacking,8)

registersymbol(playerAttacking)


// Detects a gunshot from the player.
// Unique AOB: 44 89 A6 40 03 00 00
define(omniDetectPlayerFire,"Cyberpunk2077.exe"+1AF82F7)

assert(omniDetectPlayerFire,44 89 A6 40 03 00 00)
alloc(detectPlayerFire,$1000,omniDetectPlayerFire)

registersymbol(omniDetectPlayerFire)

detectPlayerFire:
    pushf
    push rax
    mov rax,playerMagazine
    cmp [rax],rsi
    pop rax
    jne resetPlayerAttacking
    mov [playerAttacking],1
    jmp detectPlayerFireOriginalCode
resetPlayerAttacking:
    mov [playerAttacking],0
detectPlayerFireOriginalCode:
    popf
    mov [rsi+00000340],r12d
    jmp detectPlayerFireReturn

omniDetectPlayerFire:
    jmp detectPlayerFire
    nop 2
detectPlayerFireReturn:


// Hooks into the vitals update code to perform tasks such as player melee hit detection.
// UNIQUE AOB: F3 0F 11 82 90 01 00 00
define(omnifyPlayerVitalsUpdateHook,"Cyberpunk2077.exe"+19CCF3A)

assert(omnifyPlayerVitalsUpdateHook,F3 0F 11 82 90 01 00 00)
alloc(playerVitalsUpdate,$1000,omnifyPlayerVitalsUpdateHook)
alloc(punchInTheFaceThreshold,8)

registersymbol(omnifyPlayerVitalsUpdateHook)

playerVitalsUpdate:
    pushf
    // Make sure stamina has been initialized.
    push rax
    mov rax,playerStamina
    cmp rax,0
    pop rax
    je playerVitalsUpdateOriginalCode
    // Check if we're punching someone in the face. See if stamina is being updated.
    push rax
    mov rax,playerStamina
    cmp [rax],rdx
    pop rax
    jne playerVitalsUpdateOriginalCode
    ucomiss xmm6,[punchInTheFaceThreshold]
    ja playerVitalsUpdateOriginalCode
    mov [playerAttacking],1
playerVitalsUpdateOriginalCode:
    popf
    movss [rdx+00000190],xmm0
    jmp playerVitalsUpdateReturn

omnifyPlayerVitalsUpdateHook:
    jmp playerVitalsUpdate
    nop 3
playerVitalsUpdateReturn:


punchInTheFaceThreshold:
    dd (float)-5.0


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
    mov rax,playerAttacking
    cmp [rax],1
    jne initiateApocalypseCleanup
    mov [rax],0
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


coordinatesAreDoubles:
    dd 1

negativeVerticalDisplacementEnabled:
    dd 0

yIsVertical:
    dd 0

teleportitisDisplacementX:
    dd (float)0.5

verticalTeleportitisDisplacementX:
    dd (float)5.0


[DISABLE]


// Cleanup of omniPlayerLocationHook
omniPlayerLocationHook:
    db 0F 10 81 10 02 00 00

unregistersymbol(omniPlayerLocationHook)
unregistersymbol(playerLocation)

dealloc(playerLocation)
dealloc(getPlayerLocation)


// Cleanup of omniPlayerHook
omniPlayerHook:
    db 41 B8 0A 00 00 00

unregistersymbol(omniPlayerHook)
unregistersymbol(player)

dealloc(player)
dealloc(getPlayer)


// Cleanup of omniPlayerMagazineHook
omniPlayerMagazineHook:
    db 0F B7 8E 40 03 00 00

unregistersymbol(omniPlayerMagazineHook)
unregistersymbol(playerMagazine)

dealloc(getPlayerMagazine)
dealloc(playerMagazine)


// Cleanup of omniDetectPlayerFire
omniDetectPlayerFire:
    db 44 89 A6 40 03 00 00

unregistersymbol(omniDetectPlayerFire)

dealloc(detectPlayerFire)


// Cleanup of omnifyPlayerVitalsUpdateHook
omnifyPlayerVitalsUpdateHook:
    db F3 0F 11 82 90 01 00 00

unregistersymbol(omnifyPlayerVitalsUpdateHook)

dealloc(punchInTheFaceThreshold)
dealloc(playerVitalsUpdate)


// Cleanup of hit detection globals
unregistersymbol(playerAttacking)

dealloc(playerAttacking)


// Cleanup of omnifyApocalypseHook
omnifyApocalypseHook:
    db F3 0F 58 89 90 01 00 00

unregistersymbol(omnifyApocalypseHook)

dealloc(initiateApocalypse)


// Cleanup of omniPlayerHealthHook
omniPlayerHealthHook:
    db F3 0F 10 80 90 01 00 00

unregistersymbol(omniPlayerHealthHook)
unregistersymbol(playerHealth)

dealloc(playerHealth)
dealloc(getPlayerHealth)


// Cleanup of omniPlayerStaminaHook
omniPlayerStaminaHook:
    db 48 89 44 24 70

unregistersymbol(omniPlayerStaminaHook)
unregistersymbol(playerStamina)

dealloc(playerStamina)
dealloc(getPlayerStamina)