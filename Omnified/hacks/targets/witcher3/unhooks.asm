// Unhooks for Omnified Witcher 3
// Written By: Matt Weber (https://twitch.tv/omni)
// Copyright 2021 Bad Echo LLC

// Cleanup of omniPlayerVitalsHook
omniPlayerVitalsHook:
db 8B 0C 90 89 0E
  
unregistersymbol(omniPlayerVitalsHook)
unregistersymbol(playerVitals)
unregistersymbol(playerAbilityManager)

dealloc(getPlayerVitals)
dealloc(playerAbilityManager)
dealloc(playerVitals)


// Cleanup of omniPlayerPhysicsHook
omniPlayerPhysicsHook:
db 0F 2F 83 98 01 00 00

unregistersymbol(omniPlayerPhysicsHook)
unregistersymbol(playerPhysics)

dealloc(getPlayerPhysics)
dealloc(playerPhysics)


// Cleanup of omniPlayerHook
omniPlayerHook:
  db 48 8B 01 48 8D 54 24 60

  unregistersymbol(omniPlayerHook)
  unregistersymbol(player)
  unregistersymbol(playerLocation)
  
  dealloc(getPlayer)
  dealloc(player)
  dealloc(getPlayerLocation)
  

// Cleanup of omniPlayerXPHook
omniPlayerXPHook:
  db 49 8B D7 48 8B CB

unregistersymbol(omniPlayerXPHook)
unregistersymbol(playerXP)

dealloc(playerXP)
dealloc(getPlayerXP)


// Cleanup of omnifyXPHook
omnifyXPHook:
  db 01 03 8B 03 48 85 F6

unregistersymbol(omnifyXPHook)
unregistersymbol(xpX)

dealloc(changeXP)
dealloc(xpX)


// Cleanup of omnifyApocalypseHook
omnifyApocalypseHook:
  db F3 0F 5C 44 24 40

unregistersymbol(omnifyApocalypseHook)

dealloc(initiateApocalypse)


// Cleanup of omnifyPlayerSpeedHook
omnifyPlayerSpeedHook:
  db 48 8B 43 08 48 89 86 B8 01 00 00

unregistersymbol(omnifyPlayerSpeedHook)
unregistersymbol(playerSpeedX)
unregistersymbol(playerVerticalX)

dealloc(identityValue)
dealloc(playerSpeedX)
dealloc(playerVerticalX)
dealloc(applyPlayerSpeed)


// Cleanup of omnifyPredatorHook
omnifyPredatorHook:
  db 8B 02 89 41 70

unregistersymbol(omnifyPredatorHook)

dealloc(initiatePredator)


// Cleanup of omnifyAbomnificationHook
omnifyAbomnificationHook:
  db 41 C7 06 FF FF 7F 7F

unregistersymbol(omnifyAbomnificationHook)

dealloc(initiateAbomnification)


// Cleanup of omnifyApplyAbomnificationHook
omnifyApplyAbomnificationHook:
  db 41 8B 00 89 41 40

unregistersymbol(omnifyApplyAbomnificationHook)

dealloc(headHeightCoefficient)
dealloc(headHeightShifter)
dealloc(applyAbomnification)