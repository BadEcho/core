-- Omnified messaging library.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

require("omnified")
require("statisticMessages")

-- Creates a JSON-encoded dump of hacked game statistics.
local function dumpStatistics()
    local playerHealth = not healthIsInteger
                            and floor(readFloat(playerHealthAddress))
                            or readInteger(playerHealthAddress)

    local playerMaxHealth = not healthIsInteger
                                and floor(readFloat(playerMaxHealthAddress))
                                or readInteger(playerMaxHealthAddress)

    -- Last damaged enemy health always floating point, as it is maintained by the Apocalypse system.
    local enemyHealth = floor(readFloat(lastEnemyHealthValue))

    local lastDamageToPlayer = floor(readFloat("lastDamageToPlayer"))
    local maxDamageToPlayer = floor(readFloat("maxDamageToPlayer"))
    local lastDamageByPlayer = floor(readFloat("lastDamageByPlayer"))
    local maxDamageByPlayer = floor(readFloat("maxDamageByPlayer"))
    local totalDamageByPlayer = floor(readFloat("totalDamageByPlayer"))

    local playerCoordinates = readPlayerCoordinates()

    local playerDamageX = readFloat("playerDamageX")

    local statistics = {
        FractionalStatistic("Health", playerHealth, playerMaxHealth),
        WholeStatistic("Enemy Health", enemyHealth),
        WholeStatistic("Last Damage Taken", lastDamageToPlayer),
        WholeStatistic("Max Damage Taken", maxDamageToPlayer),
        WholeStatistic("Last Damage Done", lastDamageByPlayer),
        WholeStatistic("Max Damage Done", maxDamageByPlayer),
        WholeStatistic("Total Damage Done", totalDamageByPlayer),
        CoordinateStatistic("Coordinates", playerCoordinates.X, playerCoordinates.Y, playerCoordinates.Z)
    }

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