import clr
import sys
from System.IO import Directory, Path

sys.path.append(Directory.GetCurrentDirectory())
clr.AddReferenceToFile('OECLib.dll')

from OECLib.WebRequests import WebRequest


def Initialize():
	return

def GetName():
	return "Exoplanet.NASA"

def GetDescription():
	return "I'm too lazy to write this one so just pretend this is a description."

def Run():
	webRequest = WebRequest()
	webRequest.MakeRequest()
	print "{0}: {1}".format(GetName(), GetDescription())
