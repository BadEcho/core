// Omnified Framework Assembly Functions v. 0.5
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC

// Global memory.
alloc(zero,8)
alloc(epsilon,8)
alloc(damageThreshold,8)
alloc(yIsVertical,8)

registersymbol(epsilon)
registersymbol(yIsVertical)

zero:
  dd 0

epsilon:
  dd (float)0.001

damageThreshold:
  dd (float)3.9

yIsVertical:
  dd 1

  
// Checks if the loaded address is a valid pointer.
// rcx: The address to check.
alloc(checkBadPointer,$1000)

registersymbol(checkBadPointer)

checkBadPointer:
  push rcx
  shr rcx,20
  cmp rcx,0x7FFF  
  pop rcx
  jg checkBadPointerExit
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  sub rsp,10
  movdqu [rsp],xmm2
  sub rsp,10
  movdqu [rsp],xmm3
  sub rsp,10
  movdqu [rsp],xmm4
  sub rsp,10
  movdqu [rsp],xmm5
  push rax
  push rbx
  push rdx
  push rsi
  push rdi
  push r8
  push r9
  push r10
  push r11
  push r12
  push r13
  push r14
  push r15
  push rbp
  mov rbp,rsp
  and spl,F0
  sub rsp,20
  mov edx,4
  call isBadReadPtr
  mov rcx,eax
  mov rsp,rbp
  pop rbp
  pop r15
  pop r14
  pop r13
  pop r12
  pop r11
  pop r10
  pop r9
  pop r8
  pop rdi
  pop rsi
  pop rdx
  pop rbx
  pop rax
  movdqu xmm5,[rsp]
  add rsp,10
  movdqu xmm4,[rsp]
  add rsp,10
  movdqu xmm3,[rsp]
  add rsp,10
  movdqu xmm2,[rsp]
  add rsp,10
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
checkBadPointerExit:
  ret
  

// Random number generation function.
// After r12-r14 pushes:
// [rsp+20]: initialization state address, 0 if first time
// [rsp+28]: Upper bounds
// [rsp+30]: Lower bounds
// Return value is in EAX
alloc(generateRandomNumber,$1000)

registersymbol(generateRandomNumber)

generateRandomNumber:
  push r12
  push r13
  push r14
  mov r12,[rsp+20]
  mov r13,[rsp+28]
  mov r14,[rsp+30]
  push rbx
  push rcx
  push rdx
  push r8
  push r10
  push r11
  cmp [r12],0
  jne getRandomNumber
initializeSeed:
  call kernel32.GetTickCount
  push eax
  call msvcrt.srand
  pop eax
  mov [r12],1
getRandomNumber:
  call msvcrt.rand
  xor edx,edx
  mov ebx,r14
  mov ecx,r13
  cmp ecx,ebx
  cmovl ecx,ebx
  inc ecx
  sub ecx,ebx
  idiv ecx
  add edx,ebx
  mov eax,edx
  pop r11
  pop r10
  pop r8
  pop rdx
  pop rcx
  pop rbx
  pop r14
  pop r13
  pop r12
  ret 18


// Mark and recall symbols.
alloc(teleport,8)
alloc(teleportX,8)
alloc(teleportY,8)
alloc(teleportZ,8)

registersymbol(teleport)
registersymbol(teleportX)
registersymbol(teleportY)
registersymbol(teleportZ)

// Global Apocalypse memory.
alloc(playerDamageX,8)

registersymbol(playerDamageX)

playerDamageX:
  dd (float)1.0
  

// Player Apocalypse System Function
// [rsp+48]: Player's coordinates (aligned at x-coordinate)
// [rsp+50]: Max player health amount
// [rsp+58]: Player's health amount
// [rsp+60]: Damage amount
// Updated damage is in EAX. 
// Updated health before damage is in EBX.
alloc(executePlayerApocalypse,$1000)
alloc(playerApocalypseRandomState,8)
alloc(logApocalypse,8)
alloc(negativeOne,8)
alloc(apocalypseResult,8)
alloc(apocalypseResultUpper,8)
alloc(apocalypseResultLower,8)
alloc(extraDamageSafetyThreshold,8)
alloc(extraDamageResidualHealth,8)
alloc(teleported,8)
alloc(teleportedX,8)
alloc(teleportedY,8)
alloc(teleportedZ,8)
alloc(teleportitisResult,8)
alloc(teleportitisResultUpper,8)
alloc(teleportitisResultLower,8)
alloc(teleportitisDivisor,8)
alloc(teleportitisShifter,8)
alloc(lastVerticalDisplacement,8)
alloc(negativeVerticalDisplacementEnabled,8)
alloc(teleportitisDisplacementX,8)
alloc(coordinatesAreDoubles,8)
alloc(riskOfMurderResult,8)
alloc(riskOfMurderResultUpper,8)
alloc(riskOfMurderResultLower,8)
alloc(fatalisResult,8)
alloc(fatalisResultUpper,8)
alloc(fatalisResultLower,8)
// fatalisState: 0 = not active; 1 = active; 2 = cured (used for announcement, then set to 0)
alloc(fatalisState,8)
alloc(fatalisBloodlustDamageX,8)
alloc(basePlayerDamageX,8)
alloc(extraDamageX,8)
alloc(sixtyNineDamageX,8)
alloc(maxDamageToPlayer,8)
alloc(lastDamageToPlayer,8)
alloc(totalDamageToPlayer,8)
alloc(playerGodMode,8)
alloc(disableTeleportitis,8)
alloc(disableSixtyNine,8)
alloc(sixtyNineEveryTime,8)

registersymbol(executePlayerApocalypse)
registersymbol(logApocalypse)
registersymbol(extraDamageSafetyThreshold)
registersymbol(teleported)
registersymbol(teleportedX)
registersymbol(teleportedY)
registersymbol(teleportedZ)
registersymbol(apocalypseResult)
registersymbol(negativeVerticalDisplacementEnabled)
registersymbol(teleportitisDisplacementX)
registersymbol(riskOfMurderResult)
registersymbol(fatalisResult)
registersymbol(fatalisResultUpper)
registersymbol(fatalisState)
registersymbol(fatalisBloodlustDamageX)
registersymbol(basePlayerDamageX)
registersymbol(extraDamageX)
registersymbol(maxDamageToPlayer)
registersymbol(lastDamageToPlayer)
registersymbol(totalDamageToPlayer)
registersymbol(lastVerticalDisplacement)
registersymbol(coordinatesAreDoubles)
registersymbol(playerGodMode)
registersymbol(disableTeleportitis)
registersymbol(disableSixtyNine)
registersymbol(sixtyNineEveryTime)

executePlayerApocalypse:
  // Backing up a few SSE registers we'll be using to
  // hold the parameters provided to this function.
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  sub rsp,10
  movdqu [rsp],xmm2
  sub rsp,10
  movdqu [rsp],xmm3  
  // Load the player's health parameter.
  movss xmm3,[rsp+58]
  // Load the damage amount parameter.
  movss xmm0,[rsp+60]
  // Check if the damage being done is enough to warrant Apocalypse execution.
  mov rax,damageThreshold
  ucomiss xmm0,[rax]  
  jbe exitPlayerApocalypse
  // If God Mode is disabled, then we apply the Apocalypse.
  cmp [playerGodMode],1
  jne applyApocalypse
  // Otherwise, we zero out our final damage amount register and exit.
  xorps xmm0,xmm0
  jmp exitPlayerApocalypse  
applyApocalypse:
  // If "Sixty Nine Every Time!" mode is enabled, force a sixty nine effect.
  cmp [sixtyNineEveryTime],1
  jne checkFatalis
  mov [apocalypseResult],8
  mov [riskOfMurderResult],4
  jmp sixtyNine
checkFatalis:
  // If the player has the Fatalis debuff, all damage is fatal.
  cmp [fatalisState],1
  jne applyApocalypseRoll
  // To make good on the Fatalis debuff, we set the damage equal to the health.
  movss xmm0,xmm3
  jmp updateEnemyDamageStats
applyApocalypseRoll:
  // Load the parameters for generating the dice roll random number.
  push [apocalypseResultLower]
  push [apocalypseResultUpper]
  mov rax,playerApocalypseRandomState
  push rax
  call generateRandomNumber
  // Our random roll value is in eax -- we back it up to the "apocalypseResult"
  // symbol so that the value can be displayed by the event logging display code.
  mov [apocalypseResult],eax
  cmp eax,4
  jle extraDamage
  cmp eax,6
  jle teleportitis
  cmp eax,9
  jle riskOfMurder
  jmp suddenGasm
extraDamage:
  // Load the player's maximum health, check if current health is below that multiplied by
  // the safety threshold.
  movss xmm1,[rsp+50]
  mulss xmm1,[extraDamageSafetyThreshold]
  ucomiss xmm3,xmm1
  jb applyExtraDamage  
  // Check if the damage would normally kill us, even without the extra damage effect.  
  movss xmm1,xmm3
  subss xmm1,xmm0
  movss xmm2,[epsilon]
  ucomiss xmm1,xmm2
  jb applyExtraDamage
  // Normal damage wouldn't kill us, check if extra damage would, and then prevent it from doing so.
  movss xmm1,xmm3
  movss xmm2,xmm0
  mulss xmm2,[extraDamageX]
  subss xmm1,xmm2
  movss xmm2,[epsilon]
  ucomiss xmm1,xmm2
  ja applyExtraDamage
  // Set damage to be same as current health minus our "left over" health so that we end up with
  // this health amount following application of damage to our health.
  movss xmm0,xmm3
  subss xmm0,[extraDamageResidualHealth]
  jmp updateEnemyDamageStats
applyExtraDamage:
  mulss xmm0,[extraDamageX]
  jmp updateEnemyDamageStats
teleportitis:
  // Check if teleportitis is disabled. If so, we instead apply extra damage.
  cmp [disableTeleportitis],1
  jne commitTeleportitis
  mov [apocalypseResult],1
  jmp extraDamage
commitTeleportitis:
  // Some games will disable modifications being made to the player's coordinates
  // during certain animations such as weapon attacks or getting knocked back.
  // This code needs to be hooked into and temporarily disabled in order for 
  // teleportitis to work. This can be done by checking the "teleported" symbol 
  // and preventing those coordinates from being reset. That code will then 
  // need to set this symbol to 0. In most games I've hacked, this is not 
  // required. In fact, only one: Dark Souls I.
  mov [teleported],1
  // Load the player coordinates address parameter.
  mov rbx,[rsp+48]
  // Load the parameters for generating the random displacement value to be 
  // applied to the x-coordinate.
  push [teleportitisResultLower]
  push [teleportitisResultUpper]
  mov rax,playerApocalypseRandomState
  push rax
  call generateRandomNumber
  // The random number is an integer and will need to be converted to a float.
  mov [teleportitisResult],eax
  cvtsi2ss xmm1,[teleportitisResult]
  // The random number is divided by the following divisor to bring it into the
  // expected range (0-10) along with some decimal precision (3 decimal places).
  divss xmm1,[teleportitisDivisor]
  // We cannot generate random negative integers, we instead shift what we have 
  // here by a negative amount. The range 0 to 10 becomes -5 to 5.
  subss xmm1,[teleportitisShifter]
  // We finally take what we have and then multiply it by the displacement 
  // multiplier to get the final displacement value to apply to the player's 
  // x-coordinate.
  mulss xmm1,[teleportitisDisplacementX]
  // We then take the player's current x-coordinate from memory and add the
  // displacement value to it.
  cmp [coordinatesAreDoubles],1
  je loadXAsDouble
  movss xmm2,[rbx]
  jmp addChangeToX
loadXAsDouble:
  cvtsd2ss xmm2,[rbx]
addChangeToX:
  addss xmm2,xmm1
  // The updated x-coordinate is committed back into the memory, which will 
  // move the player.
  movss [teleportedX],xmm2
  cmp [coordinatesAreDoubles],1
  je commitXAsDouble
  movss [rbx],xmm2
  jmp teleportitisY
commitXAsDouble:
  cvtss2sd xmm1,xmm2
  movsd [rbx],xmm1
teleportitisY:
  // Load the parameters for generating the random displacement value to be 
  // applied to the y-coordinate. 
  push [teleportitisResultLower]
  push [teleportitisResultUpper]
  mov rax,playerApocalypseRandomState
  push rax
  call generateRandomNumber
  mov [teleportitisResult],eax
  cvtsi2ss xmm1,[teleportitisResult]
  divss xmm1,[teleportitisDivisor]
  // If the Y-axis is not the vertical axis, then we we don't need to check 
  // whether vertical displacement is enabled.
  cmp [yIsVertical],1
  jne skipYSkipCheck
  // If negative vertical displacement is not enabled we do not want to shift it,
  // this causes the random value to remain in the range of 0 to 10.
  cmp [negativeVerticalDisplacementEnabled],1
  jne skipNegativeVerticalYDisplacement
skipYSkipCheck:
  subss xmm1,[teleportitisShifter]
skipNegativeVerticalYDisplacement:
  mulss xmm1,[teleportitisDisplacementX]
  // The vertical displacement value is logged and displayed to viewers as 
  // changes to are often the most consequential. We make sure the Y-axis is 
  // the vertical one before logging it.
  cmp [yIsVertical],1
  jne skipLastYVerticalDisplacement
  movss [lastVerticalDisplacement],xmm1
skipLastYVerticalDisplacement:
  // We then take the player's current y-coordinate from memory and add the
  // displacement value to it.
  cmp [coordinatesAreDoubles],1
  je loadYAsDouble
  movss xmm2,[rbx+4]
  jmp addChangeToY
loadYAsDouble:
  cvtsd2ss xmm2,[rbx+8]
addChangeToY:
  addss xmm2,xmm1
  // The updated y-coordinate is commited back into the memory, which will 
  // move the player.
  movss [teleportedY],xmm2
  cmp [coordinatesAreDoubles],1
  je commitYAsDouble
  movss [rbx+4],xmm2
  jmp teleportitisZ
commitYAsDouble:
  cvtss2sd xmm1,xmm2  
  movsd [rbx+8],xmm1
teleportitisZ:
  // Load the parameters for generating the random displacement value to be 
  // applied to the z-coordinate. 
  push [teleportitisResultLower]
  push [teleportitisResultUpper]
  mov rax,playerApocalypseRandomState
  push rax
  call generateRandomNumber
  mov [teleportitisResult],eax
  cvtsi2ss xmm1,[teleportitisResult]
  divss xmm1,[teleportitisDivisor]
  // Like the y-axis, the z-axis can sometimes be the vertical axis. So checks 
  // similar to the ones made in the y-coordinate displacement code are made.
  cmp [yIsVertical],0
  jne skipZSkipCheck
  cmp [negativeVerticalDisplacementEnabled],1
  jne skipNegativeVerticalZDisplacement
skipZSkipCheck:
  subss xmm1,[teleportitisShifter]
skipNegativeVerticalZDisplacement:
  mulss xmm1,[teleportitisDisplacementX]
  cmp [yIsVertical],0
  jne skipLastZVerticalDisplacement
  movss [lastVerticalDisplacement],xmm1
skipLastZVerticalDisplacement:
  // We then take the player's current z-coordinate from memory and add the
  // displacement value to it.
  cmp [coordinatesAreDoubles],1
  je loadZAsDouble
  movss xmm2,[rbx+8]  
  jmp addChangeToZ
loadZAsDouble:
  cvtsd2ss xmm2,[rbx+10]
addChangeToZ:
  addss xmm2,xmm1
  // The updated z-coordinate is commited back into the memory, which will 
  // move the player.
  movss [teleportedZ],xmm2
  cmp [coordinatesAreDoubles],1
  je commitZAsDouble
  movss [rbx+8],xmm2
  jmp updateEnemyDamageStats
commitZAsDouble:
  cvtss2sd xmm1,xmm2
  movsd [rbx+10],xmm1  
  jmp updateEnemyDamageStats
riskOfMurder:
  // Load the parameters for generating the Risk of Murder dice roll random 
  // number.
  push [riskOfMurderResultLower]
  push [riskOfMurderResultUpper]
  mov rax,playerApocalypseRandomState
  push rax
  call generateRandomNumber
  // Our Risk of Murder roll is in eax -- we back it up to the "riskOfMurderResult" 
  // symbol so that the value can be displayed by the event logging display code.
  mov [riskOfMurderResult],eax
  cmp eax,3
  // If the resulting roll is 4 or 5, then the player is getting sixty nined.
  jg sixtyNine
  // Otherwise, normal damage applies, however there is also now a very slight chance
  // of Fatalis being applied, but only if it is not already active.  
  cmp [fatalisState],1
  je updateEnemyDamageStats
  push [fatalisResultLower]
  push [fatalisResultUpper]
  mov rax,playerApocalypseRandomState
  push rax
  call generateRandomNumber
  // The Fatalis roll is in eax -- we throw it into the "fatalisResult" symbol
  // so we can report on it.
  mov [fatalisResult],eax
  // Fatalis will only be applied if the roll landed on the maximum possible value.
  cmp eax,[fatalisResultUpper]
  jne updateEnemyDamageStats
  mov [fatalisState],1
  // The player, although made fragile by the scourge of Fatalis, enters a rage that increases their damage.
  // Store current player damage for later restoration, and then increase it by 1.25x.
  movss xmm1,[playerDamageX]
  movss [basePlayerDamageX],xmm1
  mulss xmm1,[fatalisBloodlustDamageX]
  movss [playerDamageX],xmm1
  jmp updateEnemyDamageStats  
sixtyNine:
  // Check if sixty nine is disabled, if so, just apply normal damage.
  cmp [disableSixtyNine],1
  jne commitSixtyNine
  mov [riskOfMurderResult],1
  jmp updateEnemyDamageStats
commitSixtyNine:
  mulss xmm0,[sixtyNineDamageX]
  jmp updateEnemyDamageStats
suddenGasm:  
  // Load the player's maximum health parameter. This is stored in the final 
  // player health (prior to damage applied) register.
  movss xmm3,[rsp+50]
  // We zero out our final damage amount register.
  xorps xmm0,xmm0
  jmp applyPlayerApocalypseExit
updateEnemyDamageStats:
  // If the final damage amount is less than or equal to the current max damage 
  // to the player, it doesn't need to be updated obviously!
  ucomiss xmm0,[maxDamageToPlayer]
  jna skipMaxEnemyDamageUpdate
  movss [maxDamageToPlayer],xmm0
skipMaxEnemyDamageUpdate:
  // We save the final damage amount as the last damage to be done to the player,
  // and add it to the running total.
  movss [lastDamageToPlayer],xmm0
  movss xmm1,xmm0
  addss xmm1,[totalDamageToPlayer]
  movss [totalDamageToPlayer],xmm1
applyPlayerApocalypseExit:
  // Because Apocalypse execution is complete, we trigger an event log entry for 
  // it by setting "logApocalypse" to 1.
  mov [logApocalypse],1
  jmp exitPlayerApocalypse
exitPlayerApocalypse:
  // We commit our final damage amount to eax.
  movd eax,xmm0
  // We commit our final working player health to ebx.
  movd ebx,xmm3
  // Restore backed up values.
  movdqu xmm3,[rsp]
  add rsp,10
  movdqu xmm2,[rsp]
  add rsp,10
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10  
  // This function has 4 parameters, each require 8 bytes. 4x8 == 20 (hex).
  ret 20
  
playerApocalypseRandomState:
  dd 0
  
logApocalypse:
  dd 0
  
apocalypseResult:
  dd 0
  
apocalypseResultUpper:
  dd #10
  
apocalypseResultLower:
  dd 1

extraDamageResidualHealth:
  dd (float)69.0

extraDamageSafetyThreshold:
  dd (float)0.9
  
teleportitisResult:
  dd 0
  
teleportitisResultUpper:
  dd #10000
  
teleportitisResultLower:
  dd 0
  
teleportitisDivisor:
  dd (float)1000.0
  
teleportitisShifter:
  dd (float)5.0

teleported:
  dd 0

teleportedX:
  dd (float)0.0

teleportedY:
  dd (float)0.0

teleportedZ:
  dd (float)0.0

negativeVerticalDisplacementEnabled:
  dd 1  

coordinatesAreDoubles:
  dd 0
  
teleportitisDisplacementX:
  dd (float)1.0
    
negativeOne:
  dd (float)-1.0

riskOfMurderResult:
  dd 0
  
riskOfMurderResultUpper:
  dd #5
  
riskOfMurderResultLower:
  dd 1
  
fatalisResult:
  dd 0
  
fatalisResultUpper:
  dd #7
  
fatalisResultLower:
  dd 1
  
fatalisState:
  dd 0

fatalisBloodlustDamageX:
  dd (float)1.25
  
extraDamageX:
  dd (float)2.0
  
sixtyNineDamageX:
  dd (float)69.0
  
maxDamageToPlayer:
  dd 0
  
lastDamageToPlayer:
  dd 0
  
totalDamageToPlayer:
  dd 0
  
playerGodMode:
  dd 0  
  
disableTeleportitis:
  dd 0
  
disableSixtyNine:
  dd 0

sixtyNineEveryTime:
  dd 0

  
// Enemy Apocalypse System Function
// [rsp+38]: Target health value
// [rsp+40]: Damage amount
alloc(executeEnemyApocalypse,$1000)
alloc(maxDamageByPlayer,8)
alloc(lastDamageByPlayer,8)
alloc(totalDamageByPlayer,8)
alloc(logKamehameha,8)
alloc(gokuResult,8)
alloc(gokuResultUpper,8)
alloc(gokuResultLower,8)
alloc(gokuDamageX,8)
alloc(lastEnemyHealthValue,8)
alloc(playerCritChanceResultUpper,8)
alloc(playerCritChanceResultLower,8)
alloc(playerCritChanceResult,8)
alloc(playerCritDamageResultUpper,8)
alloc(playerCritDamageResultLower,8)
alloc(playerCritDamageResult,8)
alloc(playerCritDamageDivisor,8)
alloc(logPlayerCrit,8)
alloc(enemyApocalypseRandomState,8)

registersymbol(executeEnemyApocalypse)
registersymbol(maxDamageByPlayer)
registersymbol(lastDamageByPlayer)
registersymbol(totalDamageByPlayer)
registersymbol(logKamehameha)
registersymbol(gokuDamageX)
registersymbol(gokuResultUpper)
registersymbol(lastEnemyHealthValue)
registersymbol(playerCritDamageResultUpper)
registersymbol(playerCritDamageResultLower)
registersymbol(playerCritDamageResult)
registersymbol(logPlayerCrit)

executeEnemyApocalypse:
  // Backing up a few SSE registers we'll be using to
  // hold the parameters provided to this function.
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1  
  sub rsp,10
  movdqu [rsp],xmm2
  // Load the enemy's health parameter.  
  movss xmm1,[rsp+38]
  // Load the damage amount parameter.
  movss xmm0,[rsp+40]
  // Check if the damage being done is enough to warrant Apocalypse execution.
  mov rax,damageThreshold
  ucomiss xmm0,[rax]  
  jbe exitEnemyApocalypse  
applyPlayerDamage:
  // Apply our base player damage multiplier to the damage amount.
  mulss xmm0,[playerDamageX]
  // Load the parameters for generating the critical hit check.
  push [playerCritChanceResultLower]
  push [playerCritChanceResultUpper]
  mov rax,enemyApocalypseRandomState
  push rax
  call generateRandomNumber
  // Our random roll value is in eax -- we back it up to the 
  // "playerCritChanceResult" symbol so that the value can be displayed by the 
  // event logging display code.
  mov [playerCritChanceResult],eax
  // We can't generate random floats, only random integers. So, to have 
  // a 6.25% crit chance, we generate a number in the range of 0 to 400, and 
  // then check if the value is less than or equal to 25 (25/400 = 0.0625).
  cmp eax,25
  jg checkKamehameha  
  // Load the parameters for generating the critical hit damage.
  push [playerCritDamageResultLower]
  push [playerCritDamageResultUpper]
  mov rax,enemyApocalypseRandomState
  push rax
  call generateRandomNumber
  // Our random roll value is in eax -- we back it up to the 
  // "playerCritDamageResult" symbol so that the value can be displayed by 
  // the event logging display code.
  mov [playerCritDamageResult],eax
  // We can't generate floating point random numbers, so we convert it to float
  // and divide it by our divisor to put it in range with one level of decimal
  // precision.
  cvtsi2ss xmm2,[playerCritDamageResult]
  divss xmm2,[playerCritDamageDivisor]
  mulss xmm0,xmm2
  // We signal to the event logging system that a crit occurred by setting
  // the "logPlayerCrit" symbol to 1.
  mov [logPlayerCrit],1  
checkKamehameha:
  // Load the parameters for generating the Kamehameha check.
  push [gokuResultLower]
  push [gokuResultUpper]
  mov rax,enemyApocalypseRandomState
  push rax
  call generateRandomNumber
  // Our random roll value is in eax -- we back it up to the "gokuResult"
  // symbol so that the value can be displayed by the event logging display code.
  mov [gokuResult],eax
  // If the roll is exactly 69, a Kamehameha has occurred! Big damage time baby.
  cmp eax,#69
  jne updatePlayerDamageStats
  // We signal to the event logging system that a Kamehameha occurred by setting
  // the "logKamehameha" symbol to 1.
  mov [logKamehameha],1
  mulss xmm0,[gokuDamageX]  
updatePlayerDamageStats:
  // Sometimes the enemy health is hidden from the player by the game. Well I like
  // flexing my muscle and displaying it anyway on stream with the 
  // "lastEnemyHealthValue" symbol. We perform a mock damage application here and 
  // store it there.
  subss xmm1,xmm0
  movss [lastEnemyHealthValue],xmm1
  // If the final damage amount is less than or equal to the current max damage 
  // from the player, it doesn't need to be updated obviously!
  ucomiss xmm0,[maxDamageByPlayer]
  jna skipMaxPlayerDamageUpdate
  movss [maxDamageByPlayer],xmm0
skipMaxPlayerDamageUpdate:
  // We save the final damage amount as the last damage to be done from the 
  // player, and add it to the running total.
  movss [lastDamageByPlayer],xmm0
  movss xmm1,xmm0
  addss xmm1,[totalDamageByPlayer]
  movss [totalDamageByPlayer],xmm1
exitEnemyApocalypse:
  // We commit our final damage amount to eax.
  movd eax,xmm0
  // We commit our final damage amount to ebx, which actually never changes. It
  // is simply used to calculate what the health will be to display with the
  // "lastEnemyHealthValue" stat, and also for purposes of polymorphism, in a 
  // manner of speaking.
  movss xmm1,[rsp+38]
  movd ebx,xmm1
  movdqu xmm2,[rsp]
  add rsp,10
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
  // This function has 2 parameters, each require 8 bytes. 2x8 == 10 (hex).
  ret 10

 
totalDamageByPlayer:
  dd 0

maxDamageByPlayer:
  dd 0
  
lastDamageByPlayer:
  dd 0  

logKamehameha:
  dd 0

gokuResult:
  dd 0
  
gokuResultUpper:
  dd #1000
  
gokuResultLower:
  dd 0
  
gokuDamageX:
  dd (float)10000.0  
  
lastEnemyHealthValue:
  dd 0
  
playerCritChanceResult:
  dd 0
  
playerCritChanceResultUpper:
  dd #800
  
playerCritChanceResultLower:
  dd 0
  
playerCritDamageResult:
  dd 0
  
playerCritDamageResultUpper:
  dd #50
  
playerCritDamageResultLower:
  dd #20
  
playerCritDamageDivisor:
  dd (float)10.0
  
logPlayerCrit:
  dd 0
  
enemyApocalypseRandomState:
  dd 0
  
// Predator System Functions
alloc(enemySpeedX,8)
alloc(aggroDistance,8)
alloc(threatDistance,8)

registersymbol(enemySpeedX)
registersymbol(threatDistance)
registersymbol(aggroDistance)

enemySpeedX:
  dd (float)1.25

aggroDistance:
  dd (float)10.0

threatDistance:
  dd (float)2.5
  

// Determines the distance between the player and another creature.
// This function's parameters are most easily loaded onto the stack
// from an SSE register: the player's coordinates first, and then the enemy's.
// [rsp+28]: Enemy z-coordinate value
// [rsp+30]: Enemy x-coordinate value
// [rsp+34]: Enemy y-coordinate value
// [rsp+38]: Player z-coordinate value
// [rsp+40]: Player x-coordinate value
// [rsp+44]: Player y-coordinate value
// Distance is in EAX
alloc(findCoordinateDistance,$1000)

registersymbol(findCoordinateDistance)

findCoordinateDistance:
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  // Load enemy's coordinates into xmm0, and player's into xmm1.
  movups xmm0,[rsp+28]
  movups xmm1,[rsp+38]
  // Take the difference between the two sets of coordinates.
  subps xmm0,xmm1
  // Square the differences.
  mulps xmm0,xmm0
  // Then we proceed to add the various squared differences to each other.
  movdqu xmm1,xmm0
  // Shuffle the y-coordinate results to the lowest doubleword for adding to the 
  // running total.
  shufps xmm1,xmm0,0xB
  addss xmm0,xmm1
  // Shuffle the x-coordinate results to the lowest doubleword for adding to the
  // running total.
  shufps xmm1,xmm1,0x1
  addss xmm0,xmm1
  // Take the square root of everything, and we have our distance!
  sqrtss xmm0,xmm0
  movd eax,xmm0
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
  ret 20


// Calculates the scaled base speed.
// [rsp+28]: Enemy depth scale
// [rsp+30]: Enemy width scale
// [rsp+34]: Enemy height scale
// Scaled speed is in EAX.
alloc(calculateScaledSpeed,$1000)
alloc(averageScaleDivisor,8)

registersymbol(calculateScaledSpeed)

calculateScaledSpeed:
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  // Find average of the three scales.
  movss xmm0,[rsp+28]
  movss xmm1,[rsp+30]
  addss xmm0,xmm1
  movss xmm1,[rsp+34]
  addss xmm0,xmm1
  divss xmm0,[averageScaleDivisor]
  // Load the base enemy speed boost and divide it by the average scale.
  movss xmm1,[enemySpeedX]
  divss xmm1,xmm0
  movd eax,xmm1  
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
  ret 10


averageScaleDivisor:
  dd (float)3.0


// Determines if enemy is moving towards the player.
// [rsp+38]: Change to enemy's z-coordinate value
// [rsp+40]: Change to enemy's x-coordinate value
// [rsp+44]: Change to enemy's y-coordinate value
// [rsp+48]: Enemy's z-coordinate value
// [rsp+50]: Enemy's x-coordinate value
// [rsp+54]: Enemy's y-coordinate value
// [rsp+58]: Player's z-coordinate value
// [rsp+60]: Player's x-coordinate value
// [rsp+64]: Player's y-coordinate value
// EAX is 1 if enemy is moving towards player, otherwise 0.
alloc(isMovingTowards,$1000)

registersymbol(isMovingTowards)

isMovingTowards:
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  sub rsp,10
  movdqu [rsp],xmm2
  // Load player's coordinates into xmm0, and enemy's into xmm1.
  movups xmm0,[rsp+58]
  movups xmm1,[rsp+48]
  // We take the difference between each set to find distances on each axis.
  subps xmm0,xmm1
  // We square the results so that we aren't bothered by any notion of signs.
  mulps xmm0,xmm0
  // Contents of xmm0 register: (zp-ze)^2 | 0 | (xp-xe)^2 | (yp-ye)^2
  movups xmm1,xmm0
  shufps xmm1,xmm0,0x87
  // Contents of xmm1 register: (yp-ye)^2 | 0 | (zp-ze)^2 | (xp-xe)^2 
  cmpps xmm0,xmm1,6
  // This compare instruction will create a bitmask telling us: 
  // -If the z-coordinate distance is greater than the y-coordinate distance
  // -If the x-coordinate distance is greater than the z-coordinate distance
  // -If the y-coordinate distance is greater than the x-coordinate distance 
  sub rsp,10
  movups [rsp],xmm0  
isYGreaterThanX:
  mov eax,[rsp+C]
  test eax,eax
  jne isZGreaterThanY
  je isXGreaterThanZ
isZGreaterThanY:
  mov eax,[rsp]
  test eax,eax
  jne isZMovingTowards
  je isYMovingTowards
isXGreaterThanZ:
  mov eax,[rsp+8]
  test eax,eax
  jne isXMovingTowards
  je isZMovingTowards
isYMovingTowards:
  add rsp,10
  xor rax,rax
  movups xmm0,[rsp+38]
  mulps xmm0,xmm0
  sub rsp,10
  movups [rsp],xmm0
  movss xmm0,[rsp+C]
  // Is the change in Y greater than the change in X?
  ucomiss xmm0,[rsp+8]
  jb primaryChangeTooSmall
  // Is the change in Y greater than the change in Z?
  ucomiss xmm0,[rsp]
  jb primaryChangeTooSmall  
  add rsp,10
  movss xmm0,[rsp+64]
  subss xmm0,[rsp+54]
  movss xmm1,[rsp+44]
  jmp isChangeMovingTowards
isXMovingTowards:
  add rsp,10
  xor rax,rax
  movups xmm0,[rsp+38]
  mulps xmm0,xmm0
  sub rsp,10
  movups [rsp],xmm0
  movss xmm0,[rsp+8]
  // Is the change in X greater than the change in Y?
  ucomiss xmm0,[rsp+C]
  jb primaryChangeTooSmall
  // Is the change in X greater than the change in Z?
  ucomiss xmm0,[rsp]
  jb primaryChangeTooSmall
  add rsp,10
  movss xmm0,[rsp+60]
  subss xmm0,[rsp+50]
  movss xmm1,[rsp+40]
  jmp isChangeMovingTowards
isZMovingTowards:
  add rsp,10
  xor rax,rax
  movups xmm0,[rsp+38]
  mulps xmm0,xmm0
  sub rsp,10
  movups [rsp],xmm0
  movss xmm0,[rsp]
  // Is the change in Z greater than the change in Y?
  ucomiss xmm0,[rsp+C]
  jb primaryChangeTooSmall
  // Is the change in Z greater than the change in X?
  ucomiss xmm0,[rsp+8]
  jb primaryChangeTooSmall
  add rsp,10
  movss xmm0,[rsp+58]
  subss xmm0,[rsp+48]
  movss xmm1,[rsp+38]
isChangeMovingTowards:
  movd eax,xmm0
  shr eax,1F
  test eax,eax
  jne isChangeNegative
  movd eax,xmm1
  shr eax,1F
  test eax,eax
  je confirmMovingTowards
  xor rax,rax
  jmp isMovingTowardsExit
isChangeNegative:
  movd eax,xmm1
  shr eax,1F
  test eax,eax
  jne confirmMovingTowards
  xor rax,rax
  jmp isMovingTowardsExit
confirmMovingTowards:
  mov eax,1
  jmp isMovingTowardsExit
primaryChangeTooSmall:
  add rsp,10
isMovingTowardsExit:
  movdqu xmm2,[rsp]
  add rsp,10
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
  ret 30


// Main Predator system function.
// [rsp+98]: Change to enemy's z-coordinate value
// [rsp+A0]: Change to enemy's x-coordinate value
// [rsp+A4]: Change to enemy's y-coordinate value
// [rsp+A8]: Enemy depth scale
// [rsp+B0]: Enemy width scale
// [rsp+B4]: Enemy height scale
// [rsp+B8]: Enemy's z-coordinate value
// [rsp+C0]: Enemy's x-coordinate value
// [rsp+C4]: Enemy's y-coordinate value
// [rsp+C8]: Player's z-coordinate value
// [rsp+D0]: Player's x-coordinate value
// [rsp+D4]: Player's y-coordinate value
// EAX has updated change to enemy's x-coordinate value
// EBX has updated change to enemy's y-coordinate value
// ECX has updated change to enemy's z-coordinate value 
alloc(executePredator,$1000)
alloc(indifferenceDistanceX,8)
alloc(defaultSpeedX,8)
alloc(areaBoostX,8)
alloc(aggressionSpeedX,8)
alloc(positiveLimit,8)
alloc(negativeLimit,8)
alloc(positiveLimitCorrection,8)
alloc(negativeLimitCorrection,8)

registersymbol(executePredator)
registersymbol(positiveLimit)
registersymbol(negativeLimit)
registersymbol(defaultSpeedX)
registersymbol(aggressionSpeedX)
registersymbol(positiveLimitCorrection)
registersymbol(negativeLimitCorrection)

executePredator:
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  sub rsp,10
  movdqu [rsp],xmm2
  sub rsp,10
  movdqu [rsp],xmm3
  sub rsp,10
  movdqu [rsp],xmm4
  sub rsp,10
  movdqu [rsp],xmm5
  sub rsp,10
  movdqu [rsp],xmm6
  sub rsp,10
  movdqu [rsp],xmm7
  sub rsp,10
  movdqu [rsp],xmm8
  xorps xmm8,xmm8
  // xmm8 will hold the speed buff multiplier to be applied.
  movss xmm8,[defaultSpeedX]
  // Load the player's coordinates into xmm0.
  movups xmm0,[rsp+C8]
  // Load the enemy's coordinates into xmm1.
  movups xmm1,[rsp+B8]
  // Load the enemy's scales into xmm2.
  movups xmm2,[rsp+A8]
  // Load the movement offsets into xmm3.
  movups xmm3,[rsp+98]
  // Submit the scales as a parameter for calculating the scaled speed.
  sub rsp,10
  movdqu [rsp],xmm2
  call calculateScaledSpeed
  // The scaled base speed boost will be held in xmm4 for the remainder
  // of this function's execution.
  movd xmm4,eax
  // Next we submit the player and enemy coordinates are parameters for
  // finding the distance between them.
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  call findCoordinateDistance
  // The distance between enemy and player will be held in xmm5 for the
  // remainder of this function's execution.
  movd xmm5,eax
  // Is the enemy within the area of aggro?
  ucomiss xmm5,[aggroDistance]
  jbe areaOfAggro
  // If not, are we between the area of indifference and aggro?
  movss xmm7,[aggroDistance]
  mulss xmm7,[indifferenceDistanceX]
  ucomiss xmm5,xmm7
  jb areaOfSketchiness
areaOfIndifference:
  // If not, we're in the area of indifference. The scaled base speed boost
  // becomes the effective speed boost.
  movss xmm8,xmm4
  jmp executePredatorExit
areaOfSketchiness:
  // In the area of sketchiness, the scaled base speed boost is the effective
  // speed boost, unless the enemy is moving towards the player.
  movss xmm8,xmm4
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  sub rsp,10
  movdqu [rsp],xmm3
  call isMovingTowards
  cmp eax,1
  jne executePredatorExit  
  mulss xmm8,[areaBoostX]
  mulss xmm8,[aggressionSpeedX]
  jmp executePredatorExit
areaOfAggro:
  // If the enemy is in the area of threat, there is no speed boost applied.
  ucomiss xmm5,[threatDistance]
  jbe executePredatorExit
  // Otherwise, in the area of aggro, we either double the boost observed within
  // the area of sketchiness with the enemy moving towards the player, or we strip
  // all speed boosts if the enemy isn't moving towards the player.
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  sub rsp,10
  movdqu [rsp],xmm3
  call isMovingTowards
  cmp eax,0
  // If the enemy isn't moving towards the player, remember that we still have defaultSpeedX
  // loaded into xmm8, so exiting out now will result in no speed buff.
  je executePredatorExit
  movss xmm8,xmm4  
  mulss xmm8,[areaBoostX]
  mulss xmm8,[areaBoostX]
  mulss xmm8,[aggressionSpeedX]
executePredatorExit:
  // No speed buff is applied to the axis set as the vertical axis.
  cmp [yIsVertical],0
  je commitZChange
  mulss xmm3,xmm8
commitZChange:
  movd ecx,xmm3
  shufps xmm3,xmm3,0x87
  cmp [yIsVertical],1
  je commitYChange
  mulss xmm3,xmm8
commitYChange:
  movd ebx,xmm3
  shufps xmm3,xmm3,0x87
  mulss xmm3,xmm8
  movd eax,xmm3
  // We now test whether x, y, or z offsets violate designated speed limits.
  push rax
  shr eax,1F
  test eax,eax
  pop rax
  jne isXLessThanNegativeLimit
  movd xmm2,eax
  ucomiss xmm2,[positiveLimit]
  jbe isYPastLimit
  mov eax,[positiveLimitCorrection]
  jmp isYPastLimit
isXLessThanNegativeLimit:
  movd xmm2,eax
  ucomiss xmm2,[negativeLimit]
  ja isYPastLimit
  mov eax,[negativeLimitCorrection]
isYPastLimit:
  cmp [yIsVertical],1
  je isZPastLimit
  push rbx
  shr ebx,1F  
  test ebx,ebx
  pop rbx
  jne isYLessThanNegativeLimit
  movd xmm2,ebx
  ucomiss xmm2,[positiveLimit]
  jbe isZPastLimit
  mov ebx,[positiveLimitCorrection]
  jmp isZPastLimit
isYLessThanNegativeLimit:
  movd xmm2,ebx
  ucomiss xmm2,[negativeLimit]
  ja isZPastLimit
  mov ebx,[negativeLimitCorrection]
isZPastLimit:
  cmp [yIsVertical],0
  je executePredatorCleanup
  push rcx
  shr ecx,1F
  test ecx,ecx
  pop rcx
  jne isZLessThanNegativeLimit
  movd xmm2,ecx
  ucomiss xmm2,[positiveLimit]
  jbe executePredatorCleanup
  mov ecx,[positiveLimitCorrection]
  jmp executePredatorCleanup
isZLessThanNegativeLimit:
  movd xmm2,ecx
  ucomiss xmm2,[negativeLimit]
  ja executePredatorCleanup
  mov ecx,[negativeLimitCorrection]
executePredatorCleanup:
  movdqu xmm8,[rsp]
  add rsp,10
  movdqu xmm7,[rsp]
  add rsp,10
  movdqu xmm6,[rsp]
  add rsp,10
  movdqu xmm5,[rsp]
  add rsp,10
  movdqu xmm4,[rsp]
  add rsp,10
  movdqu xmm3,[rsp]
  add rsp,10
  movdqu xmm2,[rsp]
  add rsp,10
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
  ret 40

defaultSpeedX:
  dd (float)1.0

indifferenceDistanceX:
  dd (float)2.0

areaBoostX:
  dd (float)2.0

positiveLimit:
  dd (float)15.0

negativeLimit:
  dd (float)-15.0

positiveLimitCorrection:
  dd (float)0

negativeLimitCorrection:
  dd (float)0
  
aggressionSpeedX:
  dd (float)1.0
  
  
// Abomnification Scale Data
alloc(morphScaleData,$33FFCC)
alloc(morphScaleIndex,8)
  
registersymbol(morphScaleData)
registersymbol(morphScaleIndex)

morphScaleIndex:
  dd 0    

// Abomnification System Function
// [rsp+10]: The identifying address.
// Return Values
// eax: Updated width scale
// ebx: Updated height scale
// ecx: Updated depth scale
alloc(executeAbomnification,$1000)
alloc(abominifyRandomState,8)
alloc(abominifyMorphStepsResultUpper,8)
alloc(abominifyMorphStepsResultLower,8)
alloc(abominifyHeightResultUpper,8)
alloc(abominifyHeightResultLower,8)
alloc(abominifyWidthResultUpper,8)
alloc(abominifyWidthResultLower,8)
alloc(abominifyDepthResultUpper,8)
alloc(abominifyDepthResultLower,8)
alloc(abominifyMorphModeResultUpper,8)
alloc(abominifyMorphModeResultLower,8)
alloc(unnaturalBigThreshold,8)
alloc(unnaturalBigX,8)
alloc(unnaturalSmallX,8)
alloc(abominifyDivisor,8)
alloc(defaultScaleX,8)
alloc(speedMorph,8)
alloc(speedMorphDivisor,8)
alloc(stopMorphs,8)
alloc(zeroValue,8)
alloc(forceScrub,8)
alloc(overrideMorphSteps,8)
alloc(overrideMorphStepsValue,8)
alloc(disableAbomnification,8)

registersymbol(executeAbomnification)
registersymbol(abominifyMorphStepsResultUpper)
registersymbol(abominifyMorphStepsResultLower)
registersymbol(abominifyHeightResultUpper)
registersymbol(abominifyHeightResultLower)
registersymbol(abominifyWidthResultUpper)
registersymbol(abominifyWidthResultLower)
registersymbol(abominifyDepthResultUpper)
registersymbol(abominifyDepthResultLower)
registersymbol(unnaturalBigThreshold)
registersymbol(unnaturalBigX)
registersymbol(speedMorphDivisor)
registersymbol(unnaturalSmallX)
registersymbol(speedMorph)
registersymbol(stopMorphs)
registersymbol(forceScrub)
registersymbol(overrideMorphSteps)
registersymbol(overrideMorphStepsValue)
registersymbol(disableAbomnification)

executeAbomnification:
  push rdx  
  cmp [disableAbomnification],1
  jne continueAbomnification
  mov eax,0x80000000
  jmp executeAbomnificationCleanup
continueAbomnification:
  mov rbx,[rsp+10]
  cmp [forceScrub],1
  je scrubMorphData
  //cmp [rbx],0
  cmp rbx,0
  jne applyMorphScaleFromData  
checkMorphDataIndexLimit:
  cmp [morphScaleIndex],#999
  jne incrementMorphDataIndex
scrubMorphData:
  mov [forceScrub],0
  mov [morphScaleIndex],0
  push rax
  mov rax,0
nextMorphDataScrub:
  add rax,#52
  cmp rax,0x33FFCC
  je exitMorphDataScrub
  mov rdx,morphScaleData
  add rdx,rax
  mov [rdx],0
  mov [rdx+4],0
  mov [rdx+8],0
  mov [rdx+C],0
  mov [rdx+10],0
  mov [rdx+14],0
  mov [rdx+18],0
  mov [rdx+1C],0
  mov [rdx+20],0
  mov [rdx+24],0
  mov [rdx+28],0
  mov [rdx+2C],0
  mov [rdx+30],0
  jmp nextMorphDataScrub
exitMorphDataScrub:
  pop rax
incrementMorphDataIndex:
  inc [morphScaleIndex]
  mov rcx,[morphScaleIndex]
  mov [rbx],ecx
applyMorphScaleFromData:  
  mov rdx,morphScaleData
  push rax
  push rbx
  mov rax,#52
  movzx ecx,bx
  imul eax,ecx
  mov rcx,rax
  pop rbx
  pop rax  
  add rdx,rcx  
  cmp [rdx],0
  ja continuePhase
  // Current phase is located in [rdx+30].
  // 0 == Pause phase. Switch to step morph phase.
  // 1 == Step morph phase. Switch to pause phase.
  cmp [rdx+30],1
  je changeToPausePhase
  mov [rdx+30],1
  jmp initializeMorphSteps
changeToPausePhase:
  mov [rdx+30],0
  jmp initializeMorphSteps
continuePhase:
  cmp [stopMorphs],1
  je executeAbomnificationCleanup
  dec [rdx]
  jmp executeMorphPhase
initializeMorphSteps:
  sub rsp,10
  movdqu [rsp],xmm0
  movss xmm0,[rdx+4]
  push rax
  mov rax,zeroValue
  ucomiss xmm0,[rax]
  pop rax
  ja skipInitializeDefaultScales
  movss xmm0,[defaultScaleX]
  movss [rdx+4],xmm0
  movss [rdx+8],xmm0
  movss [rdx+C],xmm0
skipInitializeDefaultScales:
  movss [rdx+14],xmm0
  movss xmm0,[rdx+8]
  movss [rdx+18],xmm0
  movss xmm0,[rdx+C]
  movss [rdx+1C],xmm0
  push rax
  cmp [rdx+10],1
  je initializeMorphStepsExit
  movss xmm0,[rdx+20]
  mov rax,zeroValue
  ucomiss xmm0,[rax]
  ja skipGenerateMorphSteps
  push [abominifyMorphStepsResultLower]
  push [abominifyMorphStepsResultUpper]
  mov rax,abominifyRandomState
  push rax
  call generateRandomNumber
  cvtsi2ss xmm0,eax
  movss [rdx+20],xmm0
skipGenerateMorphSteps:
  cmp [overrideMorphSteps],1
  jne loadGeneratedMorphSteps
  movss xmm0,[overrideMorphStepsValue]
  jmp processMorphSteps
loadGeneratedMorphSteps:
  movss xmm0,[rdx+20]
processMorphSteps:
  cmp [speedMorph],1
  jne generateMonsterMorphTargets
  divss xmm0,[speedMorphDivisor]  
generateMonsterMorphTargets:  
  cvtss2si eax,xmm0
  mov [rdx],eax
  push [abominifyHeightResultLower]
  push [abominifyHeightResultUpper]
  mov rax,abominifyRandomState
  push rax
  call generateRandomNumber
  cvtsi2ss xmm0,eax
  divss xmm0,[abominifyDivisor]
  movss [rdx+28],xmm0
  cmp [rdx+10],0 
  jne skipGenerateMorphMode
  push [abominifyMorphModeResultLower]
  push [abominifyMorphModeResultUpper]
  mov rax,abominifyRandomState
  push rax
  call generateRandomNumber
  jmp applyMorphMode
skipGenerateMorphMode:
  mov eax,[rdx+10]
applyMorphMode:
  cmp eax,1
  je staticUnnaturalMorphing
  cmp eax,5
  jle uniformMorphing
  jmp nonUniformMorphing
staticUnnaturalMorphing:
  mov [rdx+10],1
  ucomiss xmm0,[unnaturalBigThreshold]
  ja unnaturalBiggify
  mulss xmm0,[unnaturalSmallX]
  jmp unnaturalizeIt
unnaturalBiggify:
  mulss xmm0,[unnaturalBigX] 
unnaturalizeIt:
  movss [rdx+4],xmm0
  movss [rdx+8],xmm0
  movss [rdx+C],xmm0
  jmp initializeMorphStepsExit
uniformMorphing:
  mov [rdx+10],5
  movss [rdx+24],xmm0
  movss [rdx+2C],xmm0
  jmp initializeMorphStepsExit
nonUniformMorphing:
  mov [rdx+10],8
  push [abominifyWidthResultLower]
  push [abominifyWidthResultUpper]
  mov rax,abominifyRandomState
  push rax
  call generateRandomNumber
  cvtsi2ss xmm0,eax
  divss xmm0,[abominifyDivisor]
  movss [rdx+24],xmm0
  push [abominifyDepthResultLower]
  push [abominifyDepthResultUpper]
  mov rax,abominifyRandomState
  push rax
  call generateRandomNumber
  cvtsi2ss xmm0,eax
  divss xmm0,[abominifyDivisor]
  movss [rdx+2C],xmm0
initializeMorphStepsExit:
  pop rax
  movss xmm0,[rdx+4]
  movd eax,xmm0
  movss xmm0,[rdx+8]
  movd ebx,xmm0
  movss xmm0,[rdx+C]
  movd ecx,xmm0
  movdqu xmm0,[rsp]
  add rsp,10
  jmp executeAbomnificationCleanup
executeMorphPhase:
  // Current phase is located in [rdx+30].
  // 0 == Pause phase. Do nothing!
  // 1 == Step morph phase. Continue morphing!
  cmp [rdx+30],1
  je stepMorph
  jmp executeAbomnificationCleanup
stepMorph:
  cmp [rdx+10],1  
  je executeAbomnificationCleanup
  sub rsp,10
  movdqu [rsp],xmm0
  sub rsp,10
  movdqu [rsp],xmm1
  push rsi
  movss xmm0,[rdx+20]
  cmp [speedMorph],1
  jne generateMorphsForStep
  divss xmm0,[speedMorphDivisor]  
generateMorphsForStep:
  cvtss2si eax,xmm0
  mov esi,[rdx]
  sub eax,esi
  cvtsi2ss xmm1,eax
  divss xmm1,xmm0 //[rdx+20]
  // Width Step
  movss xmm0,[rdx+24]
  subss xmm0,[rdx+14]
  mulss xmm0,xmm1
  addss xmm0,[rdx+14]
  movss [rdx+4],xmm0
  movd eax,xmm0
  // Height Step
  movss xmm0,[rdx+28]
  subss xmm0,[rdx+18]
  mulss xmm0,xmm1
  addss xmm0,[rdx+18]
  movss [rdx+8],xmm0
  movd ebx,xmm0
  // Depth Step
  movss xmm0,[rdx+2C]
  subss xmm0,[rdx+1C]
  mulss xmm0,xmm1
  addss xmm0,[rdx+1C]
  movss [rdx+C],xmm0
  movd ecx,xmm0
  pop rsi
  movdqu xmm1,[rsp]
  add rsp,10
  movdqu xmm0,[rsp]
  add rsp,10
executeAbomnificationCleanup:  
  pop rdx
  ret 8  
  
abominifyRandomState:
  dd 0

abominifyMorphStepsResultUpper:
  dd #400

abominifyMorphStepsResultLower:
  dd #25

abominifyHeightResultUpper:
  dd #215

abominifyHeightResultLower:
  dd #25

abominifyWidthResultUpper:
  dd #275

abominifyWidthResultLower:
  dd #25

abominifyDepthResultUpper:
  dd #300

abominifyDepthResultLower:
  dd #25

abominifyMorphModeResultUpper:
  dd #13

abominifyMorphModeResultLower:
  dd 1

abominifyDivisor:
  dd (float)100.0

unnaturalBigThreshold:
  dd (float)1.0

unnaturalSmallX:
  dd (float)0.3

unnaturalBigX:
  dd (float)1.9

stopMorphs:
  dd 0
  
speedMorph:
  dd 0
  
speedMorphDivisor:
  dd (float)4.0

defaultScaleX:
  dd (float)1.0

zeroValue:
  dd 0	
  
forceScrub:
  dd 0
  
overrideMorphSteps:
  dd 0
  
overrideMorphStepsValue:
  dd (float)400.0
  
disableAbomnification:
  dd 0
  
  
// Retrieves the Abomnified scale multipliers for the specified
// morph scale ID.
// [rsp+10]: The identifying address.
alloc(getAbomnifiedScales,$1000)
alloc(defaultScaleX,8)

registersymbol(getAbomnifiedScales)
registersymbol(defaultScaleX)

getAbomnifiedScales:
  push rdx
  cmp [disableAbomnification],1
  je getAbomnifiedScalesDefault
  mov rcx,[rsp+10]
  mov rax,#52
  movzx rdx,cx
  imul eax,edx
  mov rdx,morphScaleData
  add rdx,rax
  mov eax,[rdx+4]
  mov ebx,[rdx+8]
  mov ecx,[rdx+C]
  // Check if we are getting empty data for return values. 
  // If so, we'll return default values instead.
  cmp eax,0
  je getAbomnifiedScalesDefault
  jmp getAbomnifiedScalesCleanup  
getAbomnifiedScalesDefault:
  mov eax,[defaultScaleX]
  mov ebx,[defaultScaleX]
  mov ecx,[defaultScaleX]
getAbomnifiedScalesCleanup:
  pop rdx
  ret 8
  
  
defaultScaleX:
  dd (float)1.0


[DISABLE]

// Cleanup of global memory
unregistersymbol(epsilon)
unregistersymbol(yIsVertical)

dealloc(zero)
dealloc(epsilon)
dealloc(damageThreshold)
dealloc(yIsVertical)

// Cleanup of checkBadPointer
unregistersymbol(checkBadPointer)

dealloc(checkBadPointer)

// Cleanup of generateRandomNumber
unregistersymbol(generateRandomNumber)

dealloc(generateRandomNumber)

// Cleanup of Mark and Recall symbols
unregistersymbol(teleport)
unregistersymbol(teleportX)
unregistersymbol(teleportY)
unregistersymbol(teleportZ)

dealloc(teleport)
dealloc(teleportX)
dealloc(teleportY)
dealloc(teleportZ)

// Cleanup of global Apocalypse memory
dealloc(playerDamageX)

unregistersymbol(playerDamageX)

// Cleanup of Player Apocalypse System Function
unregistersymbol(logApocalypse)
unregistersymbol(extraDamageSafetyThreshold)
unregistersymbol(teleported)
unregistersymbol(teleportedX)
unregistersymbol(teleportedY)
unregistersymbol(teleportedZ)
unregistersymbol(apocalypseResult)
unregistersymbol(riskOfMurderResult)
unregistersymbol(fatalisResult)
unregistersymbol(fatalisResultUpper)
unregistersymbol(fatalisState)
unregistersymbol(fatalisBloodlustDamageX)
unregistersymbol(basePlayerDamageX)
unregistersymbol(lastVerticalDisplacement)
unregistersymbol(coordinatesAreDoubles)
unregistersymbol(negativeVerticalDisplacementEnabled)
unregistersymbol(teleportitisDisplacementX)
unregistersymbol(extraDamageX)
unregistersymbol(maxDamageToPlayer)
unregistersymbol(playerGodMode)
unregistersymbol(lastDamageToPlayer)
unregistersymbol(totalDamageToPlayer)
unregistersymbol(executePlayerApocalypse)
unregistersymbol(disableTeleportitis)
unregistersymbol(disableSixtyNine)
unregistersymbol(sixtyNineEveryTime)

dealloc(playerApocalypseRandomState)
dealloc(logApocalypse)
dealloc(teleported)
dealloc(teleportedX)
dealloc(teleportedY)
dealloc(telerpotedZ)
dealloc(apocalypseResult)
dealloc(apocalypseResultUpper)
dealloc(apocalypseResultLower)
dealloc(extraDamageResidualHealth)
dealloc(extraDamageSafetyThreshold)
dealloc(negativeOne)
dealloc(teleportitisResult)
dealloc(teleportitisResultUpper)
dealloc(teleportitisResultLower)
dealloc(teleportitisDivisor)
dealloc(teleportitisShifter)
dealloc(lastVerticalDisplacement)
dealloc(coordinatesAreDoubles)
dealloc(negativeVerticalDisplacementEnabled)
dealloc(teleportitisDisplacementX)
dealloc(riskOfMurderResult)
dealloc(riskOfMurderResultUpper)
dealloc(riskOfMurderResultLower)
dealloc(basePlayerDamageX)
dealloc(fatalisBloodlustDamageX)
dealloc(fatalisResult)
dealloc(fatalisResultUpper)
dealloc(fatalisResultLower)
dealloc(fatalisState)
dealloc(extraDamageX)
dealloc(sixtyNineDamageX)
dealloc(maxDamageToPlayer)
dealloc(lastDamageToPlayer)
dealloc(totalDamageToPlayer)
dealloc(disableTeleportitis)
dealloc(disableSixtyNine)
dealloc(sixtyNineEveryTime)
dealloc(executePlayerApocalypse)
dealloc(playerGodMode)

// Cleanup of Enemy Apocalypse System Function
unregistersymbol(maxDamageByPlayer)
unregistersymbol(lastDamageByPlayer)
unregistersymbol(totalDamageByPlayer)
unregistersymbol(logKamehameha)
unregistersymbol(gokuDamageX)
unregistersymbol(gokuResultUpper)
unregistersymbol(lastEnemyHealthValue)
unregistersymbol(playerCritDamageResult)
unregistersymbol(playerCritDamageResultLower)
unregistersymbol(playerCritDamageResultUpper)
unregistersymbol(logPlayerCrit)
unregistersymbol(executeEnemyApocalypse)

dealloc(maxDamageByPlayer)
dealloc(lastDamageByPlayer)
dealloc(totalDamageByPlayer)
dealloc(logKamehameha)
dealloc(gokuResult)
dealloc(gokuResultUpper)
dealloc(gokuResultLower)
dealloc(gokuDamageX)
dealloc(lastEnemyHealthValue)
dealloc(playerCritChanceResultUpper)
dealloc(playerCritChanceResultLower)
dealloc(playerCritChanceResult)
dealloc(playerCritDamageResultUpper)
dealloc(playerCritDamageResultLower)
dealloc(playerCritDamageResult)
dealloc(playerCritDamageDivisor)
dealloc(logPlayerCrit)
dealloc(enemyApocalypseRandomState)
dealloc(executeEnemyApocalypse)

// Cleanup of findCoordinateDistance
unregistersymbol(findCoordinateDistance)

dealloc(findCoordinateDistance)

// Cleanup of calculateScaledSpeed
unregistersymbol(calculateScaledSpeed)

dealloc(averageScaleDivisor)
dealloc(calculateScaledSpeed)

// Cleanup of isMovingTowards
unregistersymbol(isMovingTowards)

dealloc(isMovingTowards)


// Cleanup of Predator System Function
unregistersymbol(executePredator)
unregistersymbol(enemySpeedX)
unregistersymbol(threatDistance)
unregistersymbol(aggroDistance)
unregistersymbol(positiveLimit)
unregistersymbol(negativeLimit)
unregistersymbol(positiveLimitCorrection)
unregistersymbol(negativeLimitCorrection)
unregistersymbol(defaultSpeedX)
unregistersymbol(aggressionSpeedX)

dealloc(enemySpeedX)
dealloc(aggroDistance)
dealloc(threatDistance)
dealloc(indifferenceDistanceX)
dealloc(defaultSpeedX)
dealloc(aggressionSpeedX)
dealloc(areaBoostX)
dealloc(positiveLimit)
dealloc(negativeLimit)
dealloc(positiveLimitCorrection)
dealloc(negativeLimitCorrection)
dealloc(executePredator)

// Cleanup of Abomnification Scale Data
unregistersymbol(morphScaleData)
unregistersymbol(morphScaleIndex)

dealloc(morphScaleIndex)
dealloc(morphScaleData)

// Cleanup of Abomnification System Function
unregistersymbol(executeAbomnification)
unregistersymbol(abominifyMorphStepsResultUpper)
unregistersymbol(abominifyMorphStepsResultLower)
unregistersymbol(abominifyHeightResultUpper)
unregistersymbol(abominifyHeightResultLower)
unregistersymbol(abominifyWidthResultUpper)
unregistersymbol(abominifyWidthResultLower)
unregistersymbol(abominifyDepthResultUpper)
unregistersymbol(abominifyDepthResultLower)
unregistersymbol(unnaturalBigThreshold)
unregistersymbol(unnaturalBigX)
unregistersymbol(unnaturalSmallX)
unregistersymbol(speedMorphDivisor)
unregistersymbol(speedMorph)
unregistersymbol(stopMorphs)
unregistersymbol(forceScrub)
unregistersymbol(overrideMorphSteps)
unregistersymbol(overrideMorphStepsValue)
unregistersymbol(disableAbomnification)

dealloc(stopMorphs)
dealloc(speedMorph)
dealloc(speedMorphDivisor)
dealloc(defaultScaleX)
dealloc(abominifyRandomState)
dealloc(abominifyMorphStepsResultUpper)
dealloc(abominifyMorphStepsResultLower)
dealloc(abominifyHeightResultUpper)
dealloc(abominifyHeightResultLower)
dealloc(abominifyWidthResultUpper)
dealloc(abominifyWidthResultLower)
dealloc(abominifyDepthResultUpper)
dealloc(abominifyDepthResultLower)
dealloc(abominifyMorphModeResultUpper)
dealloc(abominifyMorphModeResultLower)
dealloc(unnaturalBigThreshold)
dealloc(unnaturalBigX)
dealloc(unnaturalSmallX)
dealloc(abominifyDivisor)
dealloc(zeroValue)
dealloc(forceScrub)
dealloc(overrideMorphSteps)
dealloc(overrideMorphStepsValue)
dealloc(disableAbomnification)
dealloc(executeAbomnification)

// Cleanup of getAbomnifiedScales
unregistersymbol(getAbomnifiedScales)
unregistersymbol(defaultScaleX)

dealloc(defaultScaleX)
dealloc(getAbomnifiedScales)