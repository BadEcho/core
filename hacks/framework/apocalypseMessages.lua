--[[
    Defines the Omnified Apocalypse event messaging schema.
    Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
    Copyright 2022 Bad Echo LLC
    
    Bad Echo Technologies are licensed under a
    Creative Commons Attribution-NonCommercial 4.0 International License.
    
    See accompanying file LICENSE.md or a copy at:
    http://creativecommons.org/licenses/by-nc/4.0/
--]]

require("utility")

EventType = defineEnum {
    "Enemy",
    "ExtraDamage",
    "Teleportitis",
    "NormalDamage",
    "Murder",
    "Orgasm",
    "FatalisDeath",
    "FatalisCured"
}

BonusDamageType = defineEnum {
    "CriticalHit",
    "Kamehameha"
}

function ApocalypseEvent()
    local time = os.time()    
    local timestamp = os.date("!%Y-%m-%dT%H:%M:%SZ", time) 

    return {
        Timestamp = timestamp
    }
end

function EnemyApocalypseEvent(apocalypseEvent, additionalDamage, bonusMultiplier, bonusDamageType, isExtreme)  
    apocalypseEvent.AdditionalDamage = additionalDamage
    apocalypseEvent.BonusMultiplier = bonusMultiplier
    apocalypseEvent.BonusDamageType = bonusDamageType
    apocalypseEvent.IsExtreme = isExtreme

    local enemyApocalypseEvent = {
        Type = EventType.Enemy,
        Event = apocalypseEvent
    }

    return enemyApocalypseEvent
end

function PlayerApocalypseEvent(apocalypseEvent, dieRoll, damage, healthAfter)  
    apocalypseEvent.DieRoll = dieRoll
    apocalypseEvent.Damage = damage
    apocalypseEvent.HealthAfter = healthAfter

    return apocalypseEvent
end

function ExtraDamageEvent(playerApocalypseEvent, extraDamageMultiplier)
    playerApocalypseEvent.ExtraDamageMultiplier = extraDamageMultiplier

    local extraDamageEvent = {
        Type = EventType.ExtraDamage,
        Event = playerApocalypseEvent
    }

    return extraDamageEvent
end

function TeleportitisEvent(playerApocalypseEvent, xDisp, yDisp, zDisp, isFreeFalling)
    playerApocalypseEvent.XDisplacement = xDisp
    playerApocalypseEvent.YDisplacement = yDisp
    playerApocalypseEvent.ZDisplacement = zDisp
    playerApocalypseEvent.IsFreeFalling = isFreeFalling

    local teleportitisEvent = {
        Type = EventType.Teleportitis,
        Event = playerApocalypseEvent
    }

    return teleportitisEvent
end

function RiskOfMurderEvent(playerApocalypseEvent, murderRoll)
    playerApocalypseEvent.MurderRoll = murderRoll

    return playerApocalypseEvent
end

function NormalDamageEvent(riskOfMurderEvent, fatalisAfflicted)
    riskOfMurderEvent.FatalisAfflicted = fatalisAfflicted

    local normalDamageEvent = {
        Type = EventType.NormalDamage,
        Event = riskOfMurderEvent
    }

    return normalDamageEvent
end

function MurderEvent(riskOfMurderEvent, murderMultiplier)
    riskOfMurderEvent.MurderMultiplier = murderMultiplier
    
    local murderEvent = {
        Type = EventType.Murder,
        Event = riskOfMurderEvent
    }

    return murderEvent
end

function OrgasmEvent(playerApocalypseEvent, healthHealed)
    playerApocalypseEvent.HealthHealed = healthHealed

    local orgasmEvent = {
        Type = EventType.Orgasm,
        Event = playerApocalypseEvent
    }

    return orgasmEvent
end

function FatalisDeathEvent(apocalypseEvent, healthLost)
    apocalypseEvent.HealthLost = healthLost
    
    local fatalisDeathEvent = {
        Type = EventType.FatalisDeath,
        Event = apocalypseEvent
    }

    return fatalisDeathEvent
end

function FatalisCuredEvent(apocalypseEvent, deaths, minutesAfflicted)
    apocalypseEvent.Deaths = deaths
    apocalypseEvent.MinutesAfflicted = minutesAfflicted
    
    local fatalisCuredEvent = {
        Type = EventType.FatalisCured,
        Event = apocalypseEvent
    }

    return fatalisCuredEvent
end