#!/bin/bash

xbuild /p:Configuration=Debug OECUpdater/OECUpdater.sln

cd Debug
mono OECGUI.exe
