// Hooks for Omnified Cyberpunk 2077
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC

// Get the player's vitals.
// Unique AOB: F3 0F 10 80 90 01 00 00 0F 54
define(omniPlayerVitalsHook,"Cyberpunk2077.exe"+1B5F32B)

assert(omniPlayerVitalsHook,F3 0F 10 80 90 01 00 00)
alloc(getPlayerVitals,$1000,omniPlayerVitalsHook)
alloc(playerVitals,8)

registersymbol(omniPlayerVitalsHook)
registersymbol(playerVitals)

getPlayerVitals:
    pushf
    cmp r8,0
    jne getPlayerVitalsOriginalCode
    push rbx
    mov rbx,playerVitals
    mov [rbx],rax
    pop rbx
getPlayerVitalsOriginalCode:
    popf
    movss xmm0,[rax+00000190]
    jmp getPlayerVitalsReturn

omniPlayerVitalsHook:
    jmp getPlayerVitals
    nop 3
getPlayerVitalsReturn:


// Get the player's location.
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


[DISABLE]

// Cleanup of omniPlayerVitalsHook
omniPlayerVitalsHook:
    db F3 0F 10 80 90 01 00 00

unregistersymbol(omniPlayerVitalsHook)
unregistersymbol(playerVitals)

dealloc(playerVitals)
dealloc(getPlayerVitals)


// Cleanup of omniPlayerLocationHook
omniPlayerLocationHook:
    db 0F 10 81 10 02 00 00

unregistersymbol(omniPlayerLocationHook)
unregistersymbol(playerLocation)

dealloc(playerLocation)
dealloc(getPlayerLocation)
