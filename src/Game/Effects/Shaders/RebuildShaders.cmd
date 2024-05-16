@echo off

cd /d %~dp0
mgfxc AlphaSpriteEffect.fx AlphaSpriteEffect.ogl.mgfxo /profile:OpenGL
mgfxc AlphaSpriteEffect.fx AlphaSpriteEffect.dx11.mgfxo /profile:DirectX_11
mgfxc DistanceFieldFontEffect.fx DistanceFieldFontEffect.ogl.mgfxo /profile:OpenGL
mgfxc DistanceFieldFontEffect.fx DistanceFieldFontEffect.dx11.mgfxo /profile:DirectX_11
resource-creator . -f *.mgfxo -o ..\Shaders