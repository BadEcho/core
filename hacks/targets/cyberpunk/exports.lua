-- Exported configuration settings for Omnified Cyberpunk 2077.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

function registerExports()
    if playerAttackingTimer == nil then
        playerAttackingTimer = createTimer(getMainForm())
    end
    
    playerAttackingTimer.Interval = 500
    playerAttackingTimer.OnTimer = function()
        local playerAttacking = readInteger("playerAttacking")
                
        if playerAttacking == nil or playerAttacking == 0 then
            do return end
        else
            playerAttacking = playerAttacking - 1            

            writeInteger("playerAttacking", playerAttacking)
        end
    end
end

function unregisterExports()
    
    if playerAttackingTimer ~= nil then
        playerAttackingTimer.Enabled = false
        playerAttackingTimer.destroy()
        playerAttackingTimer = nil 
    end
end