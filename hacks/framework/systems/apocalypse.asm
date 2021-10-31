//----------------------------------------------------------------------
// The Apocalypse System
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC
// 
// Bad Echo Technologies are licensed under a
// Creative Commons Attribution-NonCommercial 4.0 International License.
//
// See accompanying file LICENSE.md or a copy at:
// http://creativecommons.org/licenses/by-nc/4.0/
//----------------------------------------------------------------------

// Global Apocalypse memory.
alloc(playerDamageX,8)
alloc(playerGodMode,8)

registersymbol(playerDamageX)
registersymbol(playerGodMode)

playerDamageX:
    dd (float)1.0

playerGodMode:
    dd 0  

// Player Apocalypse System Function
// [rsp+48]: Player's coordinates (aligned at x-coordinate)
// [rsp+50]: Max player health amount
// [rsp+58]: Player's health amount
// [rsp+60]: Damage amount
// Updated damage is in EAX. 
// Updated health before damage is in EBX.
alloc(executePlayerApocalypse,$1000)
alloc(logPlayerApocalypse,8)
alloc(apocalypseDieRoll,8)
alloc(apocalypseDieRollUpper,8)
alloc(apocalypseDieRollLower,8)
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
alloc(lastXDisplacement,8)
alloc(lastYDisplacement,8)
alloc(lastZDisplacement,8)
alloc(lastVerticalDisplacement,8)
alloc(negativeVerticalDisplacementEnabled,8)
alloc(teleportitisDisplacementX,8)
alloc(verticalTeleportitisDisplacementX,8)
alloc(coordinatesAreDoubles,8)
alloc(murderRoll,8)
alloc(murderRollUpper,8)
alloc(murderRollLower,8)
alloc(fatalisResult,8)
alloc(fatalisResultUpper,8)
alloc(fatalisResultLower,8)
alloc(fatalisState,8)
alloc(fatalisBloodlustDamageX,8)
alloc(fatalisHealthLost,8)
alloc(fatalisDeaths,8)
alloc(fatalisMinutesAfflicted,8)
alloc(basePlayerDamageX,8)
alloc(extraDamageX,8)
alloc(murderDamageX,8)
alloc(orgasmHealthHealed,8)
alloc(maxDamageToPlayer,8)
alloc(lastDamageToPlayer,8)
alloc(totalDamageToPlayer,8)
alloc(disableTeleportitis,8)
alloc(disableMurder,8)
alloc(murderIsAfoot,8)

registersymbol(executePlayerApocalypse)
registersymbol(logPlayerApocalypse)
registersymbol(extraDamageSafetyThreshold)
registersymbol(teleported)
registersymbol(teleportedX)
registersymbol(teleportedY)
registersymbol(teleportedZ)
registersymbol(apocalypseDieRoll)
registersymbol(negativeVerticalDisplacementEnabled)
registersymbol(teleportitisDisplacementX)
registersymbol(verticalTeleportitisDisplacementX)
registersymbol(murderRoll)
registersymbol(fatalisResult)
registersymbol(fatalisResultUpper)
registersymbol(fatalisState)
registersymbol(fatalisBloodlustDamageX)
registersymbol(fatalisHealthLost)
registersymbol(fatalisDeaths)
registersymbol(fatalisMinutesAfflicted)
registersymbol(basePlayerDamageX)
registersymbol(extraDamageX)
registersymbol(orgasmHealthHealed)
registersymbol(murderDamageX)
registersymbol(maxDamageToPlayer)
registersymbol(lastDamageToPlayer)
registersymbol(totalDamageToPlayer)
registersymbol(lastZDisplacement)
registersymbol(lastYDisplacement)
registersymbol(lastXDisplacement)
registersymbol(lastVerticalDisplacement)
registersymbol(coordinatesAreDoubles)
registersymbol(disableTeleportitis)
registersymbol(disableMurder)
registersymbol(murderIsAfoot)

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
    // If "Murder Is Afoot!!" mode is enabled, force a murder effect.
    cmp [murderIsAfoot],1
    jne checkFatalis
    mov [apocalypseDieRoll],8
    mov [murderRoll],4
    jmp murder
checkFatalis:
    // Fatalis
    //
    // If the player has the Fatalis debuff, all damage is fatal. Exposure to Fatalis is indicated by the 'fatalisState' 
    // being 1, while full-on affliction is indicated by a state of 2. Having a state of exposure allows the messaging
    // system the ability to detect newly acquired Fatalis statuses, and to be able to report on these events appropriately. 
    //
    // The messaging system is then responsible for setting 'fatalisState' to 2, indicating that it is now active. 
    // Now, that being said: if the player is being hit at a very, very high frequency, it is possible that another 
    // hit can occur after Fatalis exposure, but before the messaging system can set it as active.
    //
    // This is very much an edge case, and it is handled by simply allowing the damage to be processed normally
    // (as in, vanilla game mechanics) and without reporting it via the Apocalypse system. This will allow us to avoid
    // any strange "experience gaps" that would occur, such as messages to the player that they have Fatalis followed
    // by non-Fatalis Player Apocalypse effect messsages. It also prevents any chance that the "exposure event" isn't
    // reported, or that only an exposure event is reported, without the expected follow-up Fatalis death event message 
    // (which would be expected since...we'd be dead).
    // 
    // The normal damage from this rare edge case will not be reported -- we'll need to implement a queue in assembly in order
    // for that and other normally missed events to be caught.
    cmp [fatalisState],1
    je exitPlayerApocalypse
    cmp [fatalisState],2
    jne applyApocalypseRoll
    // To make good on the Fatalis debuff, we set the damage equal to the health.
    movss xmm0,xmm3
    movss [fatalisHealthLost],xmm3
    inc [fatalisDeaths]
    jmp updatePlayerDamageStats
applyApocalypseRoll:
    // Load the parameters for generating the dice roll random number.
    push [apocalypseDieRollLower]
    push [apocalypseDieRollUpper]
    call generateRandomNumber
    // Our random roll value is in eax -- we back it up to the "apocalypseDieRoll"
    // symbol so that the value can be displayed by the event logging display code.
    mov [apocalypseDieRoll],eax
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
    mov rax,epsilon
    movss xmm2,[rax]
    ucomiss xmm1,xmm2
    jb applyExtraDamage
    // Normal damage wouldn't kill us, check if extra damage would, and then prevent it from doing so.
    movss xmm1,xmm3
    movss xmm2,xmm0
    mulss xmm2,[extraDamageX]
    subss xmm1,xmm2
    movss xmm2,[rax]
    ucomiss xmm1,xmm2
    ja applyExtraDamage
    // Set damage to be same as current health minus our "left over" health so that we end up with
    // this health amount following application of damage to our health.
    movss xmm0,xmm3
    subss xmm0,[extraDamageResidualHealth]
    jmp updatePlayerDamageStats
applyExtraDamage:
    mulss xmm0,[extraDamageX]
    jmp updatePlayerDamageStats
teleportitis:
    // Check if teleportitis is disabled. If so, we instead apply extra damage.
    cmp [disableTeleportitis],1
    jne commitTeleportitis
    mov [apocalypseDieRoll],1
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
    movss [lastXDisplacement],xmm1
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
    call generateRandomNumber
    mov [teleportitisResult],eax
    cvtsi2ss xmm1,[teleportitisResult]
    divss xmm1,[teleportitisDivisor]
    // If the Y-axis is not the vertical axis, then we we don't need to check 
    // whether vertical displacement is enabled.
    mov rax,yIsVertical
    cmp [rax],1
    jne skipYSkipCheck
    // If negative vertical displacement is not enabled we do not want to shift it,
    // this causes the random value to remain in the range of 0 to 10.
    cmp [negativeVerticalDisplacementEnabled],1
    jne skipNegativeVerticalYDisplacement
skipYSkipCheck:
    subss xmm1,[teleportitisShifter]
skipNegativeVerticalYDisplacement:
    mulss xmm1,[teleportitisDisplacementX]
    // The vertical displacement value is logged separately and has its own multiplier as changes to it are 
    // often the most consequential. We make sure the Y-axis is the vertical one before logging and multiplying it.
    mov rax,yIsVertical
    cmp [rax],1
    jne skipLastYVerticalDisplacement
    mulss xmm1,[verticalTeleportitisDisplacementX]
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
    movss [lastYDisplacement],xmm1
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
    call generateRandomNumber
    mov [teleportitisResult],eax
    cvtsi2ss xmm1,[teleportitisResult]
    divss xmm1,[teleportitisDivisor]
    // Like the y-axis, the z-axis can sometimes be the vertical axis. So checks 
    // similar to the ones made in the y-coordinate displacement code are made.
    mov rax,yIsVertical
    cmp [rax],0
    jne skipZSkipCheck
    cmp [negativeVerticalDisplacementEnabled],1
    jne skipNegativeVerticalZDisplacement
skipZSkipCheck:
    subss xmm1,[teleportitisShifter]
skipNegativeVerticalZDisplacement:
    mulss xmm1,[teleportitisDisplacementX]
    mov rax,yIsVertical
    cmp [rax],0
    jne skipLastZVerticalDisplacement
    mulss xmm1,[verticalTeleportitisDisplacementX]
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
    movss [lastZDisplacement],xmm1
    addss xmm2,xmm1
    // The updated z-coordinate is commited back into the memory, which will 
    // move the player.
    movss [teleportedZ],xmm2
    cmp [coordinatesAreDoubles],1
    je commitZAsDouble
    movss [rbx+8],xmm2
    jmp updatePlayerDamageStats
commitZAsDouble:
    cvtss2sd xmm1,xmm2
    movsd [rbx+10],xmm1  
    jmp updatePlayerDamageStats
riskOfMurder:
    // Load the parameters for generating the Risk of Murder dice roll random 
    // number.
    push [murderRollLower]
    push [murderRollUpper]
    call generateRandomNumber
    // Our Risk of Murder roll is in eax -- we back it up to the "murderRoll" 
    // symbol so that the value can be displayed by the event logging display code.
    mov [murderRoll],eax
    cmp eax,3
    // If the resulting roll is 4 or 5, then the player is getting murdered.
    jg murder
    // Otherwise, normal damage applies, however there is also now a very slight chance
    // of Fatalis being applied...
    push [fatalisResultLower]
    push [fatalisResultUpper]
    call generateRandomNumber
    // The Fatalis roll is in eax -- we throw it into the "fatalisResult" symbol
    // so we can report on it.
    mov [fatalisResult],eax
    // Fatalis will only be applied if the roll landed on the maximum possible value.
    cmp eax,[fatalisResultUpper]
    jne updatePlayerDamageStats
    // The player has been exposed to Fatalis.
    mov [fatalisState],1
    // The player, although made fragile by the scourge of Fatalis, enters a rage that increases their damage.
    // Store current player damage for later restoration, and then increase it by 1.25x.
    movss xmm1,[playerDamageX]
    movss [basePlayerDamageX],xmm1
    mulss xmm1,[fatalisBloodlustDamageX]
    movss [playerDamageX],xmm1
    jmp updatePlayerDamageStats  
murder:
    // Check if murder is disabled, if so, just apply normal damage.
    cmp [disableMurder],1
    jne commitMurder
    mov [murderRoll],1
    jmp updatePlayerDamageStats
commitMurder:
    mulss xmm0,[murderDamageX]
    jmp updatePlayerDamageStats
suddenGasm:  
    // The player will be fully healed and damage negated. First, we record how much we're healing by 
    // finding the difference between the value of the player maximum health parameter and the current
    // working health value.
    movss xmm0,[rsp+50]
    subss xmm0,xmm3    
    movss [orgasmHealthHealed],xmm0
    // We then load the player maximum health parameter again and store its value in the final player health 
    // (prior to damage applied) register.
    movss xmm3,[rsp+50]
    // We zero out our final damage amount register.
    xorps xmm0,xmm0
    jmp applyPlayerApocalypseExit
updatePlayerDamageStats:
    // If the final damage amount is less than or equal to the current max damage 
    // to the player, it doesn't need to be updated obviously!
    ucomiss xmm0,[maxDamageToPlayer]
    jna skipMaxPlayerDamageUpdate
    movss [maxDamageToPlayer],xmm0
skipMaxPlayerDamageUpdate:
    // We save the final damage amount as the last damage to be done to the player,
    // and add it to the running total.
    movss [lastDamageToPlayer],xmm0
    movss xmm1,xmm0
    addss xmm1,[totalDamageToPlayer]
    movss [totalDamageToPlayer],xmm1
applyPlayerApocalypseExit:
    // Because Apocalypse execution is complete, we trigger an event log entry for 
    // it by setting "logPlayerApocalypse" to 1.
    mov [logPlayerApocalypse],1
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
  

logPlayerApocalypse:
    dd 0
  
apocalypseDieRoll:
    dd 0
  
apocalypseDieRollUpper:
    dd #10
  
apocalypseDieRollLower:
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

lastXDisplacement:
    dd (float)0.0

lastYDisplacement:
    dd (float)0.0

lastZDisplacement:
    dd (float)0.0

negativeVerticalDisplacementEnabled:
    dd 1  

coordinatesAreDoubles:
    dd 0
  
teleportitisDisplacementX:
    dd (float)1.0    

verticalTeleportitisDisplacementX:
    dd (float)1.0

murderRoll:
    dd 0
  
murderRollUpper:
    dd #5
  
murderRollLower:
    dd 1
  
fatalisResult:
    dd 0
  
fatalisResultUpper:
    dd #12
  
fatalisResultLower:
    dd 1
  
fatalisState:
    dd 0

fatalisBloodlustDamageX:
    dd (float)1.25

fatalisHealthLost:
    dd (float)0.0

fatalisDeaths:
    dd 0

fatalisMinutesAfflicted:
    dd 0
  
extraDamageX:
    dd (float)2.0
  
murderDamageX:
    dd (float)69.0

orgasmHealthHealed:
    dd (float)0.0
  
maxDamageToPlayer:
    dd 0
  
lastDamageToPlayer:
    dd 0
  
totalDamageToPlayer:
    dd 0  
  
disableTeleportitis:
    dd 0
  
disableMurder:
    dd 0

murderIsAfoot:
    dd 0

  
// Enemy Apocalypse System Function
// [rsp+58]: Target health value
// [rsp+60]: Damage amount
alloc(executeEnemyApocalypse,$1000)
alloc(lastEnemyDamageEvent,8)
alloc(lastEnemyDamageEventBonusX,8)
alloc(lastEnemyDamageEventBonusAmount,8)
alloc(newEnemyDamagePulse,8)
alloc(newEnemyDamagePulseBonusAmount,8)
alloc(enemyDamagePulses,8)
alloc(newEnemyDamageEvent,8)
alloc(newEnemyDamageEventHasBonus,8)
alloc(newEnemyDamageEventBonusX,8)
alloc(newEnemyDamageEventBonusAmount,8)
alloc(newEnemyDamageEventNotProcessed,8)
alloc(maxEnemyDamageEvent,8)
alloc(maxEnemyDamageEventBonusAmount,8)
alloc(totalEnemyDamage,8)
alloc(totalEnemyDamageBonusAmount,8)
alloc(logEnemyApocalypseGoku,8)
alloc(gokuResult,8)
alloc(gokuResultUpper,8)
alloc(gokuResultLower,8)
alloc(gokuDamageX,8)
alloc(lastEnemyHealthValue,8)
alloc(logEnemyApocalypseCrit,8)
alloc(playerCritChanceResultUpper,8)
alloc(playerCritChanceResultLower,8)
alloc(playerCritChanceResult,8)
alloc(playerCritDamageResultUpper,8)
alloc(playerCritDamageResultLower,8)
alloc(playerCritDamageResult,8)
alloc(playerCritDamageDivisor,8)

registersymbol(executeEnemyApocalypse)
registersymbol(lastEnemyDamageEvent)
registersymbol(lastEnemyDamageEventBonusX)
registersymbol(lastEnemyDamageEventBonusAmount)
registersymbol(newEnemyDamagePulse)
registersymbol(newEnemyDamagePulseBonusAmount)
registersymbol(enemyDamagePulses)
registersymbol(newEnemyDamageEvent)
registersymbol(newEnemyDamageEventHasBonus)
registersymbol(newEnemyDamageEventBonusX)
registersymbol(newEnemyDamageEventBonusAmount)
registersymbol(newEnemyDamageEventNotProcessed)
registersymbol(maxEnemyDamageEvent)
registersymbol(maxEnemyDamageEventBonusAmount)
registersymbol(totalEnemyDamage)
registersymbol(totalEnemyDamageBonusAmount)
registersymbol(logEnemyApocalypseGoku)
registersymbol(gokuDamageX)
registersymbol(gokuResultUpper)
registersymbol(lastEnemyHealthValue)
registersymbol(logEnemyApocalypseCrit)
registersymbol(playerCritDamageResultUpper)
registersymbol(playerCritDamageResultLower)
registersymbol(playerCritDamageResult)

executeEnemyApocalypse:
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
    sub rsp,10
    movdqu [rsp],xmm4
    // Load the enemy's health parameter.  
    movss xmm1,[rsp+58]
    // Load the damage amount parameter.
    movss xmm0,[rsp+60]
    // Check if the damage being done is enough to warrant Apocalypse execution.
    mov rax,damageThreshold
    ucomiss xmm0,[rax]  
    jbe exitEnemyApocalypse  
    cmp [playerGodMode],1
    jne checkForNewEvent
    // If God Mode is enabled, enemies die immediately. The damage, being fake, is not recorded to our
    // normal statistics.
    movss xmm0,[rsp+58]
    movd eax,xmm0
    movd ebx,xmm0
    jmp enemyApocalypseCleanup
checkForNewEvent:
    cmp [newEnemyDamageEventNotProcessed],0
    jne applyDamageByPlayer
    mov [newEnemyDamageEvent],0    
    mov [newEnemyDamageEventBonusAmount],0
    mov [newEnemyDamageEventBonusX],0
    mov [newEnemyDamageEventHasBonus],0
    mov [newEnemyDamageEventNotProcessed],1
applyDamageByPlayer:
    // Clear out the register that will eventually hold any bonus damage amount.
    xorps xmm3,xmm3
    // Apply our base player damage multiplier to the damage amount.
    mulss xmm0,[playerDamageX]
    // If the current damage pulse is part of an event which already has a bonus multiplier, then there is no need 
    // to roll for bonuses again.
    cmp [newEnemyDamageEventHasBonus],1
    je applyExistingBonus
    // Load the parameters for generating the critical hit check.
    push [playerCritChanceResultLower]
    push [playerCritChanceResultUpper]
    call generateRandomNumber
    // Our random roll value is in eax -- we back it up to the 
    // "playerCritChanceResult" symbol so that the value can be displayed by the 
    // event logging display code.
    mov [playerCritChanceResult],eax
    // We can't generate random floats, only random integers. So, to have 
    // a 1.5% crit chance, we generate a number in the range of 0 to 200, and 
    // then check if the value is less than or equal to 3 (3/200 = 0.015).
    cmp eax,#3
    jg checkKamehameha  
    // Load the parameters for generating the critical hit damage.
    push [playerCritDamageResultLower]
    push [playerCritDamageResultUpper]
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
    movss [newEnemyDamageEventBonusX],xmm2
    mov [newEnemyDamageEventHasBonus],1
    // We signal to the event logging system that a crit occurred by setting
    // the "logEnemyApocalypseCrit" symbol to 1.
    mov [logEnemyApocalypseCrit],1  
    jmp applyNewBonus
checkKamehameha:
    // Load the parameters for generating the Kamehameha check.
    push [gokuResultLower]
    push [gokuResultUpper]
    call generateRandomNumber
    // Our random roll value is in eax -- we back it up to the "gokuResult"
    // symbol so that the value can be displayed by the event logging display code.
    mov [gokuResult],eax
    // If the roll is exactly 69, a Kamehameha has occurred! Big damage time baby.
    cmp eax,#69
    jne updateEnemyDamageStats
    movss xmm2,[gokuDamageX]
    movss [newEnemyDamageEventBonusX],xmm2
    mov [newEnemyDamageEventHasBonus],1
    // We signal to the event logging system that a Kamehameha occurred by setting
    // the "logEnemyApocalypseGoku" symbol to 1.
    mov [logEnemyApocalypseGoku],1
applyNewBonus:
    // If there were any previous damage pulses, we'll want to apply the bonus to these amounts retroactively too.
    // So, we take all damage previously done and subtract that from all previous damage multiplied by the bonus.
    movss xmm4,[newEnemyDamageEvent]
    movss xmm3,xmm4
    mulss xmm3,xmm2
    subss xmm3,xmm4
    // We add the current pulse's pre-bonus damage to the previous pulses' pre-bonus damage, so we
    // can figure out the amount of additional bonus damage being applied during this pulse.  
    addss xmm4,xmm0
    // We then apply the bonus to the current pulse's damage, and add the additional bonus damage from previous pulses to it.
    mulss xmm0,xmm2
    addss xmm0,xmm3
    // Now, to figure out this pulse's additional bonus damage, we subtract the pre-bonus damage (which includes pre-bonus amounts
    // from previous pulses, as well the pre-bonus damage from this pulse) from the just calculated amount for this pulse.
    movss xmm3,xmm0
    subss xmm3,xmm4
    jmp updateEnemyDamageStats
applyExistingBonus:
    movss xmm4,xmm0
    // Apply the bonus to the current pulse's damage.
    movss xmm2,[newEnemyDamageEventBonusX]
    mulss xmm0,xmm2
    // We then figure out this pulse's additional bonus damage by subtracting the post-bonus damage from the pre-bonus damage.
    movss xmm3,xmm0
    subss xmm3,xmm4
updateEnemyDamageStats:
    // At this point, the following registers hold the following values:
    // xmm0: The current pulse's final damage amount (including any bonus damage amount).
    // xmm1: The current pulse's target health value.
    // xmm3: The current pulse's bonus damage amount.
    // Sometimes the enemy health is hidden from the player by the game. Well I like
    // flexing my muscle and displaying it anyway on stream with the 
    // "lastEnemyHealthValue" symbol. We perform a mock damage application here and 
    // store it there.
    subss xmm1,xmm0
    movss [lastEnemyHealthValue],xmm1  
    // We publish the current pulse's final and bonus damage amounts to their respective symbols.
    movss [newEnemyDamagePulse],xmm0
    movss [newEnemyDamagePulseBonusAmount],xmm3   
    // Add the final damage amount from this pulse to the running total damage associated with the event.
    movss xmm2,[newEnemyDamageEvent]
    addss xmm2,xmm0
    movss [newEnemyDamageEvent],xmm2
    // Add the bonus damage amount from this pulse to the running total bonus damage associated with this event.
    movss xmm4,[newEnemyDamageEventBonusAmount]
    addss xmm4,xmm3
    movss [newEnemyDamageEventBonusAmount],xmm4
    // If the total damage amount associated with the event is less than or equal to the current max damage event
    // amount, then we don't need to update the max damage event obviously!
    ucomiss xmm2,[maxEnemyDamageEvent]
    jna skipMaxEnemyDamageUpdate
    movss [maxEnemyDamageEvent],xmm2
    movss [maxEnemyDamageEventBonusAmount],xmm4
skipMaxEnemyDamageUpdate:
    // Add the final damage amount from this pulse to the total damage done to enemies.
    movss xmm1,xmm0
    addss xmm1,[totalEnemyDamage]
    movss [totalEnemyDamage],xmm1
    // Add the bonus damage amount from this pulse to the total bonus damage done to enemies.
    movss xmm1,xmm3
    addss xmm1,[totalEnemyDamageBonusAmount]
    movss [totalEnemyDamageBonusAmount],xmm1
exitEnemyApocalypse:
    // We commit our final damage amount to eax.
    movd eax,xmm0
    // We commit our final target health value to ebx, which actually never changes.
    // It is simply used to calculate what the health will be to display with the
    // "lastEnemyHealthValue" stat, and also for purposes of polymorphism, in a 
    // manner of speaking.
    movss xmm1,[rsp+58]
    // Increment the number of enemy damage pulses that have occurred.
    inc [enemyDamagePulses]
enemyApocalypseCleanup:
    movd ebx,xmm1
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
    // This function has 2 parameters, each require 8 bytes. 2x8 == 10 (hex).
    ret 10


lastEnemyDamageEvent:
    dd (float)0.0

lastEnemyDamageEventBonusX:
    dd (float)0.0

lastEnemyDamageEventBonusAmount:
    dd (float)0.0

newEnemyDamagePulse:
    dd (float)0.0

newEnemyDamagePulseBonusAmount:
    dd (float)0.0

enemyDamagePulses:
    dd 0

newEnemyDamageEvent:
    dd (float)0.0

newEnemyDamageEventHasBonus:
    dd 0

newEnemyDamageEventBonusX:
    dd (float)0.0

newEnemyDamageEventBonusAmount:
    dd (float)0.0

newEnemyDamageEventNotProcessed:
    dd 0
 
maxEnemyDamageEvent:
    dd (float)0.0

maxEnemyDamageEventBonusAmount:
    dd (float)0.0

totalEnemyDamage:
    dd (float)0.0

totalEnemyDamageBonusAmount:
    dd (float)0.0

logEnemyApocalypseGoku:
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
    dd #200
  
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
  
logEnemyApocalypseCrit:
    dd 0

  
[DISABLE]

  // Cleanup of global Apocalypse memory
dealloc(playerDamageX)
dealloc(playerGodMode)

unregistersymbol(playerDamageX)
unregistersymbol(playerGodMode)

// Cleanup of Player Apocalypse System Function
unregistersymbol(logPlayerApocalypse)
unregistersymbol(extraDamageSafetyThreshold)
unregistersymbol(teleported)
unregistersymbol(teleportedX)
unregistersymbol(teleportedY)
unregistersymbol(teleportedZ)
unregistersymbol(apocalypseDieRoll)
unregistersymbol(murderRoll)
unregistersymbol(fatalisResult)
unregistersymbol(fatalisResultUpper)
unregistersymbol(fatalisState)
unregistersymbol(fatalisBloodlustDamageX)
unregistersymbol(fatalisHealthLost)
unregistersymbol(fatalisDeaths)
unregistersymbol(fatalisMinutesAfflicted)
unregistersymbol(basePlayerDamageX)
unregistersymbol(lastXDisplacement)
unregistersymbol(lastYDisplacement)
unregistersymbol(lastZDisplacement)
unregistersymbol(lastVerticalDisplacement)
unregistersymbol(coordinatesAreDoubles)
unregistersymbol(negativeVerticalDisplacementEnabled)
unregistersymbol(teleportitisDisplacementX)
unregistersymbol(verticalTeleportitisDisplacementX)
unregistersymbol(orgasmHealthHealed)
unregistersymbol(extraDamageX)
unregistersymbol(murderDamageX)
unregistersymbol(maxDamageToPlayer)
unregistersymbol(lastDamageToPlayer)
unregistersymbol(totalDamageToPlayer)
unregistersymbol(executePlayerApocalypse)
unregistersymbol(disableTeleportitis)
unregistersymbol(disableMurder)
unregistersymbol(murderIsAfoot)

dealloc(logPlayerApocalypse)
dealloc(teleported)
dealloc(teleportedX)
dealloc(teleportedY)
dealloc(teleportedZ)
dealloc(apocalypseDieRoll)
dealloc(apocalypseDieRollUpper)
dealloc(apocalypseDieRollLower)
dealloc(extraDamageResidualHealth)
dealloc(extraDamageSafetyThreshold)
dealloc(teleportitisResult)
dealloc(teleportitisResultUpper)
dealloc(teleportitisResultLower)
dealloc(teleportitisDivisor)
dealloc(teleportitisShifter)
dealloc(lastZDisplacement)
dealloc(lastYDisplacement)
dealloc(lastXDisplacement)
dealloc(lastVerticalDisplacement)
dealloc(coordinatesAreDoubles)
dealloc(negativeVerticalDisplacementEnabled)
dealloc(teleportitisDisplacementX)
dealloc(verticalTeleportitisDisplacementX)
dealloc(murderRoll)
dealloc(murderRollUpper)
dealloc(murderRollLower)
dealloc(basePlayerDamageX)
dealloc(fatalisMinutesAfflicted)
dealloc(fatalisDeaths)
dealloc(fatalisHealthLost)
dealloc(fatalisBloodlustDamageX)
dealloc(fatalisResult)
dealloc(fatalisResultUpper)
dealloc(fatalisResultLower)
dealloc(fatalisState)
dealloc(extraDamageX)
dealloc(murderDamageX)
dealloc(orgasmHealthHealed)
dealloc(maxDamageToPlayer)
dealloc(lastDamageToPlayer)
dealloc(totalDamageToPlayer)
dealloc(disableTeleportitis)
dealloc(disableMurder)
dealloc(murderIsAfoot)
dealloc(executePlayerApocalypse)

// Cleanup of Enemy Apocalypse System Function
unregistersymbol(lastEnemyDamageEvent)
unregistersymbol(lastEnemyDamageEventBonusX)
unregistersymbol(lastEnemyDamageEventBonusAmount)
unregistersymbol(newEnemyDamagePulse)
unregistersymbol(newEnemyDamagePulseBonusAmount)
unregistersymbol(enemyDamagePulses)
unregistersymbol(newEnemyDamageEvent)
unregistersymbol(newEnemyDamageEventHasBonus)
unregistersymbol(newEnemyDamageEventBonusX)
unregistersymbol(newEnemyDamageEventBonusAmount)
unregistersymbol(newEnemyDamageEventNotProcessed)
unregistersymbol(maxEnemyDamageEvent)
unregistersymbol(maxEnemyDamageEventBonusAmount)
unregistersymbol(totalEnemyDamage)
unregistersymbol(totalEnemyDamageBonusAmount)
unregistersymbol(logEnemyApocalypseGoku)
unregistersymbol(gokuDamageX)
unregistersymbol(gokuResultUpper)
unregistersymbol(lastEnemyHealthValue)
unregistersymbol(playerCritDamageResult)
unregistersymbol(playerCritDamageResultLower)
unregistersymbol(playerCritDamageResultUpper)
unregistersymbol(logEnemyApocalypseCrit)
unregistersymbol(executeEnemyApocalypse)

dealloc(lastEnemyDamageEvent)
dealloc(lastEnemyDamageEventBonusX)
dealloc(lastEnemyDamageEventBonusAmount)
dealloc(newEnemyDamagePulse)
dealloc(newEnemyDamagePulseBonusAmount)
dealloc(newEnemyDamageEvent)
dealloc(newEnemyDamageEventHasBonus)
dealloc(newEnemyDamageEventBonusX)
dealloc(newEnemyDamageEventBonusAmount)
dealloc(newEnemyDamageEventNotProcessed)
dealloc(enemyDamagePulses)
dealloc(maxEnemyDamageEvent)
dealloc(maxEnemyDamageEventBonusAmount)
dealloc(totalEnemyDamage)
dealloc(totalEnemyDamageBonusAmount)
dealloc(logEnemyApocalypseGoku)
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
dealloc(logEnemyApocalypseCrit)
dealloc(executeEnemyApocalypse)