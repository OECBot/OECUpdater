using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib.Data.Measurements
{
    public class NumberErrorMeasurement : Measurement
    {
        double errPlus;
        double errMinus;
        double measurement;

        public NumberErrorMeasurement(string name, double measurement, double errPlus, double errMinus) : base(name)
        {
            this.errPlus = errPlus;
            this.errMinus = errMinus;
            this.measurement = measurement;
            GetMeasurementType = MeasurementType.NumberErrMeasurement;
        }

        public override XmlElement WriteXmlTag(XmlElement element)
        {
            element.InnerText = measurement.ToString();
            element.SetAttribute("errorplus", errPlus.ToString());
            element.SetAttribute("errorminus", errMinus.ToString());
            return element;
        }

		public override MeasurementUnit getValue(){
			return new MeasurementUnit (measurement, errPlus, errMinus);
		}
    }
}
