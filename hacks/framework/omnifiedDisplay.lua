--[[
	Omnified Display
    Written By: Matt Weber (https://badecho.com) (https://twitch.tv/omni)
    Copyright 2022 Bad Echo LLC
    
    Bad Echo Technologies are licensed under the
    GNU Affero General Public License v3.0.
    
    See accompanying file LICENSE.md or a copy at:
    https://www.gnu.org/licenses/agpl-3.0.html
--]]

require("omnified")

function FloorIt(number)
	if number ~= nil then
		return math.floor(number)
	end

	return nil
end

function activateLoggers()	

	if loggersTimer == nil then
		loggersTimer = createTimer(getMainForm())
	end

	loggersTimer.Interval = 400
	loggersTimer.OnTimer = function()


		local playerHealth = not healthIsInteger
								and toInt(readFloat(playerHealthAddress))
								or readInteger(playerHealthAddress)

		local playerMaxHealth = not healthIsInteger
								and toInt(readFloat(playerMaxHealthAddress))
								or readInteger(playerMaxHealthAddress)      
		
		local enemyHealth = readFloat("lastEnemyHealthValue")

		enemyHealth = FloorIt(enemyHealth)
		
		local deathcounter = assert(io.open("\\\\parsec\\c$\\streamData\\deathcount-synced.txt","w"))		
		
		local playerDeaths = readInteger("playerDeaths")
		
		if playerDeaths ~= nil then
			deathcounter:write("Deaths: ", playerDeaths)
		end
		
		deathcounter:close()	
		
		local newEnemyDamageEventBonusNotProcessed = readInteger("newEnemyDamageEventBonusNotProcessed")

		if newEnemyDamageEventBonusNotProcessed ~= nil and newEnemyDamageEventBonusNotProcessed == 1 then
			local newEnemyDamageEventBonusAmount = readFloat("newEnemyDamageEventBonusAmount")

			if newEnemyDamageEventBonusAmount ~= nil then
				writeFloat("lastEnemyDamageEventBonusAmount", newEnemyDamageEventBonusAmount)			
			end
		
			local newEnemyDamageEventBonusX = readFloat("newEnemyDamageEventBonusX")

			if newEnemyDamageEventBonusX ~= nil then
				writeFloat("lastEnemyDamageEventBonusX", newEnemyDamageEventBonusX)
			end

			writeInteger("newEnemyDamageEventBonusNotProcessed", 0)
		end

		local lastDamageToPlayer = toInt(readFloat("lastDamageToPlayer"))		
		local lastEnemyDamageEvent = toInt(readFloat("lastEnemyDamageEvent"))	
		local lastEnemyDamageEventBonusAmount = toInt(readFloat("lastEnemyDamageEventBonusAmount"))		
		local lastEnemyDamageEventBonusX = round(readFloat("lastEnemyDamageEventBonusX"),1)

		local log = assert(io.open("log.txt", "a"))

		local ts = os.time()
		local timestamp = os.date('%H:%M-', ts)

		local logEntryEnemyRoll = "Enemy rolls a"
		local logEntryPlayerData
			= "damage!\nPlayer now has"

		local logApocalypse
			= readInteger("logApocalypse")

		local apocalypseResult
			= readInteger("apocalypseResult")

		local riskOfMurderResult
			= readInteger("riskOfMurderResult")
			
		local fatalisResult
			= readInteger("fatalisResult")
			
		local fatalisResultUpper
			= readInteger("fatalisResultUpper")
			
		local fatalisState
			= readInteger("fatalisState")

		local extraDamageX
			= readFloat("extraDamageX")
			
		local lastVerticalDisplacement
			= readFloat("lastVerticalDisplacement")
			
		if fatalisState == 2 then
			log:write(timestamp, "Miraculously, the player has been cured of Fatalis!\n")
			playSound(findTableFile('fatalisCured.wav'))
			writeInteger("fatalisState", 0)
		end

		if logApocalypse == 1 and apocalypseResult ~= nil
							  and lastDamageToPlayer ~= nil
							  and playerHealth ~= nil
							  and timestamp ~= nil
							  and extraDamageX ~= nil
							  and fatalisResult ~= nil
							  and fatalisResultUpper ~= nil
							  and fatalisState ~= nil
							  then
		
			if fatalisState == 1 and fatalisResult == 0 then
				log:write(timestamp, "The player, afflicted with Fatalis, dies immediately from the enemy attack!\n")

				local randomFatalis = {
					{"wilhelm.wav", 1},
					{"fatalisDeath.wav", 2},
					{"haha.wav", 1}
				}

				local fatalisSound = randomize(randomFatalis)

				playSound(findTableFile(fatalisSound))						
			else
				local apocalypseEnemyRoll =
					string.format("%s%s %i: ", timestamp,
											   logEntryEnemyRoll,
											   apocalypseResult)

				local apocalypseDamagedHealth =
					string.format("%.0f %s %.0f health.\n", lastDamageToPlayer,
															logEntryPlayerData,
															playerHealth)

				if apocalypseResult >= 1 and apocalypseResult <= 4 then
					log:write(apocalypseEnemyRoll,
							  extraDamageX,
							  "x DAMAGE causing ",
							  apocalypseDamagedHealth)
				elseif apocalypseResult == 5 or apocalypseResult == 6 then
					log:write(apocalypseEnemyRoll,
							  "SUDDEN TELEPORTITIS (",
							  lastVerticalDisplacement,
							  ") causing ",
							  apocalypseDamagedHealth)
					if lastVerticalDisplacement > freeFallThreshold then
						playSound(findTableFile('freefallin.wav'))
					end	
				elseif apocalypseResult >= 7 and apocalypseResult <= 9 then
					log:write(apocalypseEnemyRoll,
							  "RISK OF MURDER!\n")

					local riskOfMurderEnemyRoll =
						string.format("%s%s %i: ", timestamp,
												   logEntryEnemyRoll,
												   riskOfMurderResult)

					if riskOfMurderResult <= 3 then
						log:write(riskOfMurderEnemyRoll,
								  "WHEW! Just normal damage causing ",
								  apocalypseDamagedHealth)
						if fatalisResult == fatalisResultUpper then
							log:write(timestamp, "Unfortunately the player has been stricken with Fatalis for an unknown period of time...\n")						
							playSound(findTableFile('fatalisAfflication.wav'))
							writeInteger("fatalisResult",0)
						end								  
					else
						log:write(riskOfMurderEnemyRoll,
								  "HOLY SHIT! Player has been SIXTY NINED causing ",
								  apocalypseDamagedHealth)

								  local randomSixtyNine = {									  
									  {"headshot.wav", 2},
									  {"Holyshit.wav", 2},
									  {"vincentLaugh.wav", 1}
								  }

								  local sixtyNineSound = randomize(randomSixtyNine)

								  playSound(findTableFile(sixtyNineSound))
					end
				else				
						log:write(apocalypseEnemyRoll,
								  "WOW! Player achieves orgasm!\nPlayer is healed fully to ",
								  playerHealth,
								  " health.\n")

								  local randomOrgasm = {
									  {"tuturu.wav", 1},
									  {"Wow.wav", 1}
								  }

								  local orgasmSound = randomize(randomOrgasm)

								  playSound(findTableFile(orgasmSound))
				end

			end

			writeInteger("logApocalypse", 0)				

		end

		local logPlayerCrit = readInteger("logPlayerCrit")

		if logPlayerCrit == 1 and lastEnemyDamageEvent ~= nil and lastEnemyDamageEventBonusX ~= nil and lastEnemyDamageEventBonusAmount ~= nil then		

			local playerCritDamageResult = readInteger("playerCritDamageResult")
			local playerCritDamageResultUpper = readInteger("playerCritDamageResultUpper")
			local playerCritDamageResultLower = readInteger("playerCritDamageResultLower")

			if areNotNil(playerCritDamageResult, playerCritDamageResultUpper, playerCritDamageResultLower) then
				log:write(timestamp,
						  "Enemy has been critically hit (",
						  lastEnemyDamageEventBonusX,
						  "x) causing an additional ",
						  lastEnemyDamageEventBonusAmount,
						  " damage!\n")
				
				local critSound = nil

				local critDamageRange = playerCritDamageResultUpper - playerCritDamageResultLower

				local critHighDamageMinimum = playerCritDamageResultUpper - (critDamageRange * (1/3))

				if (playerCritDamageResult >= critHighDamageMinimum) then
					critSound = "mudboneCrit.wav"
				else
					local randomLowCrit = {
						{"chocobo.wav", 2},
						{"comboBreaker.wav", 1}
					}

					critSound = randomize(randomLowCrit)
				end

				playSound(findTableFile(critSound))
			end
			
			writeInteger("logPlayerCrit", 0)
		end

		local logKamehameha = readInteger("logKamehameha")
		local gokuDamageX = readFloat("gokuDamageX")

		if logKamehameha == 1 and lastEnemyDamageEvent ~= nil
							  and lastEnemyDamageEventBonusAmount ~= nil
							  and lastEnemyDamageEventBonusX ~= nil
							  and timestamp ~= nil then
			log:write(timestamp,
					  "Player has unlocked his inner Goku and performs a devastating KAMEHAMEHAAAAA attack causing ",
					  lastEnemyDamageEventBonusX,
					  "x extra damage and resulting in an additional ",
					  lastEnemyDamageEventBonusAmount,
						" damage!\n")
				playSound(findTableFile('kame.wav'))
				writeInteger("logKamehameha", 0)
		end

		log:close()
	end

end

function deactivateLoggers()
  if loggersTimer ~= nil then
	loggersTimer.Enabled = false
	loggersTimer.Destroy()
	loggersTimer = nil
  end

end