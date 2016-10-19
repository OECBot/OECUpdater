import clr
import sys
from System.IO import Directory, Path

sys.path.append(Directory.GetCurrentDirectory())
clr.AddReferenceToFile('OECLib.dll')

from OECLib.WebRequests import WebRequest

def Initialize():
	return

def GetName():
	return "Exoplanet.EU"

def GetDescription():
	return "This plugin allows the extraction of data from the exoplanet.eu database"

def Run():
	webRequest = WebRequest()
	print webRequest.MakeRequest()
	print "{0}: {1}".format(GetName(), GetDescription())
