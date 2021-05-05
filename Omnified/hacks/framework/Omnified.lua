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

function assemble(assemblyFilePath, disableInfo)
	local assemblyFile = assert(io.open(assemblyFilePath))

	if assemblyFile == nil then 
	  print("Assembly file located at ", assemblyFilePath, " was unable to be opened.")
	  do return end
	end
  
	local assembly = assemblyFile:read("a")
	assemblyFile.close()
  
	assembly = "[ENABLE]\n" .. assembly

	local result, resultDisableInfo = autoAssemble(assembly, false, disableInfo)
  
	if result == false then
	  print("Failed to assemble ", assemblyFilePath)
	end
  
	return result, resultDisableInfo
end

frameworkRegistered = false
frameworkDisableInfo = nil

targetAssemblyRegistered = false
targetAssemblyDisableInfo = nil

function registerOmnification(targetAssemblyFilePath)
	if not frameworkRegistered then
		markHotkey = createHotkey(mark, VK_NUMPAD8)
		recallHotkey = createHotkey(recall, VK_NUMPAD9)

		frameworkRegistered, frameworkDisableInfo = assemble("..\\..\\framework\\Omnified.hooks.asm")		
		
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

  if not targetAssemblyRegistered then
    targetAssemblyRegistered, targetAssemblyDisableInfo = assemble(targetAssemblyFilePath)

    if not targetAssemblyRegistered  then
      print("Failed to register target assembly.")
    end
  end
end

function unregisterOmnification(targetAssemblyFilePath)
	if frameworkRegistered and frameworkDisableInfo ~= nil then
		markHotkey.destroy()
		recallHotkey.destroy()

		frameworkRegistered = assemble("..\\..\\framework\\Omnified.hooks.asm", frameworkDisableInfo)
		
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

  if targetAssemblyRegistered and targetAssemblyDisableInfo ~= nil then
    targetAssemblyRegistered = assemble(targetAssemblyFilePath, targetAssemblyDisableInfo)

    if not targetAssemblyRegistered then
      print("Failed to unregister target assembly.")
    else
      targetAssemblyRegistered = false
    end
  end
end