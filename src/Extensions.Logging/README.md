# Bad Echo Logging Extensions
[![Discord](https://img.shields.io/discord/348353194801364992?style=flat-square&label=Discord&logo=discord&logoColor=white&color=7289DA)](https://discord.gg/omni) 

The `BadEcho.Extensions.Logging` library adds support for capturing Bad Echo library events with loggers configured using the Microsoft.Extensions.Logging framework. Bad Echo class libraries log diagnostic events the same way the .NET runtime does: by writing to an `EventSource`.

Dependency injection (a requirement for Microsoft.Extension.Logging) is an application concern, not a library concern. Trying to inject an `ILogger` instance into all the places a class library wishes to emit messages (a common bit of misadvice found on various Developer Q&A platforms) is to descend into madness.

This library provides extension methods which will forward log messages from Bad Echo event sources to `ILogger` instances created from a provided `ILoggerFactory`, an approach used by Microsoft for several of their products. This keeps the library sane while giving the application control over where these messages go.

This package exists to be used by specific Bad Echo applications, but is licensed under the GNU Affero General Public License so that others may enjoy it as well; see the accompanying [license](https://github.com/BadEcho/core/blob/master/LICENSE.md) for details.

## About Bad Echo
Bad Echo is a collection of software technologies and [various writings](https://badecho.com) by Matt Weber: a software designer, partnered [Twitch](https://twitch.tv/omni) streamer, and game developer.

While Bad Echo code concerns itself a great deal with the "best approaches" to general software problems, it also focuses on game development and providing entertainment through the clever manipulation of games (the results of which are streamed by myself).

## Getting in Contact
I'm a partnered Twitch streamer, and talking to me during one of my [Omnified streams](https://twitch.tv/omni), on the off chance that I actually _am_ streaming, is most definitely the quickest way to get my attention!

You may also reach me [via email](mailto:matt@badecho.com).