# Omnified Hacking Framework
[What Is Omnified?](https://badecho.com/index.php/what-is-omnified/)

This is the home of the Bad Echo Omnified framework. It consists of the common, game-neutral, code that greatly overhauls many aspects of a game, as well as the specific target games that have been Omnified. Please refer to my [hackpad](https://badecho.com) for details analysis and information regarding the Omnified process, my approach, and my methods.

## Hacking Workspace Structure

The Omnified hacking framework uses the following structure:

* **omnified.hacks.workspace**: Workspace file for Visual Studio Code, which is what I use to do all my work on the Omnified Hacking framework.
* **/framework/**: This contains all game-neutral, common Omnified framework code.
  * **defines.lua**: Configuration settings meant to be overridden in target .CT file registration routines.
  * **utility.lua**: Utility functions employed by the Omnified framework.
  * **messaging.lua**: Library for messages sent by the Omnified framework.
  * **apocalypseMessages.lua**: Defines the Omnified Apocalypse event messaging schema.
  * **statisticMessages.lua**: Defines the Omnified game statistic messaging schema.
  * **omnified.lua**: Omnified framework system code. This provides functionality that allows us to import in, load, and clean up the disparate bits of code that make up an Omnified hack, such as the framework and game-targeted assembly code. Many other system-wide functionalities are provided here as well, such as timers relating to status effects, etc.
  * **omnified.asm**: The core Omnified framework assembly code. All central code and symbols that Omnified game-neutral systems are dependent on can be found here, such as general-purpose functions like random number generation and safe pointer checking. 
  * **template.ct**: This is a template .CT file that is used as the base for all newly targeted Omnified games.
  * **/systems/**: This contains all of the various Omnified game-neutral systems.
    * **apocalypse.asm**: The assembly code for the [Apocalypse system](https://badecho.com/index.php/2020/10/19/apocalypse-system/), a process that completely overhauls the system in a game responsible for handling damage dealt to the player.
    * **predator.asm**: The assembly code for the [Predator system](https://badecho.com/index.php/2021/06/18/predator-system/), a process that makes movement of enemies deadlier through intelligent, and sometimes quite large, boosts to their speed.
    * **abomnification.asm**: The assembly code for the Abomnification system, a process that causes creatures to randomly change their shape and size in a manner reminiscent to a bad acid trip.
* **/targets/**: This contains all Omnified targets, typically a game that I've hacked and have played through Omnified on my [stream](https://twitch.tv/omni). Each Omnified game will have its own subdirectory. Games appearing here are referred to as being
Omnified 2.0.
  * **/nameOfGame/**: This would be the directory for the game acting as an Omnified target, containing all the code specific to that game.
    * **omnified_nameOfGame.ct**: The .CT file for the Omnified game. This is what we double click on and then check the check box in, in order to get the loaded game's process Omnified. This is a direct copy from the Omnified template .CT file containing all the boilerplate code required to load the framework and target hacking code. The only things specific to the game in this file are the game's data structures discovered through reverse engineering. 
    * **hooks.asm**: Game specific hooks for the target Omnified game. These generally consist of the initiation points for the various game-neutral Omnified systems. Sometimes they also include Omnified-flavored enhancements specific to that game as well.
* **/legacyTargets/**: This is a historical archive of all Omnified games that were created prior to the advent of Omnified 2.0. More information about these targets can be found on the **legacyTargets** page in source control.

## Getting More Information About Omnified Games

Previous playthroughs can be seen in their entirety on my [YouTube](https://www.youtube.com/omniTTV) and the VODs available on my [Twitch stream](https://twitch.tv/omni). You can also find a variety of articles regarding the hacking/Omnification of many the games on the [Bad Echo website](https://badecho.com/index.php/category/games/).

For all games that were created and beaten following the creation of my [hackpad](https://badecho.com), you can find a write-up/summary of the playthrough in the [Tombstones](https://badecho.com/index.php/category/tombstones/) section.

For all Omnified games that were played and beaten prior to the creation of my [hackpad](https://badecho.com/), you can find their tombstones in the **#tombstones** channel on my [Discord server](https://discord.gg/omni).
