//----------------------------------------------------------------------
// The Abomnification System
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2022 Bad Echo LLC
// 
// Bad Echo Technologies are licensed under the
// GNU Affero General Public License v3.0.
//
// See accompanying file LICENSE.md or a copy at:
// https://www.gnu.org/licenses/agpl-3.0.html
//----------------------------------------------------------------------

// Abomnification Data Footprint
// 0x0: Remaining # of morph steps in current cycle - Integer
// 0x4: Effective width scale - Float
// 0x8: Effective height scale - Float
// 0xC: Effective depth scale - Float
// 0x10: Morph Mode - Integer
//          Value indicates the type of morph mode, and is a result of an RNG roll.
//          1: Static Unnatural (doesn't change shape; unnaturally large or small).
//          2-5: Uniform Morphing (all dimensional scales change uniformly).
//          6-13: Nonuniform Morphing (all dimensional scales change independently).
// 0x14: Initial width scale for morph cycle - Float
// 0x18: Initial height scale for morph cycle - Float
// 0x1C: Initial depth scale for morph cycle - Float
// 0x20: Number of morph steps for each cycle - Float (due to being used in several floating-point calculations)
// 0x24: Target width scale for morph cycle - Float
// 0x28: Target height scale for morph cycle - Float
// 0x2C: Target depth scale for morph cycle - Float
// 0x30: Current phase - Integer
//          0: Pause phase; no actual morphing is occurring. Lasts for a normal morph cycle.
//          1: Active phase; morphing is occurring.
// 0x34: Status - Integer
//          An entity's status determines whether or not it will be affected by the Abomnification system, and is
//          generated prior to its first morphing cycle.
//          0: Entity's status is undetermined.
//          1: Entity is not Abomnified
//          2: Entity is Abomnified
alloc(morphScaleData,$33FFCC)
  
registersymbol(morphScaleData)


// Abomnification System Function
// [rsp+10]: The identifying address.
// Return Values
// eax: Updated width scale
// ebx: Updated height scale
// ecx: Updated depth scale
alloc(executeAbomnification,$1000)
alloc(abomnifyMorphStepsResultUpper,8)
alloc(abomnifyMorphStepsResultLower,8)
alloc(abomnifyHeightResultUpper,8)
alloc(abomnifyHeightResultLower,8)
alloc(abomnifyWidthResultUpper,8)
alloc(abomnifyWidthResultLower,8)
alloc(abomnifyDepthResultUpper,8)
alloc(abomnifyDepthResultLower,8)
alloc(abomnifyMorphModeResultUpper,8)
alloc(abomnifyMorphModeResultLower,8)
alloc(abomnifyPercentage,8)
alloc(morphEverything,8)
alloc(unnaturalBigThreshold,8)
alloc(unnaturalBigX,8)
alloc(unnaturalSmallX,8)
alloc(abomnifyDivisor,8)
alloc(defaultScaleX,8)
alloc(speedMorph,8)
alloc(speedMorphDivisor,8)
alloc(stopMorphs,8)
alloc(zeroValue,8)
alloc(overrideMorphSteps,8)
alloc(overrideMorphStepsValue,8)
alloc(disableAbomnification,8)
alloc(defaultScaleX,8)

registersymbol(executeAbomnification)
registersymbol(abomnifyMorphStepsResultUpper)
registersymbol(abomnifyMorphStepsResultLower)
registersymbol(abomnifyHeightResultUpper)
registersymbol(abomnifyHeightResultLower)
registersymbol(abomnifyWidthResultUpper)
registersymbol(abomnifyWidthResultLower)
registersymbol(abomnifyDepthResultUpper)
registersymbol(abomnifyDepthResultLower)
registersymbol(abomnifyPercentage)
registersymbol(morphEverything)
registersymbol(unnaturalBigThreshold)
registersymbol(unnaturalBigX)
registersymbol(speedMorphDivisor)
registersymbol(unnaturalSmallX)
registersymbol(speedMorph)
registersymbol(stopMorphs)
registersymbol(overrideMorphSteps)
registersymbol(overrideMorphStepsValue)
registersymbol(disableAbomnification)
registersymbol(defaultScaleX)

executeAbomnification:
    push rdx  
    cmp [disableAbomnification],1
    jne checkMorphStatus
    mov eax,[defaultScaleX]
    mov ebx,[defaultScaleX]
    mov ecx,[defaultScaleX]
    jmp executeAbomnificationCleanup
checkMorphStatus:
    mov rbx,[rsp+10]
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
    // Skip the status check if forced morphing is enabled.
    cmp [morphEverything],0
    jne applyMorphScaleFromData
    cmp [rdx+34],0
    jne evaluteMorphStatus
    // Abomnification is active if the roll is less than or equal to the configured percentage.
    // For example, the roll, which is out of 100, must land on 1-25 if the percentage is 25.    
    push #1
    push #100
    call generateRandomNumber
    mov rcx,[abomnifyPercentage]
    cmp eax,ecx
    // This will set 'al' to 0 if the roll fell outside of the configured success percentage; 1 otherwise.
    setge al
    // Incrementing 'al' will bring the value inline with our defined status values (1 == failure, 2 == success).
    inc al
    mov byte ptr [rdx+34],al
evaluteMorphStatus:
    cmp [rdx+34],2
    je applyMorphScaleFromData
    // Morphing is disabled for entity.
    mov eax,[defaultScaleX]
    mov ebx,[defaultScaleX]
    mov ecx,[defaultScaleX]
    jmp executeAbomnificationCleanup
applyMorphScaleFromData:
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
    je loadAbomnifiedScales
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
    push [abomnifyMorphStepsResultLower]
    push [abomnifyMorphStepsResultUpper]
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
    cmp [speedMorph],0
    je generateMonsterMorphTargets
    divss xmm0,[speedMorphDivisor]  
generateMonsterMorphTargets:  
    cvtss2si eax,xmm0
    mov [rdx],eax
    push [abomnifyHeightResultLower]
    push [abomnifyHeightResultUpper]
    call generateRandomNumber
    cvtsi2ss xmm0,eax
    divss xmm0,[abomnifyDivisor]
    movss [rdx+28],xmm0
    cmp [rdx+10],0 
    jne applyMorphMode
    push [abomnifyMorphModeResultLower]
    push [abomnifyMorphModeResultUpper]
    call generateRandomNumber
    mov [rdx+10],eax
applyMorphMode:
    cmp [rdx+10],1
    je staticUnnaturalMorphing
    cmp [rdx+10],5
    jle uniformMorphing
    jmp nonUniformMorphing
staticUnnaturalMorphing:
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
    movss [rdx+24],xmm0
    movss [rdx+2C],xmm0
    jmp initializeMorphStepsExit
nonUniformMorphing:
    push [abomnifyWidthResultLower]
    push [abomnifyWidthResultUpper]
    call generateRandomNumber
    cvtsi2ss xmm0,eax
    divss xmm0,[abomnifyDivisor]
    movss [rdx+24],xmm0
    push [abomnifyDepthResultLower]
    push [abomnifyDepthResultUpper]
    call generateRandomNumber
    cvtsi2ss xmm0,eax
    divss xmm0,[abomnifyDivisor]
    movss [rdx+2C],xmm0
initializeMorphStepsExit:
    pop rax
    movdqu xmm0,[rsp]
    add rsp,10
    jmp loadAbomnifiedScales
executeMorphPhase:
    // Current phase is located in [rdx+30].
    // 0 == Pause phase. Do nothing!
    // 1 == Step morph phase. Continue morphing!
    cmp [rdx+30],1
    je stepMorph
    jmp loadAbomnifiedScales
stepMorph:
    cmp [rdx+10],1  
    je loadAbomnifiedScales
    sub rsp,10
    movdqu [rsp],xmm0
    sub rsp,10
    movdqu [rsp],xmm1
    push rsi
    movss xmm0,[rdx+20]
    cmp [speedMorph],0
    je generateMorphsForStep
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
    // Height Step
    movss xmm0,[rdx+28]
    subss xmm0,[rdx+18]
    mulss xmm0,xmm1
    addss xmm0,[rdx+18]
    movss [rdx+8],xmm0
    // Depth Step
    movss xmm0,[rdx+2C]
    subss xmm0,[rdx+1C]
    mulss xmm0,xmm1
    addss xmm0,[rdx+1C]
    movss [rdx+C],xmm0
    pop rsi
    movdqu xmm1,[rsp]
    add rsp,10
    movdqu xmm0,[rsp]
    add rsp,10
loadAbomnifiedScales:
    mov eax,[rdx+4]
    mov ebx,[rdx+8]
    mov ecx,[rdx+C]
executeAbomnificationCleanup:  
    pop rdx
    ret 8  


abomnifyMorphStepsResultUpper:
    dd #400

abomnifyMorphStepsResultLower:
    dd #25

abomnifyHeightResultUpper:
    dd #215

abomnifyHeightResultLower:
    dd #25

abomnifyWidthResultUpper:
    dd #275

abomnifyWidthResultLower:
    dd #25

abomnifyDepthResultUpper:
    dd #300

abomnifyDepthResultLower:
    dd #25

abomnifyMorphModeResultUpper:
    dd #13

abomnifyMorphModeResultLower:
    dd 1

abomnifyPercentage:
    dd #50

morphEverything:
    dd 0

abomnifyDivisor:
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
  
overrideMorphSteps:
    dd 0
  
overrideMorphStepsValue:
    dd (float)400.0
  
disableAbomnification:
    dd 0
  
defaultScaleX:
    dd (float)1.0
  
  
// Retrieves the Abomnified scale multipliers for the entity identified.
// [rsp+10]: The identifying address.
alloc(getAbomnifiedScales,$1000)

registersymbol(getAbomnifiedScales)

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
    mov rdx,defaultScaleX
    mov eax,[rdx]
    mov ebx,[rdx]
    mov ecx,[rdx]
getAbomnifiedScalesCleanup:
    pop rdx
    ret 8


// Force enables Abomnification for the entity identified.
// [rsp+20]: The identifying address. 
alloc(enableAbomnification,$1000)

registersymbol(enableAbomnification)

enableAbomnification:
    push rax
    push rbx
    push rcx
    mov rax,[rsp+20]
    mov rbx,#52
    movzx rcx,ax
    imul ebx,ecx
    mov rax,morphScaleData
    add rax,rbx
    mov [rax+34],2
    pop rcx
    pop rbx
    pop rax
    ret 8


[DISABLE]

// Cleanup of Abomnification Scale Data
unregistersymbol(morphScaleData)

dealloc(morphScaleData)


// Cleanup of Abomnification System Function
unregistersymbol(executeAbomnification)
unregistersymbol(abomnifyMorphStepsResultUpper)
unregistersymbol(abomnifyMorphStepsResultLower)
unregistersymbol(abomnifyHeightResultUpper)
unregistersymbol(abomnifyHeightResultLower)
unregistersymbol(abomnifyWidthResultUpper)
unregistersymbol(abomnifyWidthResultLower)
unregistersymbol(abomnifyDepthResultUpper)
unregistersymbol(abomnifyDepthResultLower)
unregistersymbol(abomnifyPercentage)
unregistersymbol(morphEverything)
unregistersymbol(unnaturalBigThreshold)
unregistersymbol(unnaturalBigX)
unregistersymbol(unnaturalSmallX)
unregistersymbol(speedMorphDivisor)
unregistersymbol(speedMorph)
unregistersymbol(stopMorphs)
unregistersymbol(overrideMorphSteps)
unregistersymbol(overrideMorphStepsValue)
unregistersymbol(disableAbomnification)
unregistersymbol(defaultScaleX)

dealloc(defaultScaleX)
dealloc(stopMorphs)
dealloc(speedMorph)
dealloc(speedMorphDivisor)
dealloc(defaultScaleX)
dealloc(abomnifyMorphStepsResultUpper)
dealloc(abomnifyMorphStepsResultLower)
dealloc(abomnifyHeightResultUpper)
dealloc(abomnifyHeightResultLower)
dealloc(abomnifyWidthResultUpper)
dealloc(abomnifyWidthResultLower)
dealloc(abomnifyDepthResultUpper)
dealloc(abomnifyDepthResultLower)
dealloc(abomnifyMorphModeResultUpper)
dealloc(abomnifyMorphModeResultLower)
dealloc(abomnifyPercentage)
dealloc(morphEverything)
dealloc(unnaturalBigThreshold)
dealloc(unnaturalBigX)
dealloc(unnaturalSmallX)
dealloc(abomnifyDivisor)
dealloc(zeroValue)
dealloc(overrideMorphSteps)
dealloc(overrideMorphStepsValue)
dealloc(disableAbomnification)
dealloc(executeAbomnification)


// Cleanup of getAbomnifiedScales
unregistersymbol(getAbomnifiedScales)

dealloc(getAbomnifiedScales)


// Cleanup of enableAbomnification
unregistersymbol(enableAbomnification)

dealloc(enableAbomnification)