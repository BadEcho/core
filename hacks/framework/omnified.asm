//----------------------------------------------------------------------
// Omnified Framework Assembly Functions v. 0.5
// Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC
// 
// Bad Echo Technologies are licensed under a
// Creative Commons Attribution-NonCommercial 4.0 International License.
//
// See accompanying file LICENSE.md or a copy at:
// http://creativecommons.org/licenses/by-nc/4.0/
//----------------------------------------------------------------------

// Global memory.
alloc(zero,8)
alloc(epsilon,8)
alloc(damageThreshold,8)
alloc(yIsVertical,8)
alloc(negativeOne,8)
alloc(percentageDivisor,8)

registersymbol(epsilon)
registersymbol(zero)
registersymbol(damageThreshold)
registersymbol(yIsVertical)
registersymbol(negativeOne)
registersymbol(percentageDivisor)

zero:
  dd 0

epsilon:
  dd (float)0.001

damageThreshold:
  dd (float)3.9

yIsVertical:
  dd 1
  
negativeOne:
  dd (float)-1.0
  
percentageDivisor:
  dd (float)100.0

  
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


[DISABLE]

// Cleanup of global memory
unregistersymbol(percentageDivisor)
unregistersymbol(epsilon)
unregistersymbol(yIsVertical)
unregistersymbol(negativeOne)
unregistersymbol(zero)

dealloc(zero)
dealloc(percentageDivisor)
dealloc(epsilon)
dealloc(damageThreshold)
dealloc(yIsVertical)
dealloc(negativeOne)

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