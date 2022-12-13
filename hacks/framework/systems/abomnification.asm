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
alloc(defaultScaleX,8)

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
registersymbol(defaultScaleX)

executeAbomnification:
    push rdx  
    cmp [disableAbomnification],1
    jne continueAbomnification
    mov eax,[defaultScaleX]
    mov ebx,[defaultScaleX]
    mov ecx,[defaultScaleX]
    jmp executeAbomnificationCleanup
continueAbomnification:
    mov rbx,[rsp+10]
    cmp [forceScrub],1
    je scrubMorphData  
    cmp rbx,0
    jne checkMorphStatus  
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
    mov [rdx+34],0
    jmp nextMorphDataScrub
exitMorphDataScrub:
    pop rax
incrementMorphDataIndex:
    inc [morphScaleIndex]
    mov rcx,[morphScaleIndex]
    mov [rbx],ecx
checkMorphStatus:
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
    cmp [rdx+34],0
    jne evaluteMorphStatus
    // 1/2 chance for Abomnification to be active.
    push 1
    push 2
    call generateRandomNumber
    mov [rdx+34],eax
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
    push [abominifyMorphStepsResultLower]
    push [abominifyMorphStepsResultUpper]
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
    call generateRandomNumber
    cvtsi2ss xmm0,eax
    divss xmm0,[abominifyDivisor]
    movss [rdx+28],xmm0
    cmp [rdx+10],0 
    jne applyMorphMode
    push [abominifyMorphModeResultLower]
    push [abominifyMorphModeResultUpper]
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
    push [abominifyWidthResultLower]
    push [abominifyWidthResultUpper]
    call generateRandomNumber
    cvtsi2ss xmm0,eax
    divss xmm0,[abominifyDivisor]
    movss [rdx+24],xmm0
    push [abominifyDepthResultLower]
    push [abominifyDepthResultUpper]
    call generateRandomNumber
    cvtsi2ss xmm0,eax
    divss xmm0,[abominifyDivisor]
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
  
defaultScaleX:
    dd (float)1.0
  
  
// Retrieves the Abomnified scale multipliers for the specified
// morph scale ID.
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
  

[DISABLE]

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
unregistersymbol(defaultScaleX)

dealloc(defaultScaleX)
dealloc(stopMorphs)
dealloc(speedMorph)
dealloc(speedMorphDivisor)
dealloc(defaultScaleX)
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

dealloc(getAbomnifiedScales)