using System;
using OECLib.Data;
using OECLib.Data.Measurements;

namespace OECLib.Utilities
{
	public static class PlanetMerger
	{

        public static StellarObject FindName(StellarObject mergeFrom, StellarObject mergeTo)
        {
            if (mergeFrom.names.Equals(mergeTo.names)) return mergeTo;
            foreach(StellarObject m in mergeTo.children)
            {
                var result = FindName(mergeFrom, m);
                if (result != null) return result;
            }
            return null;
        }
        public static StellarObject Merge(StellarObject mergeFrom, StellarObject mergeTo)
        {

            StellarObject stellar = FindName(mergeFrom, mergeTo);

            if (stellar == null) return mergeFrom;
            mergeTo.ResetMeasurement();
            foreach(var m in mergeFrom.measurements)
            {
                mergeTo.measurements.Add(m.Key, m.Value);
            }
            return mergeTo;
        }	
	}
}

