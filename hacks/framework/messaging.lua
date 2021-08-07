-- Omnified messaging library.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

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

    local lastDamageToPlayer = toInt(readFloat("lastDamageToPlayer"))
    local maxDamageToPlayer = toInt(readFloat("maxDamageToPlayer"))
    local lastDamageByPlayer = toInt(readFloat("lastDamageByPlayer"))
    local maxDamageByPlayer = toInt(readFloat("maxDamageByPlayer"))
    local totalDamageByPlayer = toInt(readFloat("totalDamageByPlayer"))

    local playerCoordinates = readPlayerCoordinates()
    local playerX = round(playerCoordinates.X, 2)
    local playerY = round(playerCoordinates.Y, 2)
    local playerZ = round(playerCoordinates.Z, 2)

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
        CoordinateStatistic("Coordinates", playerX, playerY, playerZ)
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