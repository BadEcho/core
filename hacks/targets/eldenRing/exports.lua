--[[
    Exported configuration settings for Omnified Elden Ring.
    Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
    Copyright 2022 Bad Echo LLC
    
    Bad Echo Technologies are licensed under the
    GNU Affero General Public License v3.0.
    
    See accompanying file LICENSE.md or a copy at:
    https://www.gnu.org/licenses/agpl-3.0.html
--]]

require("statisticMessages")

function registerExports()
    -- Our custom stats.
    additionalStatistics = function()
        local runes = toInt(readInteger("[playerGameData]+0x6C"))

        return {
            WholeStatistic("Runes (XP)", runes)
        }
    end
end

function unregisterExports()

end