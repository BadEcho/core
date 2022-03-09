--[[
    Defines the Omnified game statistic messaging schema.
    Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
    Copyright 2022 Bad Echo LLC
    
    Bad Echo Technologies are licensed under the
    GNU Affero General Public License v3.0.
    
    See accompanying file LICENSE.md or a copy at:
    https://www.gnu.org/licenses/agpl-3.0.html
--]]

require("utility")

StatisticType = defineEnum {
    "Whole",
    "Fractional",
    "Coordinate",
    "Group"
}

function WholeStatistic(name, value, isCritical, format)
    local wholeStatistic = {
        Type = StatisticType.Whole,
        Statistic = {
            Name = name,
            IsCritical = isCritical,
            Format = format,
            Value = value
        }
    }
    
    return wholeStatistic
end

function FractionalStatistic(name, currentValue, maximumValue, primaryBarColor, secondaryBarColor)
    local fractionalStatistic = {
        Type = StatisticType.Fractional,
        Statistic = {
            Name = name,
            CurrentValue = currentValue,
            MaximumValue = maximumValue,
            PrimaryBarColor = primaryBarColor,
            SecondaryBarColor = secondaryBarColor
        }
    }

    return fractionalStatistic
end

function CoordinateStatistic(name, x, y, z)
    local coordinateStatistic = {
        Type = StatisticType.Coordinate,        
        Statistic = {
            Name = name,
            Format = "{0:0.000}",
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
            Statistics = statistics
        }
    }

    return statisticGroup
end