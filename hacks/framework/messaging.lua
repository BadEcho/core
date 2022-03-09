--[[
    Omnified messaging library.
    Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
    Copyright 2022 Bad Echo LLC
    
    Bad Echo Technologies are licensed under the
    GNU Affero General Public License v3.0.
    
    See accompanying file LICENSE.md or a copy at:
    https://www.gnu.org/licenses/agpl-3.0.html
--]]

require("omnified")
require("statisticMessages")
require("apocalypseMessages")

-- Reads the current death counter from a local death counter file.
local function readDeathCounter()
    local deathCounterFile = assert(io.open("deathCounter.txt", "r"))

    local deathCounterFromFile = deathCounterFile:read("*n")

    if deathCounterFromFile == nil then
        deathCounterFromFile = 0
    end

    deathCounterFile:close()

    return deathCounterFromFile
end

local function writeDeathCounter(deathCounter)
    local deathCounterFile = assert(io.open("deathCounter.txt", "w"))
    deathCounterFile:write(deathCounter)
    deathCounterFile:close()
end

-- Creates a JSON-encoded dump of hacked game statistics.
local function dumpStatistics()
    local playerHealth = not healthIsInteger
                            and toInt(readFloat(playerHealthAddress))
                            or toInt(readInteger(playerHealthAddress))

    local playerMaxHealth = not healthIsInteger
                                and toInt(readFloat(playerMaxHealthAddress))
                                or toInt(readInteger(playerMaxHealthAddress))

    local playerStamina = not staminaIsInteger
                            and toInt(readFloat(playerStaminaAddress))
                            or toInt(readInteger(playerStaminaAddress))

    local playerMaxStamina = not staminaIsInteger
                                and toInt(readFloat(playerMaxStaminaAddress))
                                or toInt(readInteger(playerMaxStaminaAddress))

    -- Last damaged enemy health is always floating point, as it is maintained by the Apocalypse system.
    local lastEnemyHealthValue = toInt(readFloat("lastEnemyHealthValue"))  
    local lastDamageToPlayer = toInt(readFloat("lastDamageToPlayer"))
    local maxDamageToPlayer = toInt(readFloat("maxDamageToPlayer"))
    local totalDamageToPlayer = toInt(readFloat("totalDamageToPlayer"))
    local lastEnemyDamageEvent = toInt(readFloat("lastEnemyDamageEvent"))
    local maxEnemyDamageEvent = toInt(readFloat("maxEnemyDamageEvent"))
    local totalEnemyDamage = toInt(readFloat("totalEnemyDamage"))
    local enemyDamagePulses = readInteger("enemyDamagePulses")

    local playerCoordinates = readPlayerCoordinates()
        
    local playerDamageX = toInt(readFloat("playerDamageX") * 100)
    local playerSpeedX = toInt(readFloat("playerSpeedX") * 100)

    local deathCounterFromFile = readDeathCounter()
    local deathCounter = toInt(readInteger("deathCounter"))

    if deathCounterFromFile ~= 0 and deathCounter == 0 then
        deathCounter = deathCounterFromFile
        writeInteger("deathCounter", deathCounter)
    end

    writeDeathCounter(deathCounter)

    local statistics = {
        FractionalStatistic("Health", playerHealth, playerMaxHealth, "#AA43BC50", "#AA27D88D"),
        FractionalStatistic("Stamina", playerStamina, playerMaxStamina, "#AA7515D9", "#AAB22DE5"),
        WholeStatistic("Enemy Health", lastEnemyHealthValue),
        StatisticGroup("Damage Taken", { 
            WholeStatistic("Last", lastDamageToPlayer), 
            WholeStatistic("Max", maxDamageToPlayer, true),
            WholeStatistic("Total", totalDamageToPlayer)
        }),
        StatisticGroup("Damage Inflicted", {
            WholeStatistic("Hits", enemyDamagePulses),
            WholeStatistic("Last", lastEnemyDamageEvent),
            WholeStatistic("Max", maxEnemyDamageEvent, true),
            WholeStatistic("Total", totalEnemyDamage)
        }),
        CoordinateStatistic("Coordinates", playerCoordinates.X, playerCoordinates.Y, playerCoordinates.Z),
        WholeStatistic("Player Damage", playerDamageX, false, "{0}%"),
        WholeStatistic("Player Speed", playerSpeedX, false, "{0}%"),
        WholeStatistic("Deaths", deathCounter)
    }

    local additionalIndex = 2

    for _, v in pairs(additionalStatistics ~= nil and additionalStatistics() or {}) do
        additionalIndex = additionalIndex + 1
        table.insert(statistics, additionalIndex, v)
    end 

    return jsonEncode(statistics)
end

-- Creates a JSON-Lines-encoded dump of recent Player Apocalypse events.
local function dumpPlayerApocalypseEvent()
    local apocalypseEvent = ApocalypseEvent()    

    -- Fatalis checks.
    local fatalisState = readInteger("fatalisState")

    -- A 'fatalisState' of 3 means we've been cured of Fatalis!
    if fatalisState == 3 then
        local fatalisDeaths = readInteger("fatalisDeaths")
        local fatalisMinutesAfflicted = readInteger("fatalisMinutesAfflicted")

        writeInteger("fatalisState", 0)
        writeInteger("fatalisDeaths", 0)
        writeInteger("fatalisMinutesAfflicted", 0)

        return jsonEncode(FatalisCuredEvent(apocalypseEvent, fatalisDeaths, fatalisMinutesAfflicted))
    end

    -- A 'fatalisState' of 2 means our exposure to Fatalis has developed into an affliction and, because we've just been damaged,
    -- we're dead.     
    if fatalisState == 2 then
        local fatalisHealthLost = toInt(readFloat("fatalisHealthLost"))

        return jsonEncode(FatalisDeathEvent(apocalypseEvent, fatalisHealthLost))
    end

    -- Main Player Apocalypse die roll checks.  
    local apocalypseDieRoll = readInteger("apocalypseDieRoll")    
    local lastDamageToPlayer = toInt(readFloat("lastDamageToPlayer"))
    local healthAfter = not healthIsInteger
                            and toInt(readFloat(playerHealthAddress))
                            or readInteger(playerHealthAddress)    
    apocalypseEvent 
        = PlayerApocalypseEvent(apocalypseEvent, apocalypseDieRoll, lastDamageToPlayer, healthAfter)
    
    if apocalypseDieRoll <= 4 then
        -- Extra damage.
        local extraDamageX = readFloat("extraDamageX")

        apocalypseEvent = ExtraDamageEvent(apocalypseEvent, extraDamageX)    
    elseif apocalypseDieRoll <= 6 then
        -- Teleportitis.
        local lastXDisplacement = readFloat("lastXDisplacement")
        local lastYDisplacement = readFloat("lastYDisplacement")
        local lastZDisplacement = readFloat("lastZDisplacement")
        local lastVerticalDisplacement = readFloat("lastVerticalDisplacement")
        local isFreeFalling = lastVerticalDisplacement > freeFallThreshold
        
        apocalypseEvent = TeleportitisEvent(apocalypseEvent,
                                            lastXDisplacement,
                                            lastYDisplacement,
                                            lastZDisplacement,
                                            isFreeFalling)         
    elseif apocalypseDieRoll <= 9 then
        -- Risk of Murder.
        local murderRoll = readInteger("murderRoll")
        apocalypseEvent = RiskOfMurderEvent(apocalypseEvent, murderRoll)
        
        if murderRoll <= 3 then
            local fatalisAfflicted = fatalisState == 1

            if fatalisAfflicted then 
                -- The Fatalis will now be treated as an affliction upon the next hit to the player.
                writeInteger("fatalisState", 2)
            end

            -- Normal damage. If we've been exposeed to Fatalis, then we must be sure to announce it!
            apocalypseEvent = NormalDamageEvent(apocalypseEvent, fatalisAfflicted)                            
        else
            -- Murder!
            local murderDamageX = readFloat("murderDamageX")

            apocalypseEvent = MurderEvent(apocalypseEvent, murderDamageX)
        end
    -- Orgasm!    
    else 
        local orgasmHealthHealed = toInt(readFloat("orgasmHealthHealed"))

        apocalypseEvent = OrgasmEvent(apocalypseEvent, orgasmHealthHealed)
    end

    return jsonEncode(apocalypseEvent)
end

-- Creates a JSON-Lines-encoded dump of recent Enemy Apocalypse events.
local function dumpEnemyApocalypseEvent()
    local apocalypseEvent = ApocalypseEvent()

    local logEnemyApocalypseCrit = readInteger("logEnemyApocalypseCrit")    
    local lastEnemyDamageEventBonusX = readFloat("lastEnemyDamageEventBonusX")
    local lastEnemyDamageEventBonusAmount = toInt(readFloat("lastEnemyDamageEventBonusAmount"))

    if logEnemyApocalypseCrit == 1 then
        local playerCritDamageResult = readInteger("playerCritDamageResult")
        local playerCritDamageResultUpper = readInteger("playerCritDamageResultUpper")
        local playerCritDamageResultLower = readInteger("playerCritDamageResultLower")                

        local playerCritDamageRange
            = playerCritDamageResultUpper - playerCritDamageResultLower
        local playerCritExtremeMinimum
            = playerCritDamageResultUpper - (playerCritDamageRange * (1/3))
      
        local isExtreme = playerCritDamageResult >= playerCritExtremeMinimum

        apocalypseEvent = EnemyApocalypseEvent(apocalypseEvent,
                                               lastEnemyDamageEventBonusAmount,
                                               lastEnemyDamageEventBonusX,
                                               BonusDamageType.CriticalHit,
                                               isExtreme)
    else 
        apocalypseEvent = EnemyApocalypseEvent(apocalypseEvent,
                                               lastEnemyDamageEventBonusAmount,
                                               lastEnemyDamageEventBonusX,
                                               BonusDamageTpe.Kamehameha,
                                               false)
    end
    
    return jsonEncode(apocalypseEvent)
end

-- Saves any newly accumulated damage pulses as the most recent damage event.
local function commitNewDamageEvents()
    local newEnemyDamageEvent = readFloat("newEnemyDamageEvent")
    local newEnemyDamageEventBonusAmount = readFloat("newEnemyDamageEventBonusAmount")
    local newEnemyDamageEventBonusX = readFloat("newEnemyDamageEventBonusX")

    if newEnemyDamageEvent > 0 then
        writeFloat("lastEnemyDamageEvent", newEnemyDamageEvent)
    end    

    if newEnemyDamageEventBonusAmount > 0 then
        writeFloat("lastEnemyDamageEventBonusAmount", newEnemyDamageEventBonusAmount)
    end

    if newEnemyDamageEventBonusX > 0 then
        writeFloat("lastEnemyDamageEventBonusX", newEnemyDamageEventBonusX)
    end
end

-- Processes recent Apocalypse events.
local function processApocalypseEvents()
    local newEnemyDamageEventNotProcessed = readInteger("newEnemyDamageEventNotProcessed")
    local logPlayerApocalypse = readInteger("logPlayerApocalypse")
    local logEnemyApocalypseGoku = readInteger("logEnemyApocalypseGoku")
    local logEnemyApocalypseCrit = readInteger("logEnemyApocalypseCrit")

    if newEnemyDamageEventNotProcessed == 1 then
        commitNewDamageEvents()      
        writeInteger("newEnemyDamageEventNotProcessed", 0)
    end

    if logPlayerApocalypse == 1 then 
        local playerApocalypseEvent = dumpPlayerApocalypseEvent()
        writeInteger("logPlayerApocalypse", 0) 

        return playerApocalypseEvent
    end    

    if any(1, logEnemyApocalypseGoku, logEnemyApocalypseCrit) then
        local enemyApocalypseEvent = dumpEnemyApocalypseEvent()
        writeInteger("logEnemyApocalypseCrit", 0)
        writeInteger("logEnemyApocalypseGoku", 0)        

        return enemyApocalypseEvent
    end
end

-- Enables the publishing of Apocalypse event messages.
function startApocalypseEventsPublisher()
    if apocalypseTimer == nil then
        apocalypseTimer = createTimer(getMainForm())
    end

    apocalypseTimer.Interval = 50
    apocalypseTimer.OnTimer = function()
        local apocalypseEvent = processApocalypseEvents()

        if apocalypseEvent ~= nil then
            -- New files don't get a newline, otherwise we'll have a blank first line. Duh!
            if fileExists("apocalypse.jsonl") then apocalypseEvent = "\n" .. apocalypseEvent end            
            local apocalypseEventsFile = assert(io.open("apocalypse.jsonl", "a"))

            apocalypseEventsFile:write(apocalypseEvent)
            apocalypseEventsFile:close()
        end
    end
end

-- Disables the publishing of Apocalypse event messages.
function stopApocalypseEventsPublisher()
    if apocalypseTimer == nil then return end

    apocalypseTimer.Enabled = false
    apocalypseTimer.Destroy()
    apocalypseTimer = nil
end


-- Enables the publishing of Omnified game statistic messages.
function startStatisticsPublisher()
    if statisticsTimer == nil then
        statisticsTimer = createTimer(getMainForm())        
    end

    statisticsTimer.Interval = 50
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