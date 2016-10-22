import clr
import sys
from System.IO import Directory, Path

sys.path.append(Directory.GetCurrentDirectory())
clr.AddReferenceToFile('OECLib.dll')

from OECLib.HTTPRequests import HTTPRequest

def Initialize():
	return

def GetName():
	return "Exoplanet.EU"

def GetDescription():
	return "This plugin allows the extraction of data from the exoplanet.eu database"

def Run():
	webRequest = HTTPRequest()
	print webRequest.RequestAsString("www.google.ca")
	print "{0}: {1}".format(GetName(), GetDescription())
