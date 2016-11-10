using System;
using OECLib.Data;
using OECLib.Data.Measurements;

namespace OECLib.Utilities
{
	public static class PlanetMerger
	{
		public static StellarObject Merge(StellarObject mergeFrom, StellarObject mergeTo) {
			foreach (StellarObject fromChild in mergeFrom.children) {
				foreach (StellarObject toChild in mergeTo) {
					
				}
			}
			foreach(Measurement measure in mergeFrom.measurements.Values) {
				
			}
		}

	}
}

