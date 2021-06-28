-- General-purpose Omnified statistics message definitions.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC


require('Omnified')

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