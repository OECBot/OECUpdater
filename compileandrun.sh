#!/bin/bash

xbuild /p:Configuration=Release OECUpdater/OECUpdater.sln

mv OECUpdater/OECUpdater/bin/Release .

mono Release/OECUpdater.exe
