--[[
    Exported configuration settings for Omnified Nioh 2.
    Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
    Copyright 2022 Bad Echo LLC
    
    Bad Echo Technologies are licensed under the
    GNU Affero General Public License v3.0.
    
    See accompanying file LICENSE.md or a copy at:
    https://www.gnu.org/licenses/agpl-3.0.html
--]]

require("statisticMessages")

function registerExports()
    -- Custom statistics.  
    additionalStatistics = function()
        local playerAmrita = toInt(readInteger("playerAmrita"))

        return { 
            WholeStatistic("Amrita (XP)", playerAmrita)
        }
    end
end

function unregisterExports()

end