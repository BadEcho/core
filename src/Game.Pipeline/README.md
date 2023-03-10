# Bad Echo Content Pipeline Extension
[![Discord](https://img.shields.io/discord/348353194801364992?style=flat-square&label=Discord&logo=discord&logoColor=white&color=7289DA)](https://discord.gg/omni) 

The `BadEcho.Game.Pipeline` library provides [Bad Echo](https://badecho.com) extensions to MonoGame's content pipeline.

This mainly revolves around the custom types of content baked into the **.xnb** files employed by Bad Echo gaming products.

This package exists to be used by Bad Echo games, but is licensed under the GNU Affero General Public License so that others may enjoy it as well; see the accompanying [license](https://github.com/BadEcho/core/blob/master/LICENSE.md) for details.

## Target Framework Version for Consuming Projects
This library targets the latest version of .NET (6.0), which will be problematic for consumers if they are building against either the release and prerelease versions of MonoGame.

The current release version of `MonoGame.Content.Builder.Task` will only support content pipeline extension libraries targeting .NET Core 3.1, whereas the current prerelease version of the aforementioned MSBuild task package supports only up to .NET 5.0.

In order to consume this pipeline extension, the very latest MonoGame sources must be utilized; as there is currently no published package that contains .NET 6.0 support, all consumers must either build from MonoGame's source themselves, or use [Bad Echo's experimental MonoGame package feed](https://www.myget.org/F/monogame-dev/api/v3/index.json).

## About Bad Echo
Bad Echo is a collection of software technologies and [various writings](https://badecho.com) by Matt Weber: a software designer, partnered [Twitch](https://twitch.tv/omni) streamer, and game developer.

While Bad Echo code concerns itself a great deal with the "best approaches" to general software problems, it also focuses on game development and providing entertainment through the clever manipulation of games (the results of which are streamed by myself).

## Getting in Contact
I'm a partnered Twitch streamer, and talking to me during one of my [Omnified streams](https://twitch.tv/omni), on the off chance that I actually _am_ streaming, is most definitely the quickest way to get my attention!

You may also reach me [via email](mailto:matt@badecho.com).