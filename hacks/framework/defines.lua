-- Configuration settings for the Omnified framework meant to be overridden in target .CT file registration routines.
-- Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
-- Copyright 2021 Bad Echo LLC

-- Coordinates are assumed to be floating point, unless this is set to true.
coordinatesAreDoubles = false

-- Exportation of player coordinate data requires these to be set to addresses in memory where coordinates are stored.
playerCoordinatesXAddress = "[example]+0x10"
playerCoordinatesYAddress = "[example]+0x14"
playerCoordinatesZAddress = "[example]+0x18"

-- Health values are assumed to be floating point, unless this is set to be true.
healthIsInteger = false

-- Exportation of player health requires these to be set to addresses in memory where coordinates are stored.
playerHealthAddress = "[exampleHealth]+0x14"
playerMaxHealthAddress = "[exampleHealth]+0x18"