#!/bin/bash 
set -e

if [ `uname -s` = "Darwin" ]
then
	wget https://download.mono-project.com/archive/4.6.1/macos-10-universal/MonoFramework-MDK-4.6.1.5.macos10.xamarin.universal.pkg
	installer -pkg MonoFramework-MDK-4.6.1.5.macos10.xamarin.universal.pkg -target /
fi

if [ -f /etc/redhat-release ]
then
	yum install yum-utils
	rpm --import "https://pgp.mit.edu/pks/lookup?op=get&search=0x3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF"
	yum-config-manager --add-repo http://download.mono-project.com/repo/centos/
	yum install upgrade
	yum install mono-devel
fi

if [ -f /etc/debian_version ]
then
	apt-key adv --keyserver "hkp://pgp.mit.edu:80" --recv-keys "3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF"
	echo "deb http://download.mono-project.com/repo/debian wheezy main" | tee /etc/apt/sources.list.d/mono-xamarin.list
	apt-get update
	apt-get install mono-devel
fi
