using System;
using System.Collections;
using System.Xml;

namespace OECLib.Data.Measurements
{
	public class MeasurementUnit
	{
		public String name { get; set; }
		public object value { get; set;}
		//public String type { get; set; }
		public Double errorPlus {get; set;}
		public Double errorMinus { get; set; }

		public MeasurementUnit (object value, Double errorplus=0.0, Double errorminus=0.0)
		{
			this.value = value;
			this.errorPlus = errorplus;
			this.errorMinus = errorminus;
		}

	}
}

