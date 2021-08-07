-- Omnified messaging library.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

require("omnified")
require("statisticMessages")

-- Creates a JSON-encoded dump of hacked game statistics.
local function dumpStatistics()
    local playerHealth = not healthIsInteger
                            and round(readFloat(playerHealthAddress))
                            or readInteger(playerHealthAddress)

    local playerMaxHealth = not healthIsInteger
                                and round(readFloat(playerMaxHealthAddress))
                                or readInteger(playerMaxHealthAddress)

    local playerStamina = not staminaIsInteger
                            and round(readFloat(playerStaminaAddress))
                            or readInteger(playerStaminaAddress)

    local playerMaxStamina = not staminaIsInteger
                                and round(readFloat(playerMaxStaminaAddress))
                                or readInteger(playerMaxStaminaAddress)    

    -- Last damaged enemy health always floating point, as it is maintained by the Apocalypse system.
    local enemyHealth = round(readFloat("lastEnemyHealthValue"))

    local lastDamageToPlayer = round(readFloat("lastDamageToPlayer"))
    local maxDamageToPlayer = round(readFloat("maxDamageToPlayer"))
    local lastDamageByPlayer = round(readFloat("lastDamageByPlayer"))
    local maxDamageByPlayer = round(readFloat("maxDamageByPlayer"))
    local totalDamageByPlayer = round(readFloat("totalDamageByPlayer"))

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

    for _, v in pairs(AdditionalStatistics()) do
        table.insert(statistics, v)
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