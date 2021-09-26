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

// Creates pointers to multiple structures containing important player data.
// Filtering is achieved by looking at the root containing structure pointed 
// at by rax.
// If it is the player, the base of this structure will be set to 0x64.
// We adjust the health structure's address by +0x10 bytes here as the game, 
// while accessing current health values using an 0x20 offset, will write to 
// it using a 0x10 offset.
// [rcx+20] | {[player]+10}: Current health.
// [rcx+18] | {[player]+8}: Maximum health.
// [rcx+48] | {[player]+38}: Current stamina.
// [rcx+4C] | {[player]+3C}: Maximum stamina.
// [rcx+B8] | {[player]+A8}: Location structure.
// [rcx+14C] | {[player]+13C}: Identifier for entity type.
// UNIQUE AOB: 41 8B C6 48 3B 41 20
define(omniPlayerHook,"nioh2.exe"+807269)

assert(omniPlayerHook,41 8B C6 48 3B 41 20)
alloc(getPlayer,$1000,omniPlayerHook)
alloc(player,8)
alloc(playerLocation,8)
alloc(recallYCorrection,8)

registersymbol(recallYCorrection)
registersymbol(playerLocation)
registersymbol(player)
registersymbol(omniPlayerHook)

getPlayer:
    pushf
    cmp [rax],64
    jne getPlayerOriginalCode
    sub rsp,10
    movdqu [rsp],xmm0
    push rax
    push rbx
    // Adjust the base address of our root structure so it is aligned with how it is when
    // being processed by various functions of significance (damage application code, etc).
    mov rax,rcx    
    add rax,0x10        
    mov [player],rax
    // Grab the player's location structure, and apply any pending Recall action to it.
    mov rax,[rcx+B8]
    mov [playerLocation],rax
    mov rbx,teleport
    cmp [rbx],1
    jne getPlayerCleanup    
    mov [rbx],0
    mov rbx,teleportX
    movss xmm0,[rbx]
    movss [rax+F0],xmm0
    movss [rax+220],xmm0
    mov rbx,teleportY
    movss xmm0,[rbx]
    // The game sometimes needs a bit of a vertical boost to avoid us from sinking in
    // the ground when we're teleporting somewhere arbitrary.
    addss xmm0,[recallYCorrection]
    movss [rax+F4],xmm0
    movss [rax+224],xmm0
    mov rbx,teleportZ
    movss xmm0,[rbx]
    movss [rax+F8],xmm0
    movss [rax+228],xmm0
getPlayerCleanup:
    pop rbx
    pop rax
    movdqu xmm0,[rsp]
    add rsp,10
getPlayerOriginalCode:
    popf
    mov eax,r14d
    cmp rax,[rcx+20]
    jmp getPlayerReturn

omniPlayerHook:
    jmp getPlayer
    nop 2
getPlayerReturn:


recallYCorrection:
    dd (float)150.0


// Gets the player's last location structure and ensures Omnified changes to the player's vertical position
// is properly reflected.
// [rsp+120] | {rsp+132}: Points to root structure of NPC the coordinates belong to.
// [rdi+C8]: Player's last known y-coordinate on solid ground.
// UNIQUE AOB: 89 87 C8 00 00 00 8B 47
// Correct instruction will be single result found in nioh2.exe (not nvd3dumx.dll).
define(omniPlayerLastLocationHook,"nioh2.exe"+8549BB)

assert(omniPlayerLastLocationHook,89 87 C8 00 00 00)
alloc(getPlayerLastLocation,$1000,omniPlayerLastLocationHook)
alloc(playerLastLocation,8)

registersymbol(playerLastLocation)
registersymbol(omniPlayerLastLocationHook)

getPlayerLastLocation:
    pushf
    // The player location structure must be initialized in order for us to identify the player's last location 
    // structure.
    push rax
    mov rax,playerLocation
    cmp [rax],0
    pop rax
    je getPlayerLastLocationOriginalCode
    push rbx
    push rcx
    mov rbx,playerLocation
    mov rcx,[rbx]    
    // The player's location structure can be found here on the stack.
    cmp rcx,[rsp+132]
    jne getPlayerLastLocationCleanup
    mov [playerLastLocation],rdi    
getPlayerLastLocationCleanup:
    pop rcx
    pop rbx
getPlayerLastLocationOriginalCode:
    popf
    mov [rdi+000000C8],eax
    jmp getPlayerLastLocationReturn

omniPlayerLastLocationHook:
    jmp getPlayerLastLocation
    nop 
getPlayerLastLocationReturn:


// Gets the structure containing the game's options.

// [rax+25]: Change target when lock-on target is defeated.
//           0: Manually
//           1: Automatically           
// [rax+2F]: Reset Camera Upon Guarding
// [rax+30]: Reset Camera When Pressing Lock-On Without a Target
// [rax+39]: Action Camera: When Under Attack
// [rax+3A]: Action Camera: Yokai Abilities
// [rax+3B]: Action Camera: Other
// [rax+3C]: Pause when window is inactive.
// [rax+3D]: Mute sound when window is inactive.
// [rax+50]: Display "TAKE" over dropped equipment.
//           0: All
//           1: Uncommon or Above
//           2: Rare or Above
//           3: Exotic or Above
//           6: None
// [rax+57]: Display Melee Weapon
//           0: All
//           1: Hide Secondary Weapon
//           2: Hide Sheathed Weapon
// [rax+58]: Display Ranged Weapon
//           0: All
//           1: Hide Secondary Weapon
//           2: Hide Sheathed Weapon
// [rax+59]: Display Blood Splatter on Player
//           0: Yes?
//           1: No!
// UNIQUE AOB: 8B 83 80 00 00 00 83 E8 03
// Correct instruction is 13 instructions down from the returned result.
define(omniGameOptionsHook,"nioh2.exe"+103B628)

assert(omniGameOptionsHook,80 78 3C 00 74 6D)
alloc(getGameOptions,$1000,omniGameOptionsHook)
alloc(gameOptions,8)
alloc(pauseWhenInactive,8)

registersymbol(pauseWhenInactive)
registersymbol(gameOptions)
registersymbol(omniGameOptionsHook)

getGameOptions:
    pushf
    mov [gameOptions],rax
    push rbx
    mov rbx,[pauseWhenInactive]
    mov [rax+3C],rbx
    pop rbx
getGameOptionsOriginalCode:
    popf
    cmp byte ptr [rax+3C],00
    je nioh2.exe+103B69B
    jmp getGameOptionsReturn

omniGameOptionsHook:
    jmp getGameOptions
    nop 
getGameOptionsReturn:


pauseWhenInactive:
    dd 0


// Increments the player's death counter (upon player death...DUH).
define(omniDeathCounterHook,"nioh2.exe"+79D38F)

assert(omniDeathCounterHook,48 C7 41 10 00 00 00 00)
alloc(incrementDeathCounter,$1000,omniDeathCounterHook)
alloc(deathCounter,8)

registersymbol(deathCounter)
registersymbol(omniDeathCounterHook)

incrementDeathCounter:
    pushf
    // Make sure our player structure is initialized. It really should be, but lets leave nothing to chance.    
    push rax
    mov rax,player
    cmp [rax],0
    pop rax
    je incrementDeathCounterOriginalCode
    push rax
    mov rax,player
    cmp [rax],rcx
    pop rax
    jne incrementDeathCounterOriginalCode
    inc [deathCounter]
incrementDeathCounterOriginalCode:
    popf
    mov qword ptr [rcx+10],00000000
    jmp incrementDeathCounterReturn

omniDeathCounterHook:
    jmp incrementDeathCounter
    nop 3
incrementDeathCounterReturn:


deathCounter:
    dd 0


// Gets the player's Amrita.
// rbx: Amrita value.
// UNIQUE AOB: 45 33 E4 4D 89 7B D8
define(omniAmritaHook,"nioh2.exe"+CB33F5)

assert(omniAmritaHook,45 33 E4 4D 89 7B D8)
alloc(getAmrita,$1000,omniAmritaHook)
alloc(playerAmrita,8)

registersymbol(playerAmrita)
registersymbol(omniAmritaHook)

getAmrita:
    mov [playerAmrita],rbx
getAmritaOriginalCode:
    xor r12d,r12d
    mov [r11-28],r15
    jmp getAmritaReturn

omniAmritaHook:
    jmp getAmrita
    nop 2
getAmritaReturn:


// Gets the player's Amrita gauge (use to trigger Yokai Shift).
define(omniAmritaGaugeHook,"nioh2.exe"+82B85B)

assert(omniAmritaGaugeHook,F3 0F 10 80 C4 00 00 00)
alloc(getAmritaGauge,$1000,omniAmritaGaugeHook)
alloc(amritaGauge,8)

registersymbol(amritaGauge)
registersymbol(omniAmritaGaugeHook)

getAmritaGauge:
    pushf
    push rbx
    lea rbx,[rax+C4]
    mov [amritaGauge],rbx
    pop rbx
getAmritaGaugeOriginalCode:
    popf
    movss xmm0,[rax+000000C4]
    jmp getAmritaGaugeReturn

omniAmritaGaugeHook:
    jmp getAmritaGauge
    nop 3
getAmritaGaugeReturn:


// Processes Omnified events during execution of the location update code for the player.
// rcx: The target location structure being updated.
// UNIQUE AOB: 66 0F 7F 81 F0 00 00 00 C3
define(omnifyLocationUpdateHook,"nioh2.exe"+801863)

assert(omnifyLocationUpdateHook,66 0F 7F 81 F0 00 00 00)
alloc(updateLocation,$1000,omnifyLocationUpdateHook)
alloc(teleportLocation,16)
alloc(framesToSkip,8)

registersymbol(omnifyLocationUpdateHook)

updateLocation:
    pushf
    // Since we need to update it upon a teleport occurring, the last location structure player is required for 
    // further processing.
    push rax
    mov rax,playerLastLocation
    cmp [rax],0
    pop rax
    je updateLocationOriginalCode
    // Only events pertaining to the player are processed here.
    push rax
    mov rax,playerLocation
    cmp [rax],rcx
    pop rax
    jne updateLocationOriginalCode
    // Upon a teleport, we engage in a movement frame skip sequence, this is so the desired teleport coordinates 
    // actually get committed to the player's location (and stay there); otherwise, an army of validation-related 
    // code will revert the coordinates depending on the situation.
    cmp [framesToSkip],0
    jg skipMovementFrame
    push rax
    mov rax,teleported
    cmp [rax],1
    pop rax
    jne updateLocationOriginalCode
    push rax
    push rbx
    push rcx
    mov rax,teleported
    mov [rax],0    
    mov [framesToSkip],#25
    mov rbx,playerLastLocation
    mov rax,[rbx]
    // Need to set our last location (vertically) so that proper fall damage will occur if we just got Tom Petty'd.
    mov rcx,teleportedY
    mov rbx,[rcx]
    mov [rax+C8],rbx
    pop rcx
    pop rbx
    pop rax    
skipMovementFrame:    
    dec [framesToSkip]        
    push rax
    push rbx        
    push rcx
    // Simply ignoring the updated coordinates in xmm0 is not enough, as other validation code might've already 
    // reverted our source-of-truth coordinates. So, we load up the teleport coordinates set during the Apocalypse
    // pass.
    mov rcx,teleportLocation
    movdqu [rcx],xmm0
    mov rax,teleportedX
    mov rbx,[rax]
    mov [rcx],rbx
    mov rax,teleportedY
    mov rbx,[rax]
    mov [rcx+4],rbx
    mov rax,teleportedZ
    mov rbx,[rax]
    mov [rcx+8],rbx    
    movdqu xmm0,[rcx]
    pop rcx
    pop rbx
    pop rax    
    // Update the validation, destination coordinates as well.
    movdqu [rcx+220],xmm0
updateLocationOriginalCode:
    popf
    movdqa [rcx+000000F0],xmm0
    jmp updateLocationReturn

omnifyLocationUpdateHook:
    jmp updateLocation
    nop 3
updateLocationReturn:


// Detects if the player is responsible for a damaging attack.
// UNIQUE AOB: 4C 8B E8 48 85 C0 0F 84 B4
define(omnifyPlayerHitDetectionHook,"nioh2.exe"+8B065D)

assert(omnifyPlayerHitDetectionHook,4C 8B E8 48 85 C0)
alloc(detectPlayerHit,$1000,omnifyPlayerHitDetectionHook)
alloc(playerAttacking,8)

registersymbol(playerAttacking)
registersymbol(omnifyPlayerHitDetectionHook)

detectPlayerHit:
    pushf
    push rbx
    mov rbx,playerLocation
    cmp [rbx],rax
    pop rbx
    jne detectPlayerHitOriginalCode
    mov [playerAttacking],1
detectPlayerHitOriginalCode:
    popf
    mov r13,rax
    test rax,rax
    jmp detectPlayerHitReturn

omnifyPlayerHitDetectionHook:
    jmp detectPlayerHit
    nop 
detectPlayerHitReturn:


// Initiates the Apocalypse system.
// This is Nioh 2's damage application code.
// [rbx+10]: Working health.
// edi: Damage amount.
// rbx: Target entity (i.e. [player] or an NPC) structure.
// UNIQUE AOB: 8B 43 10 2B C7
// Correct instruction will be single result found in nioh2.exe (not the other two DLL's).
define(omnifyApocalypseHook,"nioh2.exe"+79C590)

assert(omnifyApocalypseHook,8B 43 10 2B C7)
alloc(initiateApocalypse,$1000,omnifyApocalypseHook)

registersymbol(omnifyApocalypseHook)

initiateApocalypse:
    pushf
    // An empty r12 register indicates the damage originates from falling.
    // We don't want this to trigger Apocalypse, as it may have been Apocalypse that caused the 
    // falling...
    cmp r12,0
    je initiateApocalypseOriginalCode
    // A r15 register set to 1 means we're drowning like a big dumb baby.
    cmp r15,1
    je initiateApocalypseOriginalCode
    // An r8 register set to 0x400 indicates environmental fire damage.
    cmp r8,0x400
    je initiateApocalypseOriginalCode    
executeApocalypse:
    // Ensure the required player data structures are initialized.
    push rax
    mov rax,player
    cmp [rax],0
    pop rax
    je initiateApocalypseOriginalCode
    push rax
    mov rax,playerLocation
    cmp [rax],0
    pop rax
    je initiateApocalypseOriginalCode
    // Backing up a SSE register to hold converted floating point values for the health
    // and damage amount.
    sub rsp,10
    movdqu [rsp],xmm0
    // Backing up the outputs of the Apocalypse system.
    push rax
    push rbx
    // Backing up a register to hold the address pointed to by rbx, as we need to write one 
    // of our outputs to it when all is said and done.
    push rcx    
    mov rcx,rbx    
    // Both Player and Enemy Apocalypse functions share the same first two parameters. 
    // Let's load them first before figuring out which subsystem to execute.
    // We'll need to convert the working health and damage amount values from being integer types 
    // to floating point types, as this is the data type expected by the Apocalypse system.    
    cvtsi2ss xmm0,edi    
    // Load the damage amount parameter.
    sub rsp,8
    movd [rsp],xmm0
    mov rax,[rcx+10]
    cvtsi2ss xmm0,rax
    // Load the working health amount parameter.
    sub rsp,8
    movd [rsp],xmm0    
    // Now, we need to determine whether the player or an NPC is being damaged, and then from 
    // there execute the appropriate Apocalypse subsystem.
    mov rax,player        
    cmp [rax],rcx
    je initiatePlayerApocalypse
    jmp initiateEnemyApocalypse    
initiatePlayerApocalypse:        
    // Check if the normal damage is enough (alone) to kill the player -- if it is, we forbid 
    // teleports, as we cannot move the character after he or she is dead.
    mov rax,[rcx+10]
    sub eax,edi
    cmp eax,0
    jg skipTeleportitisDisable
    mov rax,disableTeleportitis
    mov [rax],1
skipTeleportitisDisable:
    // Convert the maximum health for the player to the expected floating point form.    
    mov rax,[rcx+8]
    cvtsi2ss xmm0,rax
    // Load the maximum health parameter.
    sub rsp,8
    movd [rsp],xmm0
    // Align the player's location coordinate structure so it begins at our x-coordinate and pass 
    // that as the final parameter.
    mov rax,playerLocation
    mov rbx,[rax]
    lea rax,[rbx+F0]
    push rax
    call executePlayerApocalypse
    jmp initiateApocalypseUpdateDamage
initiateEnemyApocalypse:
    // Check if the player is responsible for the attack, if not, then the damage may be from a friendly 
    // NPC, or otherwise berserk and rampaging Yokai.
    mov rax,playerAttacking
    cmp [rax],1
    jne abortEnemyApocalypse
    call executeEnemyApocalypse
    jmp initiateApocalypseUpdateDamage
abortEnemyApocalypse:
    add rsp,10
    jmp initiateApocalypseCleanup
initiateApocalypseUpdateDamage:
    // To make use of the updated damage and working health amounts returned by the Apocalypse 
    // system, we'll need to convert them both back to integer form.
    movd xmm0,eax
    cvtss2si edi,xmm0
    movd xmm0,ebx
    cvtss2si ebx,xmm0
    mov [rcx+10],ebx
    mov rax,disableTeleportitis
    mov [rax],0
initiateApocalypseCleanup:
    mov rax,playerAttacking
    mov [rax],0
    pop rcx
    pop rbx
    pop rax
    movdqu xmm0,[rsp]
    add rsp,10
initiateApocalypseOriginalCode:
    popf
    mov eax,[rbx+10]
    sub eax,edi
    jmp initiateApocalypseReturn

omnifyApocalypseHook:
    jmp initiateApocalypse
initiateApocalypseReturn:


negativeVerticalDisplacementEnabled:
    dd 0

teleportitisDisplacementX:
    dd (float)180.0

gokuResultUpper:
    dd #1500

fatalisBloodlustDamageX:
    dd (float)1.5


// Initiates the Predator system.
// [rdx+0-8]: Movement offsets for x, y, and z-coordinates respectively.
// [rbx]: Target location structure
// UNIQUE AOB: F3 0F 58 02 F3 0F 11 81 80 01 00 00
define(omnifyPredatorHook,"nioh2.exe"+852588)

assert(omnifyPredatorHook,F3 0F 58 02 F3 0F 11 81 80 01 00 00)
alloc(initiatePredator,$1000,omnifyPredatorHook)
alloc(identityValue,8)
alloc(playerSpeedX,8)

registersymbol(playerSpeedX)
registersymbol(identityValue)
registersymbol(omnifyPredatorHook)

initiatePredator:
    pushf
    push rax
    mov rax,playerLocation
    cmp [rax],0
    pop rax
    je initiatePredatorOriginalCode
    // In some instances, rbx does not point to a valid location structure, such as when 
    // a shrine is being used.
    cmp rbx,1
    je initiatePredatorOriginalCode
    // Make sure the player isn't being treated as an enemy NPC!
    push rax
    mov rax,playerLocation
    cmp [rax],rbx
    pop rax
    je applyPlayerSpeed
    // Backing up the registers used to hold Predator system output, as well as an SSE to 
    // hold some of the parameters we'll be passing.
    sub rsp,10
    movdqu [rsp],xmm0
    push rax
    push rbx
    push rcx
    // The first parameter is our player's coordinates.
    mov rax,playerLocation
    mov rcx,[rax]
    push [rcx+F0]
    push [rcx+F8]
    // The next parameter is the target NPC's coordinates.
    push [rbx+F0]
    push [rbx+F8]
    // The third parameter is the NPC's dimensional scales. Jury is still out whether this 
    // game has True Scaling, so we'll just be passing a good ol' identity matrix for now.
    movss xmm0,[identityValue]
    shufps xmm0,xmm0,0
    sub rsp,10
    movdqu [rsp],xmm0
    // The fourth parameter is the NPC's movement offsets. Wow this has been easy!
    push [rdx]
    push [rdx+8]
    call executePredator
    // Now we just take the updated movement offsets and dump them back into [rdx].
    mov [rdx],eax
    mov [rdx+4],ebx
    mov [rdx+8],ecx
    pop rcx
    pop rbx
    pop rax
    movdqu xmm0,[rsp]
    add rsp,10
    jmp initiatePredatorOriginalCode
applyPlayerSpeed:
    sub rsp,10
    movdqu [rsp],xmm0    
    sub rsp,10
    movdqu [rsp],xmm1
    push rax
    sub rsp,10
    // We don't use the Predator system to influence player speed; rather, we just introduce
    // the use of a simple multiplier that is applied to the character's movement offsets.
    // This multiplier will be, except under special circumstances, typically 1x.
    movss xmm0,[playerSpeedX]
    shufps xmm0,xmm0,0
    movups [rsp],xmm0
    mov rax,[identityValue]
    mov [rsp+4],eax
    mov [rsp+C],eax
    movups xmm1,[rsp]
    movups xmm0,[rdx]
    mulps xmm0,xmm1
    movups [rdx],xmm0
    add rsp,10    
    pop rax
    movdqu xmm1,[rsp]
    add rsp,10
    movdqu xmm0,[rsp]
    add rsp,10
initiatePredatorOriginalCode:
    popf
    addss xmm0,[rdx]
    movss [rcx+00000180],xmm0
    jmp initiatePredatorReturn

omnifyPredatorHook:
    jmp initiatePredator
    nop 7
initiatePredatorReturn:


identityValue:
    dd (float)1.0

aggroDistance:
    dd (float)1250.0

threatDistance:
    dd (float)300.0

positiveLimit:
    dd (float)1000.0

negativeLimit:
    dd (float)-1000.0

playerSpeedX:
    dd (float)1.0


// Initiates the Abomnification system.
// This polls both the player's and NPC coordinates.
// [rdi+140]: Height
// [rdi+144]: Depth
// [rdi+148]: Width
// UNIQUE AOB: 4C 8D BF F0 00 00 00 41
define(omnifyAbomnificationHook,"nioh2.exe"+84AA21)

assert(omnifyAbomnificationHook,4C 8D BF F0 00 00 00)
alloc(initiateAbomnification,$1000,omnifyAbomnificationHook)
alloc(abomnifyPlayer,8)

registersymbol(abomnifyPlayer)
registersymbol(omnifyAbomnificationHook)

initiateAbomnification:
    pushf
    push rax
    mov rax,playerLocation
    cmp [rax],rdi
    pop rax
    jne skipAbomnifyPlayerCheck
    cmp [abomnifyPlayer],1
    jne initiateAbomnificationOriginalCode
skipAbomnifyPlayerCheck:
    // Back up the registers used as outputs of the Abomnification system.
    push rax
    push rbx
    push rcx
    // Push the address to the creature's location structure as its identifying
    // address to the stack.
    push rdi
    call executeAbomnification
    // Load the Abomnified scales into the creature's location structure.
    mov [rdi+148],eax
    mov [rdi+140],ebx
    mov [rdi+144],ecx
    pop rcx
    pop rbx
    pop rax
initiateAbomnificationOriginalCode:
    popf
    lea r15,[rdi+000000F0]
    jmp initiateAbomnificationReturn

omnifyAbomnificationHook:
    jmp initiateAbomnification
    nop 2
initiateAbomnificationReturn:


abominifyMorphStepsResultUpper:
    dd #550

abominifyHeightResultUpper:
    dd #200

abominifyDepthResultUpper:
    dd #180

abominifyWidthResultUpper:
    dd #240

unnaturalBigX:
    dd (float)1.5

abomnifyPlayer:
    dd 0


[DISABLE]

// Cleanup of omniPlayerHook
omniPlayerHook:
    db 41 8B C6 48 3B 41 20

unregistersymbol(omniPlayerHook)
unregistersymbol(player)
unregistersymbol(playerLocation)
unregistersymbol(recallYCorrection)

dealloc(recallYCorrection)
dealloc(playerLocation)
dealloc(player)
dealloc(getPlayer)


// Cleanup of omniPlayerLastLocationHook
omniPlayerLastLocationHook:
    db 89 87 C8 00 00 00

unregistersymbol(omniPlayerLastLocationHook)
unregistersymbol(playerLastLocation)

dealloc(playerLastLocation)
dealloc(getPlayerLastLocation)


// Cleanup of omniGameOptionsHook
omniGameOptionsHook:
    db 80 78 3C 00 74 6D

unregistersymbol(omniGameOptionsHook)
unregistersymbol(gameOptions)
unregistersymbol(pauseWhenInactive)

dealloc(pauseWhenInactive)
dealloc(gameOptions)
dealloc(getGameOptions)


// Cleanup of omniDeathCounterHook
omniDeathCounterHook:
    db 48 C7 41 10 00 00 00 00

unregistersymbol(omniDeathCounterHook)
unregistersymbol(deathCounter)

dealloc(deathCounter)
dealloc(incrementDeathCounter)


// Cleanup of omniAmritaHook
omniAmritaHook:
    db 45 33 E4 4D 89 7B D8

unregistersymbol(omniAmritaHook)
unregistersymbol(playerAmrita)

dealloc(playerAmrita)
dealloc(getAmrita)


// Cleanup of omniAmritaGaugeHook
omniAmritaGaugeHook:
    db F3 0F 10 80 C4 00 00 00

unregistersymbol(omniAmritaGaugeHook)
unregistersymbol(amritaGauge)

dealloc(amritaGauge)
dealloc(getAmritaGauge)


// Cleanup of omnifyLocationUpdateHook
omnifyLocationUpdateHook:
    db 66 0F 7F 81 F0 00 00 00

unregistersymbol(omnifyLocationUpdateHook)
dealloc(framesToSkip)
dealloc(teleportLocation)
dealloc(updateLocation)


// Cleanup of omnifyPlayerHitDetectionHook
omnifyPlayerHitDetectionHook:
    db 4C 8B E8 48 85 C0

unregistersymbol(omnifyPlayerHitDetectionHook)
unregistersymbol(playerAttacking)

dealloc(playerAttacking)
dealloc(detectPlayerHit)


// Cleanup of omnifyApocalypseHook
omnifyApocalypseHook:
    db 8B 43 10 2B C7

unregistersymbol(omnifyApocalypseHook)

dealloc(initiateApocalypse)


// Cleanup of omnifyPredatorHook
omnifyPredatorHook:
    db F3 0F 58 02 F3 0F 11 81 80 01 00 00

unregistersymbol(omnifyPredatorHook)
unregistersymbol(identityValue)
unregistersymbol(playerSpeedX)

dealloc(playerSpeedX)
dealloc(identityValue)
dealloc(initiatePredator)


// Cleanup of omnifyAbomnificationHook
omnifyAbomnificationHook:
    db 4C 8D BF F0 00 00 00

unregistersymbol(omnifyAbomnificationHook)
unregistersymbol(abomnifyPlayer)

dealloc(abomnifyPlayer)
dealloc(initiateAbomnification)