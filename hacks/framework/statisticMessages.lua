-- General-purpose Omnified statistic message definitions.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC


require("omnified")

StatType = define_enum {
    "Whole",
    "Fractional",
    "Coordinate"
}

function createWholeStat(name, value)

end

function createFractionalStat(name, currentValue, maximumValue)

end

function createCoordinateStat(name, x, y, z)

end