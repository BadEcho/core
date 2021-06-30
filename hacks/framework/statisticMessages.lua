-- General-purpose Omnified statistic message definitions.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

require("omnified")

StatisticType = define_enum {
    "Whole",
    "Fractional",
    "Coordinate"
}

function createWholeStat(name, value)
    local wholeStat = {
        Type = StatisticType.Whole,
        Statistic = {
            Name = name,
            Value = value
        }
    }
    
    return wholeStat
end

function createFractionalStat(name, currentValue, maximumValue)
    local fractionalStat = {
        Type = StatisticType.Fractional,
        Statistic = {
            Name = name,
            CurrentValue = currentValue,
            MaximumValue = maximumValue
        }
    }

    return fractionalStat
end

function createCoordinateStat(name, x, y, z)
    local coordinateStat = {
        Type = StatisticType.Coordinate,
        Statistic = {
            Name = name,
            X = x,
            Y = y,
            Z = z
        }
    }

    return coordinateStat
end