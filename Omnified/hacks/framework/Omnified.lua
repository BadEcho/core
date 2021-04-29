-- Omnified Framework v0.5
-- Written By: Matt Weber (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

-- These values are to be overridden in target .CT file registration routines.
coordinatesAreDoubles = false
playerCoordinatesX = "[example]+0x10"
playerCoordinatesY = "[example]+0x14"
playerCoordinatesZ = "[example]+0x18"

function areNotNil(...)
	for _,v in ipairs({...}) do
		if v == nil then return false end
	end

	return true
end

function mark()
	local currentX = not coordinatesAreDoubles 
						and readFloat(playerCoordinatesX) 
						or readDouble(playerCoordinatesX)
		
	local currentY = not coordinatesAreDoubles 
						and readFloat(playerCoordinatesY) 
						or readDouble(playerCoordinatesY)
		
	local currentZ = not coordinatesAreDoubles 
						and readFloat(playerCoordinatesZ) 
						or readDouble(playerCoordinatesZ)

	if areNotNil(currentX,currentY,currentZ) then
		if not coordinatesAreDoubles then
			writeFloat("teleportX", currentX)						
			writeFloat("teleportY", currentY)
			writeFloat("teleportZ", currentZ)
		else
			writeDouble("teleportX", currentX)			 
			writeDouble("teleportY", currentY)
			writeDouble("teleportZ", currentZ)
		end
	end	
end

function recall()
	writeInteger("teleport", 1)
end

function hook(hookFilePath)
  local hookFile = assert(io.open(hookFilePath))

  if hookFile == nil then 
    print("Hook file located at ", hookFilePath, " was unable to be opened.")
    do return end
  end

  local hooks = hookFile:read("a")
  hookFile.close()

  local registered, disableInfo = autoAssemble(hooks)

  if registered == false then
    print("Failed to register hooks located in ", hookFilePath)
  end

  return registered, disableInfo
end


function unhook(unhookFilePath, disableInfo)
  local unhookFile = assert(io.open(unhookFilePath))

  if unhookFile == nil then 
    print("Unhook file located at ", unhookFilePath, " was unable to be opened.")
    do return end
  end

  local unhooks = unhookFile:read("a")
  unhookFile.close()

  local unregistered = autoAssemble(unhooks, disableInfo)

  if unregistered == false then
    print("Failed to unregister hooks located in ", unhookFilePath)    
  end
    
  return unregistered
end

frameworkRegistered = false
frameworkDisableInfo = nil

targetHooksRegistered = false
targetHooksDisableInfo = nil

function registerOmnification(targetHookFilePath)
	if not frameworkRegistered then
		markHotkey = createHotkey(mark, VK_NUMPAD8)
		recallHotkey = createHotkey(recall, VK_NUMPAD9)

		frameworkRegistered, frameworkDisableInfo = hook("..\\..\\framework\\Omnified.hooks.asm")		
		
		if not frameworkRegistered then
			print("Failed to register Omnified framework.")
			do return end
		end
		
		if statusTimer == nil then
			statusTimer = createTimer(getMainForm())
		end
		
		statusTimer.Interval = 400
		statusTimer.OnTimer = function()
			local fatalisState 
				= readInteger("fatalisState")
				
			if fatalisState == 1 and fatalisTimer == nil then
				fatalisTimer = createTimer(getMainForm())
				fatalisTimer.Interval = 600000
				fatalisTimer.OnTimer = function()
					writeInteger("fatalisState",2)		
					
					fatalisTimer.Enabled = false
					fatalisTimer.Destroy()
					fatalisTimer = nil
				end
			end
		end
	end

  if not targetHooksRegistered then
    targetHooksRegistered, targetHooksDisableInfo = hook(targetHookFilePath)

    if not targetHooksRegistered  then
      print("Failed to register target hooks.")
    end
  end
end

function unregisterOmnification(targetUnhookFilePath)
	if frameworkRegistered and frameworkDisableInfo ~= nil then
		markHotkey.destroy()
		recallHotkey.destroy()

		frameworkRegistered = unhook("..\\..\\framework\\Omnified.unhooks.asm", frameworkDisableInfo)
		
		if not frameworkRegistered then
			print("Failed to unregister Omnified framework.")
		else 
			frameworkRegistered = false
			
			if statusTimer ~= nil then
				if fatalisTimer ~= nil then
					fatalisTimer.Enabled = false
					fatalisTimer.Destroy()
				end
				
				statusTimer.Enabled = false
				statusTimer.Destroy()
				statusTimer = nil
			end
		end		
	else 
		print("Omnified framework already unregistered.")
	end

  if targetHooksRegistered and targetHooksDisableInfo ~= nil then
    targetHooksRegistered = unhook(targetUnhookFilePath, targetHooksDisableInfo)

    if not targetHooksRegistered then
      print("Failed to unregister target hooks.")
    else
      targetHooksRegistered = false
    end
  end
end