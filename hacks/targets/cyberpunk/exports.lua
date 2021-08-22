--[[
    Exported configuration settings for Omnified Cyberpunk 2077.
    Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
    Copyright 2021 Bad Echo LLC
    
    Bad Echo Technologies are licensed under a
    Creative Commons Attribution-NonCommercial 4.0 International License.
    
    See accompanying file LICENSE.md or a copy at:
    http://creativecommons.org/licenses/by-nc/4.0/
--]]

require("statisticMessages")

function registerExports()    
    -- Cyberpunk-specific timers.
    if playerCarHackTimer == nil then
        playerCarHackTimer = createTimer(getMainForm())
    end

    -- Every five seconds, while driving (in control of a car AND moving, with the game unpaused),
    -- there is a small chance that an evil enemy hacker will manage to hack into our car and suddenly
    -- change our speed! Remember, Carpocalypse is in effect (50x damage to vehicles), so any mistake will
    -- lead to certain death!
    playerCarHackTimer.Interval = 5000
    playerCarHackTimer.OnTimer = function()
        local playerDriving = readInteger("playerDriving")

        -- The player is driving if they're piloting a car and not at rest.
        if playerDriving ~= 1 then 
            do return end
        end

        local gamePaused = readInteger("gamePaused")

        -- Don't want the hack happening while we're paused (escape menu)!
        if gamePaused == 1 then
            do return end
        end

        local randomCarHack = {
            {false, 50},
            {true, 1}
        }

        local carHacked = randomize(randomCarHack)

        if not carHacked then 
            do return end
        end
        
        -- Our car has been hacked! The evil enemy hacker now sets our speed to a random value.
        local randomCarHackSpeedBoost = {
            {2.0, 5},
            {3.0, 3},
            {4.0, 2},
            {5.0, 1}
        }

        local carHackSpeedBoost = randomize(randomCarHackSpeedBoost)

        writeFloat("playerVehicleSpeedX", carHackSpeedBoost)
        playSound(findTableFile("carHackedSiren.wav"))
    end

    -- Custom statistics.
    AdditionalStatistics = function()        
        local magazine = toInt(readInteger("[playerMagazine]+0x350"))

        -- A ridiculous value indicates that the previous place in memory has been freshly reallocated.
        if magazine ~= nil and magazine > 1000 then magazine = 0 end

        local playerVehicleSpeedX = readFloat("playerVehicleSpeedX")
        local vehicleDamageX = readFloat("vehicleDamageX")
        local playerExperience = toInt(readInteger("[playerExperience]"))
        local playerExperienceNext = toInt(readInteger("[playerExperience]+0x4"))

        if (areNotNil(playerExperience, playerExperienceNext)) then
            playerExperienceNext = playerExperienceNext + playerExperience
        end        

        return {
            WholeStatistic("Magazine", magazine),
            StatisticGroup("Vehicle", {
                WholeStatistic("Speed", playerVehicleSpeedX, true, "{0}x"),
                WholeStatistic("Damage", vehicleDamageX, false, "{0}x"),
            }),            
            FractionalStatistic("Experience", playerExperience, playerExperienceNext)
        }
    end
end

function unregisterExports()    
    
end