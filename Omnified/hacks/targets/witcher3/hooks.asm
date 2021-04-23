// Hooks for Omnified Witcher 3
// Written By: Matt Weber (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC

// Gets the player's vitals.
define(omniPlayerVitalsHook, "witcher3.exe" + E3BEB1)

assert(omniPlayerVitalsHook, 8B 0C 90 89 0E)
alloc(getPlayerVitals,$1000, omniPlayerVitalsHook)
alloc(playerVitals,8)
alloc(playerAbilityManager,8)

registersymbol(omniPlayerVitalsHook)
registersymbol(playerVitals)
registersymbol(playerAbilityManager)

getPlayerVitals:
  pushf
  push rbx
  // When the player is defending someone, their health bar is added to the screen and interferes
  // with this code. We use this field as an identifier.
  mov rbx,[rax+0x20]
  // A value of 0x5 indicates Keira. 
  cmp rbx,0x5
  je getPlayerVitalsExit
  // A value of 0x74 indicates Princess (Goat).
  cmp ebx,0x74
  je getPlayerVitalsExit
  mov rbx,playerVitals
  mov [rbx],rax
  mov rbx,playerAbilityManager
  mov [rbx],rcx
getPlayerVitalsExit:
  pop rbx
getPlayerVitalsOriginalCode:
  popf
  mov ecx,[rax+rdx*4]
  mov [rsi],ecx
  jmp getPlayerVitalsReturn


omniPlayerVitalsHook:
  jmp getPlayerVitals
  
getPlayerVitalsReturn:


// Gets the player's immediate physics wrapper.
define(omniPlayerPhysicsHook, "witcher3.exe" + 30FCA9)

assert(omniPlayerPhysicsHook, 0F 2F 83 98 01 00 00)
alloc(getPlayerPhysics,$1000, omniPlayerPhysicsHook)
alloc(playerPhysics,8)

registersymbol(omniPlayerPhysicsHook)
registersymbol(playerPhysics)

getPlayerPhysics:
  pushf
  push rax
  mov rax,playerPhysics
  mov [rax],rbx
  pop rax
getPlayerPhysicsOriginal:
  popf
  comiss xmm0,[rbx+00000198]
  jmp getPlayerPhysicsReturn

omniPlayerPhysicsHook:
  jmp getPlayerPhysics
  nop 2
getPlayerPhysicsReturn:


// Gets the player's root structure as well as resolving the source of truth location coordinate structure 
// found on it.
define(omniPlayerHook,"witcher3.exe"+AB139)

assert(omniPlayerHook,48 8B 01 48 8D 54 24 60)
alloc(getPlayer,$1000,omniPlayerHook)
alloc(player,8)
alloc(playerLocation,8)

registersymbol(omniPlayerHook)
registersymbol(player)
registersymbol(playerLocation)

getPlayer:
  pushf
  push rax
  push rbx
  push rcx
  mov rax,[rcx]
  cmp ax,0xC418
  jne getPlayerExit
  mov rax,player
  mov [rax],rcx
  // Grab the CMovingPhysicalAgentComponent.
  mov rbx,[rcx+0x218]
  // Grab the CMRPhysicalCharacter.
  mov rcx,[rbx+0x1648]
  // Grab the source CPhysicsCharacterWrapper.
  mov rbx,[rcx+0x10]
  // Grab the location coordinates holder.
  mov rcx,[rbx+0x78]
  mov rax,playerLocation
  add rcx,8
  mov [rax],rcx
getPlayerExit:
  pop rcx
  pop rbx
  pop rax
getPlayerOriginalCode:
  popf
  mov rax,[rcx]
  lea rdx,[rsp+60]
  jmp getPlayerReturn

omniPlayerHook:
  jmp getPlayer
  nop 3
getPlayerReturn:

// Gets the player's XP data.
define(omniPlayerXPHook,"witcher3.exe"+AEB63)

assert(omniPlayerXPHook,49 8B D7 48 8B CB)
alloc(getPlayerXP,$1000,omniPlayerXPHook)
alloc(playerXP,8)

registersymbol(omniPlayerXPHook)
registersymbol(playerXP)

getPlayerXP:
  pushf
  cmp rax,0x2
  jne getPlayerXPOriginalCode
  cmp rbp,0x0
  jne getPlayerXPOriginalCode
  push rax
  push rcx
  mov rcx,[rbx+8]
  mov rax,[rcx]
  call qword ptr [rax+10]
  imul rax,r8d
  add rax,[r15]
  mov rcx,playerXP
  mov [rcx],rax
  pop rcx
  pop rax
getPlayerXPOriginalCode:
  popf
  mov rdx,r15
  mov rcx,rbx
  jmp getPlayerXPReturn

omniPlayerXPHook:
  jmp getPlayerXP
  nop 
getPlayerXPReturn:


// Reduces the amount of XP gained.
// eax: Amount of new XP.
define(omnifyXPHook,"witcher3.exe"+1651690)

assert(omnifyXPHook,01 03 8B 03 48 85 F6)
alloc(changeXP,$1000,omnifyXPHook)
alloc(xpX, 8)

registersymbol(omnifyXPHook)
registersymbol(xpX)

changeXP:
  pushf
  push rax
  mov rax,playerXP
  cmp rbx,[rax]
  pop rax
  jne changeXPOriginalCode
  sub rsp,10
  movdqu [rsp],xmm0
  cvtsi2ss xmm0,eax
  mulss xmm0,[xpX]
  cvtss2si eax,xmm0
  movdqu xmm0,[rsp]
  add rsp,10
changeXPOriginalCode:
  popf
  add [rbx],eax
  mov eax,[rbx]
  test rsi,rsi
  jmp changeXPReturn

omnifyXPHook:
  jmp changeXP
  nop 2
changeXPReturn:


xpX:
  dd (float)0.69


// Initiates the Apocalypse system.
// [rsp+40] | {rsp+62}: Damage amount.
// xmm0: Player's current health.
// r12: Data source for values being calculated.
// Conditions
// ----------
// If player is being targeted:
//
// [rsp+A0] | {rsp+C2}: Value is 0 if a vital stat amount is changing for Geralt.
// A value of 0 means either a non-vital stat is changing or that Ciri is losing a vital stat amount. Confusing, I know.
// [rsp+50] | {rsp+72}: Value is 0 when Geralt's stamina is changing. Value is 0 when Ciri might be losing health.
// [rsp+80] | {rsp+A2}: Value is 0 when Geralt's health is changing (assuming [rsp+50] is not 0).
// Value is not 0 when Ciri is losing health (assuming [rsp+50] is 0). Yes. Confusing lol.
// [rsp+68] | {rsp+8A}: If Geralt's health is changing, we must ensure that this points to some kind of data, but 
// that it does not point to data of type CFunction. If it does, then the operation is most likely a regenerative 
// one. This does not apply to Ciri for some reason.
//
// If non-player is being targeted:
//
// [rsp+410] | {rsp+432}: Set to player's ability manager when player is damaging SOME enemies.
// For A VERY FEW number of enemies, this will be set to the player's root structure.
// [rsp+8] | {rsp+2A}: Set to player's ability manager when player is damaging SOME enemies.
// [rsp+68] | {rsp+8A}: Set to player's ability manager when player is damaging MOST enemies.
define(omnifyApocalypseHook,"witcher3.exe"+16529F4)

assert(omnifyApocalypseHook,F3 0F 5C 44 24 40)
alloc(initiateApocalypse,$1000,omnifyApocalypseHook)

registersymbol(omnifyApocalypseHook)

initiateApocalypse:
  pushf
  // Ensure that player ability manager pointer has been initialized.
  push rax
  mov rax,playerAbilityManager
  cmp [rax],0
  pop rax
  je initiateApocalypseOriginalCode
  sub rsp,10
  movdqu [rsp],xmm1
  push rax
  push rbx
  // Check if operation is being done on an entity's ability manager first.
  // r12 is the source of the values being calculated.
  cmp r12,0
  // If r12 isn't set, this has nothing to do with damage.
  je initiateApocalypseExit
  mov rax,[r12]
  // Lower 2 bytes of W3AbilityManager's type identity is 0x0F78.
  cmp ax,0x0F78
  jne initiateApocalypseExit  
  // Check if player is damaging an enemy. 
  // If we're damaging an enemy, r12 will not point to our own ability manager.
  mov rax,playerAbilityManager
  cmp [rax],r12
  je verifyPlayerDamaged
  // This will point to our ability manager for MOST enemies when we're responsible for the damage.
  mov rbx,[rsp+8A]
  cmp [rax],rbx
  je initiateEnemyApocalypse
  // This will point to our ability manager for SOME enemies when we're responsible for the damage.
  mov rbx,[rsp+432]
  cmp [rax],rbx
  je initiateEnemyApocalypse
  // This will point to our ability manager for SOME enemies when we're responsible for the damage.
  mov rbx,[rsp+2A]
  cmp [rax],rbx
  je initiateEnemyApocalypse  
  // This will point to our root structure for a select few enemies when we're responsible for the damage.
  mov rax,player
  mov rbx,[rsp+432]
  cmp [rax],rbx
  je initiateEnemyApocalypse
  jmp initiateApocalypseExit  
verifyPlayerDamaged:
  // If our ability manager is the data source, then this set to 0 indicates a vital stat amount is changing
  // for Geralt.  
  mov eax,[rsp+C2]
  cmp eax,0
  jne verifyCiriDamaged
  // Check if Geralt's stamina is changing.
  // This is 0 when stamina is changing.
  mov eax,[rsp+72]
  cmp eax,0
  je initiateApocalypseExit
  // If stamina is not changing, check if Geralt's health is changing.
  // This is 0 when health is changing.
  mov rax,[rsp+A2]
  cmp eax,0
  jne initiateApocalypseExit  
  // This must point to something, it just can't point to a function.
  mov rbx,[rsp+8A]
  push rcx
  lea rcx,[rbx]
  call checkBadPointer
  cmp ecx,0
  pop rcx  
  jne initiateApocalypseExit  
  // If this points to something, it must not be a function. If it is, then the operation is the result
  // of a recurring regenerative type function. If it isn't, then this operation is being caused by damage
  // from an enemy.
  mov rax,[rbx]
  cmp ax,0xFA58
  jne initiatePlayerApocalypse
  jmp initiateApocalypseExit  
verifyCiriDamaged:
  // If we're actually playing Geralt and we have both some toxicity and adrenaline, we might end up here.
  // This will be 0 if we're actually Ciri.
  mov rax,[rsp+412]
  cmp eax,0
  jne initiateApocalypseExit
  // For Ciri, this is not 0 (unlike Geralt) when (assumingly) stamina is changing.
  // If it is 0, then Ciri may be losing health.
  mov eax,[rsp+72]
  cmp eax,0
  jne initiateApocalypseExit
  // For Ciri, if stamina isn't changing, then this is not 0 when losing health.
  // No check for a recurring function is required for Ciri.
  mov rax,[rsp+A2]
  cmp eax,0
  je initiateApocalypseExit  
initiatePlayerApocalypse:
  // Push the damage amount parameter.
  movss xmm1,[rsp+62]
  sub rsp,8
  movd [rsp],xmm1
  // Push the player's current working health value.
  sub rsp,8
  movd [rsp],xmm0
  // Push the player's maximum health value.
  mov rax,playerVitals
  mov rbx,[rax]
  movss xmm1,[rbx+4]
  sub rsp,8
  movd [rsp],xmm1
  // Align the player's location struct to the X coordinate and push it.
  mov rax,playerLocation
  mov rbx,[rax]
  lea rax,[rbx+1B8]
  push rax
  call executePlayerApocalypse
  jmp initiateApocalypseUpdateDamage
initiateEnemyApocalypse:  
  // Push the damage amount parameter.
  movss xmm1,[rsp+62]
  sub rsp,8
  movd [rsp],xmm1
  // Push the target's current working health value.
  sub rsp,8
  movd [rsp],xmm0
  call executeEnemyApocalypse
initiateApocalypseUpdateDamage:
  // Commit updated damage and working health.
  mov [rsp+62],eax
  movd xmm0,ebx
initiateApocalypseExit:
  pop rbx
  pop rax
  movdqu xmm1,[rsp]
  add rsp,10
initiateApocalypseOriginalCode:
  popf
  subss xmm0,[rsp+40]
  jmp initiateApocalypseReturn
omnifyApocalypseHook:
  jmp initiateApocalypse
  nop 
initiateApocalypseReturn:

coordinatesAreDoubles:
  dd 1

negativeVerticalDisplacementEnabled:
  dd 0

yIsVertical:
  dd 0

teleportitisDisplacementX:
  dd (float)3.0


// Applies a multipler to the player's speed.
// This function is always called for the moving player -- this includes Roach (horse), but only while
// the player is riding on Roach.
// The coordinates of the moving player may not necessarily be the playerLocation pointer, as a distinct structure
// in memory is used when the player is riding Roach.
// [rsi+1B8]: Start of player's current coordinates.
// [rbx+8]: Start of player's new coordinates.
define(omnifyPlayerSpeedHook,"witcher3.exe"+F2971B)

assert(omnifyPlayerSpeedHook,48 8B 43 08 48 89 86 B8 01 00 00)
alloc(applyPlayerSpeed,$1000,omnifyPlayerSpeedHook)
alloc(playerSpeedX,8)
alloc(playerVerticalX,8)
alloc(identityValue,8)

registersymbol(omnifyPlayerSpeedHook)
registersymbol(playerSpeedX)
registersymbol(playerVerticalX)

applyPlayerSpeed:
  pushf
  push rax
  mov rax,playerLocation
  cmp rax,0
  pop rax
  je applyPlayerSpeedOriginalCode
  // We'll need to back up a few SSE registers to aid double->float->double conversions.
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  sub rsp,10
  movdqu [rsp],xmm2
  sub rsp,10
  movdqu [rsp],xmm3
  // Convert the new coordinates to floating point.
  // This structure is not aligned properly, so need to unaligned-move it to an SSE first.
  movupd xmm1,[rbx+8]
  cvtpd2ps xmm0,xmm1
  cvtsd2ss xmm1,[rbx+18]
  movlhps xmm0,xmm1  
  // Convert the current coordinates to floating point.
  cvtpd2ps xmm1,[rsi+1B8]
  cvtsd2ss xmm2,[rsi+1C8]
  movlhps xmm1,xmm2 
  // Calculate the movement offsets that were applied to the target's current coordinates.
  subps xmm0,xmm1  
  // Load the multiplier to be applied to the Z axis (typically 1x to prevent vertical boost, but we have it 
  // configurable so I can entertain viewers with stupid high jumps) as well as a 1x multiplier for any hanger-on 
  // value that may follow the X/Y/Z coords.
  movss xmm3,[playerVerticalX]  
  movss xmm2,[identityValue]
  movlhps xmm3,xmm2
  shufps xmm3,xmm3,0x88
  // Load player speed multiplier...
  movss xmm2,[playerSpeedX]
  // Prime the multipliers.
  shufps xmm2,xmm3,0x40
  // Multiply dem offsets!
  mulps xmm0,xmm2
  // Add the potentially modified target offsets back to the original coordinates to get the new coordinates.
  addps xmm0,xmm1
  cvtps2pd xmm1,xmm0
  movhlps xmm0,xmm0
  cvtss2sd xmm2,xmm0
  movupd [rbx+8], xmm1
  movsd [rbx+18], xmm2
applyPlayerSpeedCleanup:
  movdqu xmm3,[rsp]
  add rsp,10
  movdqu xmm2,[rsp]
  add rsp,10
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10  
applyPlayerSpeedOriginalCode:
  popf
  mov rax,[rbx+08]
  mov [rsi+000001B8],rax
  jmp applyPlayerSpeedReturn

omnifyPlayerSpeedHook:
  jmp applyPlayerSpeed
  nop 6
applyPlayerSpeedReturn:

playerSpeedX:
  dd (float)1.0

playerVerticalX:
  dd (float)1.0
  
identityValue:
  dd (float)1.0


// Initiates the Predator system.
define(omnifyPredatorHook,"witcher3.exe"+60E5C0)

assert(omnifyPredatorHook,8B 02 89 41 70)
alloc(initiatePredator,$1000,omnifyPredatorHook)

registersymbol(omnifyPredatorHook)

initiatePredator:
  pushf 
  // An initialized playerLocation pointer is required prior to Predator execution.
  push rax
  mov rax,playerLocation
  cmp rax,0
  pop rax
  je initiatePredatorOriginalCode
  // We'll need a few SSE registers in order to do double->float conversion.
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  // We'll need to backup the registers used by Predator to store its return values.
  push rax
  push rbx
  push rcx
  // And another register to hold onto the current coordinates, since we'll need them to finish 
  // calculating what the new coordinates should be following Predator execution.
  push rsi
  mov rsi,rcx
  mov rax,playerLocation
  mov rbx,[rax]
  // It is possible during a load screen following a death or area transition for a previously
  // initialized player location pointer to no longer be pointing to a valid place in memory. 
  // We'll need to wait in that case. Eventually, our player hook will correct this.
  lea rcx,[rbx+1B8]
  call checkBadPointer
  cmp ecx,0
  jne initiatePredatorCleanup
  // Our player's coordinates are stored as doubles. Predator expects them to be floats.
  // A packed conversion to handle both X and Y at once...
  cvtpd2ps xmm0,[rbx+1B8]
  // And then a single conversion to take care of Z.
  cvtsd2ss xmm1,[rbx+1C8]
initiatePredatorExecute:
  // With our player's coordinates converted, we'll push them as the first parameter to the stack.
  // X and Y are pushed as quadwords first, followed by Z, as if they were pushed as two m64
  // addresses.
  sub rsp,8
  movq [rsp],xmm0
  sub rsp,8
  movq [rsp],xmm1
  // Next are the current coordinates for the target.
  push [rsi+70]  
  push [rsi+78]  
  // An identity matrix is passed to represent the target's dimensional scales. True scaling is
  // most likely not present in this game; the "artificial" size of the creature shouldn't 
  // impact movement.
  movss xmm0,[identityValue]  
  shufps xmm0,xmm0,0
  sub rsp,10  
  movdqu [rsp],xmm0  
  // Time to produce the movement offsets, which act as the final parameter for the Predator system.
  // Offsets are calculated as such: coordinatesNew - coordinatesCurrent = offsets.
  movups xmm0,[rdx]
  movups xmm1,[rsi+70]  
  subps xmm0,xmm1
  // The offsets are on xmm0, they are pushed to the stack as if we were pushing two m64 addresses 
  // in memory.
  movhlps xmm1,xmm0
  sub rsp,8
  movq [rsp],xmm0
  sub rsp,8
  movq [rsp],xmm1
  // All parameters have been provided. Execute the Predator!
  call executePredator
initializePredatorUpdateCoordinates:
  // We make some room on the stack to help transfer updated offsets found in eax, ebx, and ecx
  // to an SSE register.
  sub rsp,10
  mov [rsp],eax
  mov [rsp+4],ebx
  mov [rsp+8],ecx
  movups xmm0,[rsp]
  add rsp,10
  // The updated offsets are then added to the current coordinates, giving us new target coordinates.
  movups xmm1,[rsi+70]
  addps xmm1,xmm0
  movups [rdx],xmm1
initiatePredatorCleanup:
  pop rsi
  pop rcx
  pop rbx
  pop rax
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
initiatePredatorOriginalCode:
  popf
  mov eax,[rdx]
  mov [rcx+70],eax
  jmp initiatePredatorReturn

omnifyPredatorHook:
  jmp initiatePredator  
initiatePredatorReturn:

threatDistance:
  dd (float)2.0

aggroDistance:
  dd (float)6.0

skipBoostY:
  dd 0

skipBoostZ:
  dd 1


// Initiates the Abomnification system.
// The function we're hooking into appears to be responsbile for resetting properties on A
// CRenderEntityGroup. It runs much less frequently than the actual rendering functions, so
// it is appropriate to use for Abomnified scale generation progression.
// [r14-50]: CRenderEntityGroup being updated.
define(omnifyAbomnificationHook,"witcher3.exe"+C1A3D7)

assert(omnifyAbomnificationHook,41 C7 06 FF FF 7F 7F)
alloc(initiateAbomnification,$1000,omnifyAbomnificationHook)

registersymbol(omnifyAbomnificationHook)

initiateAbomnification:
  pushf
  // We don't apply the Abomnified scales here, we just generate them for later retrieval.
  // Regardless, we need to backup the registers the Abomnification system uses to hold return values.
  push rax
  push rbx
  push rcx
  // The game maintains a CRenderEntityGroup for all renderings associated with a particular NPC.
  // This makes it a very suitable identifier for the Abomnification system.
  lea rax,[r14-50]
  // Push the identifying address parameter and let's start up the Abomnification!
  push rax
  call executeAbomnification
  pop rcx
  pop rbx
  pop rax
initiateAbomnificationOriginalCode:
  popf
  mov [r14],7F7FFFFF
  jmp initiateAbomnificationReturn

omnifyAbomnificationHook:
  jmp initiateAbomnification
  nop 2
initiateAbomnificationReturn:

// Value change to actual height change is not 1:1, ~1.25x increase in value causes actual height to be ~2x.
abominifyHeightResultUpper:
  dd #160

abominifyHeightResultLower:
  dd #10

abominifyWidthResultUpper:
  dd #400

abominifyDepthResultUpper:
  dd #400

unnaturalSmallX:
  dd (float)1.4

unnaturalBigX:
  dd (float)0.6


// Applies Abomnification generated scale multipliers.
// rbx/rcx: The rendering unit, typically CRenderProxy_Mesh.
define(omnifyApplyAbomnificationHook,"witcher3.exe"+CEEFC0)

assert(omnifyApplyAbomnificationHook,41 8B 00 89 41 40)
alloc(applyAbomnification,$1000,omnifyApplyAbomnificationHook)
alloc(headHeightCoefficient,8)
alloc(headHeightShifter,8)
alloc(morphEverything,8)

registersymbol(omnifyApplyAbomnificationHook)
registersymbol(morphEverything)

applyAbomnification:
  pushf
  // We'll need to back up two SSE registers: one for holding the scale multiplier, the other for the particular dimension
  // we're modifying.
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  // Backing up the registers used by the Abomnification system to hold return values.
  push rax
  push rbx
  push rcx
  // And an additional one to support dereferencing pointers, as rax and rcx are needed by us for 2-byte value checking and
  // pointer checking respectively.
  push rdx
  // rsi will hold the identifying address for the creature morphing -- in this case, its CRenderEntityGroup.
  push rsi
  // Our primary goal is the association of entity data with what's being rendered.
  // The majority of CRenderProxy_Mesh (or like) instances will point to the CRenderEntityGroup that they belong to here.  
  mov rsi,[rbx+140]
  lea rcx,[rsi]
  call checkBadPointer
  cmp rcx,0 
  je ensureGroup
  // Some CRenderProxy_Mesh like instances will point to a group here instead.
  mov rsi,[rbx+108]
  lea rcx,[rsi]
  call checkBadPointer
  cmp rcx,0
  jne applyAbomnificationExit
ensureGroup:
  // Ensure that this actually points to a CRenderEntityGroup.
  mov rax,[rsi]
  cmp ax,0xBFE8
  jne applyAbomnificationExit  
  cmp [morphEverything],1
  je allowMorphing
  // It seems that this will be set to 0x8 or less for all monster types. We only want to morph enemy monsters.
  mov rax,[rsi+8]
  cmp rax,0x9
  jge applyAbomnificationExit
  jmp allowMorphing  
allowMorphing:
  // Push the identifying address parameter and get the Abomnified scales.
  push rsi
  call getAbomnifiedScales    
  // Load the Abomnified width.
  movd xmm0,eax
  // Now to apply the Abomnified width. Dimensions in this game are a bit complicated -- we aren't dealing with the
  // the typical 3x4 matrix. The r8 register points to the model data we need to manipulate.
  // [r8]: Width for the body when the character is facing north or south (yes, if character is facing east/west this has 0 
  //       effect on the character's model. It has nothing to do with camera position. First time I've seen this kind of thing.
  //       Hell though, ya'll know I'm not a game dev!)
  // [r8+4]: Width for the body when character is facing east or west.
  // [r8+20]: Width for the head when character is facing north or south.
  // [r8+24]: Width for the head when character is facing east or west.
  movss xmm1,[r8]
  mulss xmm1,xmm0
  movss [r8],xmm1
  movss xmm1,[r8+4]
  mulss xmm1,xmm0
  movss [r8+4],xmm1
  movss xmm1,[r8+20]
  mulss xmm1,xmm0
  movss [r8+20],xmm1
  movss xmm1,[r8+24]
  mulss xmm1,xmm0
  movss [r8+24],xmm1
  // Load the Abomnified height.
  movd xmm0,ebx
  // The height dimension has its own complications.
  // [r8+8]: Height and vertical offset for the head. In order to keep in sync with the rest of the body's height, testing has shown
  //         that the value going here needs to be plugged into a function. By charting values required in order to keep the head lined up
  //         with the body, I derived a mathematical function that will more or less keep it in sync.
  //         Note that the position of the head cannot be lowered without shrinking the head, and indeed, in order to continue keeping the
  //         head lined up once the body height value shrinks below 0.8 requires a negative head value to be applied. That means, yes,
  //         short bodies will have upside down heads. It's brilliant!
  //      
  //         heightHead = heightBody*3.2 - 2.12
  //         Note that most monsters seem to not have a separate head height value.
  //
  // [r8+18]: Height of equipment on back.
  //
  // [r8+28]: Height for the body. Changes in this value vs changes in actual height on screen is not 1:1. 1.2x here increases height by ~2x
  //          actually, etc.
  mulss xmm0,[headHeightCoefficient]  
  subss xmm0,[headHeightShifter]
  movss xmm1,[r8+8]
  mulss xmm1,xmm0  
  movss [r8+8],xmm1
  // Reset to Abomnified height for body and equipment height.
  movd xmm0,ebx  
  movss xmm1,[r8+28]
  mulss xmm1,xmm0  
  movss [r8+28],xmm1
  movss xmm1,[r8+18]
  mulss xmm1,xmm0  
  movss [r8+18],xmm1
  // Load the Abomnified depth.
  movd xmm0,ecx
  // For the depth:
  // [r8+10]: Depth for body and head when character is facing east or west.
  // [r8+14]: Depth for body and head when character is facing north or south.
  movss xmm1,[r8+10]
  mulss xmm1,xmm0
  movss [r8+10],xmm1
  movss xmm1,[r8+14]
  mulss xmm1,xmm0
  movss [r8+14],xmm1
  // That's all she wrote.
applyAbomnificationExit:  
  pop rsi
  pop rdx
  pop rcx
  pop rbx
  pop rax
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
applyAbomnificationOriginalCode:
  popf
  mov eax,[r8]
  mov [rcx+40],eax
  jmp applyAbomnificationReturn

omnifyApplyAbomnificationHook:
  jmp applyAbomnification
  nop 
applyAbomnificationReturn:


headHeightCoefficient:
  dd (float)3.2

headHeightShifter:
  dd (float)2.12

morphEverything:
  dd 0