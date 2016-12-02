#!/bin/bash

xbuild /p:Configuration=Release OECUpdater/OECUpdater.sln

mv OECUpdater/OECGUI/bin/Release Release

cd Release
mono OECGUI.exe
