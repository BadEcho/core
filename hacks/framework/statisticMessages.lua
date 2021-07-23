-- Defines the Omnified game statistic messaging schema.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

require("utility")

StatisticType = defineEnum {
    "Whole",
    "Fractional",
    "Coordinate",
    "Group"
}

function WholeStatistic(name, value, isCritical)
    local wholeStatistic = {
        Type = StatisticType.Whole,
        Statistic = {
            Name = name,
            IsCritical = isCritical,
            Value = value
        }
    }
    
    return wholeStatistic
end

function FractionalStatistic(name, currentValue, maximumValue)
    local fractionalStatistic = {
        Type = StatisticType.Fractional,
        Statistic = {
            Name = name,
            CurrentValue = currentValue,
            MaximumValue = maximumValue
        }
    }

    return fractionalStatistic
end

function CoordinateStatistic(name, x, y, z)
    local coordinateStatistic = {
        Type = StatisticType.Coordinate,
        Statistic = {
            Name = name,
            X = x,
            Y = y,
            Z = z
        }
    }

    return coordinateStatistic
end

function StatisticGroup(name, statistics)
    local statisticGroup = {
        Type = StatisticType.Group,        
        Statistic = {
            Name = name,
            Children = statistics
        }
    }

    return statisticGroup
end