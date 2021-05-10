# Omnified Hacking Framework
[What Is Omnified?](https://badecho.com/index.php/what-is-omnified/)

This is the home of the Bad Echo Omnified framework. It consists of the common, game-neutral, code that greatly overhauls many aspects of a game, as well as the specific target games that have been Omnified. Please refer to my [hackpad](https://badecho.com) for details analysis and information regarding the Omnified process, my approach, and my methods.

## Hacking Workspace Structure

The Omnified hacking framework uses the following structure:

* **omnified.hacks.workspace**: Workspace file for Visual Studio Code, which is what I use to do all my work on the Omnified Hacking framework.
* **/framework/**: This contains all game-neutral, common Omnified framework code.
  * **Omnified.lua**: Omnified framework system code. This provides functionality that allows us to import in, load, and clean up the disparate bits of code that make up an Omnified hack, such as the framework and game-targeted assembly code. Many other system-wide functionalities are provided here as well, such as timers relating to status effects, etc.
  * **Omnified.asm**: The core Omnified framework assembly code. All central code that pertains to the Omnified game-neutral systems can be found here, such as the [Apocalypse system](https://badecho.com/index.php/2020/10/19/apocalypse-system/), the Predator system, the Abomnification system, etc. All Omnified games reference and have this code injected into the running processes of their targets.
  * **OmnifiedDisplay.lua**: Omnified framework display code, responsible for exporting hacked data for display on screen during stream — viewable now in the form of my the primitive Statistics and Apocalypse Log windows, but will also play a vital role in serializing data to be processed by my Vision application that’s currently in development.
* **/targets/**: This contains all Omnified targets, typically a game that I've hacked and have played through Omnified on my [stream](https://twitch.tv/omni). Each Omnified game will have its own subdirectory.
  * **/nameOfGame/**: This would be the directory for the game acting as an Omnified target, containing all the code specific to that game.
    * **Omnified_nameOfGame.ct**: The .CT file for the Omnified game. This is what we double click on and then check the check box in, in order to get the loaded game's process Omnified. This is a direct copy from the Omnified template .CT file containing all the boilerplate code required to load the framework and target hacking code. The only things specific to the game in this file are the game's data structures discovered through reverse engineering. 
    * **hooks.asm**: Game specific hooks for the target Omnified game. These generally consist of the initiation points for the various game-neutral Omnified systems. Sometimes they also include Omnified-flavored enhancements specific to that game as well.

## Where to Find Older Omnified Game Hacks

All games Omnified since the creation of this repository will appear here, in source control, as my active hacking workspace you may see me working on during streams is exactly what is getting pushed to this repository. I have no plans of putting any of the games I previously Omnified up into source control, as that would be a whole lot of work for very little return.

Not all is lost! Before this source repository was created, I previously was publishing all the Omnified source code for Omnified games on the [Tombstones](https://badecho.com/index.php/category/tombstones/) section of my [hackpad](https://badecho.com/).

For all Omnified games that were played and beaten prior to the creation of my [hackpad](https://badecho.com/), source code was posted for these in the **#tombestones** channel on my [Discord server](https://discord.gg/omni).

