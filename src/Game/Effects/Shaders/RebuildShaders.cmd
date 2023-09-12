@echo off

cd /d %~dp0
mgfxc AlphaSpriteEffect.fx AlphaSpriteEffect.mgfxo /profile:OpenGL
mgfxc DistanceFieldFontEffect.fx DistanceFieldFontEffect.mgfxo /profile:OpenGL
resource-creator . -f *.mgfxo -o ..\Shaders