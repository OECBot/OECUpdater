#!/bin/bash

xbuild /p:Configuration=Debug OECUpdater/OECUpdater.sln

mv OECUpdater/OECGUI/bin/Debug .
cd Debug
mono OECGUI.exe
