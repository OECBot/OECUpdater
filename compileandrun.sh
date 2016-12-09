#!/bin/bash

xbuild /p:Configuration=Release OECUpdater/OECUpdater.sln

rm -rf Release

mv OECUpdater/OECGUI/bin/Release Release

cd Release
mkdir -p Plugins

mv NASAExoplanetPlugin.dll Plugins/NASAExoplanetPlugin.dll
mv ExoplanetEU.dll Plugins/ExoplanetEU.dll

mono OECGUI.exe
