-- Omnified Framework v0.5
-- Written By: Matt Weber (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

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