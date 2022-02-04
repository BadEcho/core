//----------------------------------------------------------------------
// The Predator System
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2022 Bad Echo LLC
// 
// Bad Echo Technologies are licensed under a
// Creative Commons Attribution-NonCommercial 4.0 International License.
//
// See accompanying file LICENSE.md or a copy at:
// http://creativecommons.org/licenses/by-nc/4.0/
//----------------------------------------------------------------------

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
    push rax
    mov rax,yIsVertical  
    cmp [rax],0
    pop rax
    je commitZChange
    mulss xmm3,xmm8
commitZChange:
    movd ecx,xmm3
    shufps xmm3,xmm3,0x87
    push rax
    mov rax,yIsVertical
    cmp [rax],1
    pop rax
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
    push rax
    mov rax,yIsVertical  
    cmp [rax],1
    pop rax
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
    push rax
    mov rax,yIsVertical
    cmp [rax],0
    pop rax
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


[DISABLE]

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