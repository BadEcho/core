-- Omnified Framework v0.5
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

-- These values are to be overridden in target .CT file registration routines.
coordinatesAreDoubles = false
playerCoordinatesXAddress = "[example]+0x10"
playerCoordinatesYAddress = "[example]+0x14"
playerCoordinatesZAddress = "[example]+0x18"

function areNotNil(...)
	for _,v in ipairs({...}) do
		if v == nil then return false end
	end

	return true
end

-- Outputs a randomly selected item from the provided random settings structure.
-- The structure of random_settings is composed of object, weighted probability values like so:
-- random_settings = {
--		{firstObject, 1},
--		{secondObject, 3},
-- }
-- In this example, the second object is 3x more likely to be returned than the first object.
-- Based on the items provided, a random number between 1 and 4 (inclusive) will be generated.
-- If the random number is 1 then firstObject is returned. If it is 2, 3, or 4, secondObject is returned.
function randomize(random_settings)
	local indexedResults = {}
	local totalWeight = 0
	local lastIndex = 0

	if not randomInitialized then
		math.randomseed(os.time())
		randomInitialized = true
	end
	
	for k,v in pairs(random_settings) do
		local resultObject = v[1]
		local weight = v[2]

		totalWeight = totalWeight + weight
		while lastIndex ~= totalWeight do
			lastIndex = lastIndex + 1
			table.insert(indexedResults, lastIndex, resultObject)
		end
	end

	local result = math.random(1, lastIndex)

	return indexedResults[result]
end

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

	local coordinates = { X = x, Y = y,	Z = z }

	return coordinates
end

function mark()
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

		frameworkRegistered, frameworkDisableInfo = assemble("..\\..\\framework\\Omnified.asm")		
		
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

		frameworkRegistered = assemble("..\\..\\framework\\Omnified.asm", frameworkDisableInfo)
		
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