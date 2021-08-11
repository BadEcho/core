--[[
    Omnified messaging library.
    Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
    Copyright 2021 Bad Echo LLC
    
    Bad Echo Technologies are licensed under a
    Creative Commons Attribution-NonCommercial 4.0 International License.
    
    See accompanying file LICENSE.md or a copy at:
    http://creativecommons.org/licenses/by-nc/4.0/
--]]

require("omnified")
require("statisticMessages")

-- Creates a JSON-encoded dump of hacked game statistics.
local function dumpStatistics()
    local playerHealth = not healthIsInteger
                            and toInt(readFloat(playerHealthAddress))
                            or readInteger(playerHealthAddress)

    local playerMaxHealth = not healthIsInteger
                                and toInt(readFloat(playerMaxHealthAddress))
                                or readInteger(playerMaxHealthAddress)

    local playerStamina = not staminaIsInteger
                            and toInt(readFloat(playerStaminaAddress))
                            or readInteger(playerStaminaAddress)

    local playerMaxStamina = not staminaIsInteger
                                and toInt(readFloat(playerMaxStaminaAddress))
                                or readInteger(playerMaxStaminaAddress)    

    -- Last damaged enemy health always floating point, as it is maintained by the Apocalypse system.
    local enemyHealth = toInt(readFloat("lastEnemyHealthValue"))

    local lastDamageByPlayerNew = readFloat("lastDamageByPlayerNew")

    if lastDamageByPlayerNew ~= nil and lastDamageByPlayerNew > 0 then
        writeFloat("lastDamageByPlayer", lastDamageByPlayerNew)        
        writeFloat("lastDamageByPlayerNew", 0)
    end

    local lastDamageToPlayer = toInt(readFloat("lastDamageToPlayer"))
    local maxDamageToPlayer = toInt(readFloat("maxDamageToPlayer"))
    local lastDamageByPlayer = toInt(readFloat("lastDamageByPlayer"))
    local maxDamageByPlayer = toInt(readFloat("maxDamageByPlayer"))
    local totalDamageByPlayer = toInt(readFloat("totalDamageByPlayer"))

    local playerCoordinates = readPlayerCoordinates()
        
    local playerDamageX = readFloat("playerDamageX")

    local statistics = {
        FractionalStatistic("Health", playerHealth, playerMaxHealth),
        FractionalStatistic("Stamina", playerStamina, playerMaxStamina),
        WholeStatistic("Enemy Health", enemyHealth),
        StatisticGroup("Damage Taken", { 
            WholeStatistic("Last", lastDamageToPlayer), 
            WholeStatistic("Max", maxDamageToPlayer, true),
        }),
        StatisticGroup("Damage Inflicted", {
            WholeStatistic("Last", lastDamageByPlayer),
            WholeStatistic("Max", maxDamageByPlayer, true),
            WholeStatistic("Total", totalDamageByPlayer)
        }),
        CoordinateStatistic("Coordinates", playerCoordinates.X, playerCoordinates.Y, playerCoordinates.Z)
    }

    local additionalIndex = 2

    for _, v in pairs(AdditionalStatistics()) do
        additionalIndex = additionalIndex + 1
        table.insert(statistics, additionalIndex, v)
    end

    return jsonEncode(statistics)
end

-- Enables the publishing of Omnified game statistic messages.
function startStatisticsPublisher()
    if statisticsTimer == nil then
        statisticsTimer = createTimer(getMainForm())        
    end

    statisticsTimer.Interval = 400
    statisticsTimer.OnTimer = function()
        local statistics = dumpStatistics()
        local statisticsFile = assert(io.open("statistics.json","w"))

        statisticsFile:write(statistics)
        statisticsFile:close()
    end
end

-- Disable the publishing of Omnified game statistic messages.
function stopStatisticsPublisher()
    if statisticsTimer == nil then return end
    
    statisticsTimer.Enabled = false
    statisticsTimer.Destroy()
    statisticsTimer = nil
end