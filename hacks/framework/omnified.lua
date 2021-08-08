-- Omnified Framework
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

require("defines")
require("utility")

function readPlayerCoordinates()
	local x = not coordinatesAreDoubles 
				and readFloat(playerCoordinatesXAddress) 
				or readDouble(playerCoordinatesXAddress)

	local y = not coordinatesAreDoubles 
				and readFloat(playerCoordinatesYAddress) 
				or readDouble(playerCoordinatesYAddress)

	local z = not coordinatesAreDoubles 
				and readFloat(playerCoordinatesZAddress) 
				or readDouble(playerCoordinatesZAddress)

	if x ~= x then x = 0 end
	if y ~= y then y = 0 end
	if z ~= z then z = 0 end

	local coordinates = { X = x, Y = y,	Z = z }

	return coordinates
end

local function mark()
	local coordinates = readPlayerCoordinates()

	if areNotNil(coordinates.X,coordinates.Y,coordinates.Z) then
		if not coordinatesAreDoubles then
			writeFloat("teleportX", coordinates.X)						
			writeFloat("teleportY", coordinates.Y)
			writeFloat("teleportZ", coordinates.Z)
		else
			writeDouble("teleportX", coordinates.X)			 
			writeDouble("teleportY", coordinates.Y)
			writeDouble("teleportZ", coordinates.Z)
		end
	end	
end

local function recall()
	writeInteger("teleport", 1)
end

local function assemble(assemblyFilePath, disableInfo)
	local assemblyFile = io.open(assemblyFilePath)

	if assemblyFile == nil then 		
	  return false, "Assembly file location at " .. assemblyFilePath ..  " was unable to be opened."	  
	end
  
	local assembly = assemblyFile:read("a")
	assemblyFile.close()
  
	assembly = "[ENABLE]\n" .. assembly

	return autoAssemble(assembly, false, disableInfo)
end

local function registerModule(modulePath, moduleName)
	local result, resultDisableInfo = assemble(modulePath)

	if not result then 
		print(string.format("Failed to register %s:\r\n%s", moduleName, resultDisableInfo))
	end

	return result, resultDisableInfo
end

local function unregisterModule(modulePath, moduleName, disableInfo)
	local result, resultDisableInfo = assemble(modulePath, disableInfo)

	if not result then
		print(string.format("Failed to unregister %s:\r\n%s", moduleName, resultDisableInfo))
	end

	return not result
end

local function registerTargetExports()
	if not isPackageAvailable("exports") then
		do return end
	end

	require("exports")

	if registerExports == nil then
		print(	"Exports file does not conform to Omnified target binary configuration interface. " ..
				"Registration method should be named 'registerExports'.")
	else 
		registerExports()
	end	
end

local function unregisterTargetExports()
	if not package.loaded["exports"] then
		do return end
	end

	if unregisterExports == nil then 
		print(	"Exports file does not conform to Omnified target binary configuration interface. " ..
				"Unregistration method should be named 'unregisterExports'.")
		do return end
	end

	unregisterExports()

	if package.loaded["exports"] then
		package.loaded["exports"] = nil
		_G["exports"] = nil
	end
end

local DEFAULT_FRAMEWORK_PATH = "..\\..\\framework\\"
local OMNIFIED_MODULE_NAME = "Omnified framework assembly functions"
local APOCALYPSE_MODULE_NAME = "Apocalypse system"
local PREDATOR_MODULE_NAME = "Predator system"
local ABOMNIFICATION_MODULE_NAME = "Abomnification system"
local TARGET_MODULE_NAME = "target binary"

function registerOmnification(targetAssemblyFilePath, pathToFramework)
	if not omnifiedRegistered then
		markHotkey = createHotkey(mark, VK_NUMPAD8)
		recallHotkey = createHotkey(recall, VK_NUMPAD9)

		if pathToFramework == nil then pathToFramework = DEFAULT_FRAMEWORK_PATH end

		omnifiedRegistered, omnifiedDisableInfo 
			= registerModule(pathToFramework .. "omnified.asm", OMNIFIED_MODULE_NAME)

		apocalypseRegistered, apocalypseDisableInfo
			= registerModule(pathToFramework .. "systems\\apocalypse.asm", APOCALYPSE_MODULE_NAME)
		
		predatorRegistered, predatorDisableInfo 
			= registerModule(pathToFramework .. "systems\\predator.asm", PREDATOR_MODULE_NAME)			

		abomnificationRegistered, abomnificationDisableInfo 
			= registerModule(pathToFramework .. "systems\\abomnification.asm", ABOMNIFICATION_MODULE_NAME)			

		if not areTrue(omnifiedRegistered, apocalypseRegistered, predatorRegistered, abomnificationRegistered) then
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
					local previousPlayerDamageX =  readFloat("basePlayerDamageX")

					if previousPlayerDamageX ~= nil then
						writeFloat("playerDamageX", previousPlayerDamageX)
					end 
					
					fatalisTimer.Enabled = false
					fatalisTimer.Destroy()
					fatalisTimer = nil
				end
			end
		end
	end

  	if not targetAssemblyRegistered then
    	targetAssemblyRegistered, targetAssemblyDisableInfo 
			= registerModule(targetAssemblyFilePath, TARGET_MODULE_NAME)
  	end

  	registerTargetExports()
end

function unregisterOmnification(targetAssemblyFilePath, pathToFramework)
	if pathToFramework == nil then pathToFramework = DEFAULT_FRAMEWORK_PATH end

	if omnifiedRegistered then
		omnifiedRegistered 
			= unregisterModule(pathToFramework .. "omnified.asm", OMNIFIED_MODULE_NAME, omnifiedDisableInfo)
	end

	if apocalypseRegistered then
		apocalypseRegistered 
			= unregisterModule(pathToFramework .. "systems\\apocalypse.asm", APOCALYPSE_MODULE_NAME, apocalypseDisableInfo)
	end

	if predatorRegistered then
		predatorRegistered
			= unregisterModule(pathToFramework .. "systems\\predator.asm", PREDATOR_MODULE_NAME, predatorDisableInfo)
	end

	if abomnificationRegistered then
		abomnificationRegistered
			= unregisterModule(pathToFramework .. "systems\\abomnification.asm", ABOMNIFICATION_MODULE_NAME, abomnificationDisableInfo)
	end

	if markHotkey ~= nil then
		markHotkey.destroy()
		markHotkey = nil
	end

	if recallHotkey ~= nil then
		recallHotkey.destroy()
		recallHotkey = nil
	end

	if statusTimer ~= nil then
		if fatalisTimer ~= nil then
			fatalisTimer.Enabled = false
			fatalisTimer.Destroy()
			fatalisTimer = nil
		end
		
		statusTimer.Enabled = false
		statusTimer.Destroy()
		statusTimer = nil
	end

	if targetAssemblyRegistered then
		targetAssemblyRegistered = unregisterModule(targetAssemblyFilePath, TARGET_MODULE_NAME, targetAssemblyDisableInfo)
	end

	unregisterTargetExports()
end