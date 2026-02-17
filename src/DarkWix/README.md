# Bad Echo DarkWiX UI Library
[![Discord](https://img.shields.io/discord/348353194801364992?style=flat-square&label=Discord&logo=discord&logoColor=white&color=7289DA)](https://discord.gg/omni) 

The standard Windows installer UI is too bright. It hurts my eyes! The `BadEcho.DarkWiX` library provides a darker themed UI that can be used with WiX.

This package exists to be used by specific Bad Echo applications, but is licensed under the GNU Affero General Public License so that others may enjoy it as well; see the accompanying [license](https://github.com/BadEcho/core/blob/master/LICENSE.md) for details.

## How to Use

To use this library, simply add a reference to it from your WiX installer project, and add the following declaration to your code:

```xml
<UIRef Id="DarkUI" />
```

## Customizing the Dialog Background

The standard `WixUIDialogBmp` WiX variable is overridden by this library with a dark-themed background, which affects the built-in Welcome and Exit dialogs. A project consuming this library can further override `WixDialogBmp` to use its own background for these dialogs.

Additionally, all other dialogs which normally do not have a background have been modified so they also show this dark background. Because the Welcome and Exit dialogs have less of their surface covered by text, there may be a desire to show a logo on those dialogs and not others. To achieve this, the background for each dialog can be configured by overridding the following localizable string elements:

```xml
<String Id="DarkInstallDirDlgBitmap"  Overridable="true" Value="BinaryBitmapId"/>
<String Id="DarkMaintenanceTypeDlgBitmap"  Overridable="true" Value="BinaryBitmapId"/>
<String Id="DarkProgressDlgBitmap"  Overridable="true" Value="BinaryBitmapId"/>
<String Id="DarkVerifyReadyDlgBitmap"  Overridable="true" Value="BinaryBitmapId"/>
```

Replace `BinaryBitmapId` with the ID for the `<Binary/>` of your choice.

## Customizing the Cancel Background

The tiny modal popup that appears when a user clicks the Cancel button also has a dark background applied to it. To customize this background, override the following localizable string element:

```xml
<String Id="DarkCancelDlgBitmap" Overridable="true" Value="BinaryBitmapId"
```

Once again, replace `BinaryBitmapId` with the ID for the `<Binary/>` of your choice.

## The Action Text

Windows installer is ancient tech; painting a bitmap over all the dialogs results in some noticeable artifacts. In particular, with fast-updating action text of the install progress dialog, the text (though transparent) will update faster than the bitmap can redraw itself. Because of this, this library just uses a static action text, settable by overriding:

```xml
<String Id="ActionText" Overridable="true" Value="Installing...something..." />
```

Probably unnecessary for installers with long running actions -- but this is what I need, and it's what we all get.

## About Bad Echo
Bad Echo is a collection of software technologies and [various writings](https://badecho.com) by Matt Weber: a software designer, partnered [Twitch](https://twitch.tv/omni) streamer, and game developer.

While Bad Echo code concerns itself a great deal with the "best approaches" to general software problems, it also focuses on game development and providing entertainment through the clever manipulation of games (the results of which are streamed by myself).

## Getting in Contact
I'm a partnered Twitch streamer, and talking to me during one of my [Omnified streams](https://twitch.tv/omni), on the off chance that I actually _am_ streaming, is most definitely the quickest way to get my attention!

You may also reach me [via email](mailto:matt@badecho.com).