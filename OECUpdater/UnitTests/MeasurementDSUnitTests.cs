using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OECLib.Data;
using OECLib.Data.Measurements;
using System.Xml;

namespace UnitTests
{
    class MeasurementDSUnitTests
    {
        XmlDocument doc;

        [SetUp]
        protected void SetUp()
        {
            doc = new XmlDocument();
        }

        // NumberErrorMeasurement unit tests

        [TestCase("magB", 5.74, 0.02, 0.02)]
        public void CreateNumberErrorMeasurement(string name, double measurement, double errPlus, double errMinus)
        {
            NumberErrorMeasurement mUnit = new NumberErrorMeasurement(name, measurement, errPlus, errMinus);
        }

        [TestCase("magB", 5.74, 0.02, 0.02)]
        public void CheckNumberErrorMeasurementXML(string name, double measurement, double errPlus, double errMinus)
        {
            NumberErrorMeasurement mUnit = new NumberErrorMeasurement(name, measurement, errPlus, errMinus);
            XmlElement element = doc.CreateElement(mUnit.MeasurementName);
            element = mUnit.WriteXmlTag(element);
            Assert.AreEqual(measurement.ToString(), element.InnerText);
            Assert.AreEqual(errPlus.ToString(), element.Attributes["errorplus"].Value);
            Assert.AreEqual(errMinus.ToString(), element.Attributes["errorplus"].Value);
        }

        [TestCase("magB", 5.74, 0.02, 0.02)]
        public void CheckNumberErrorMeasurementGetValue(string name, double measurement, double errPlus, double errMinus)
        {
            NumberErrorMeasurement mUnit = new NumberErrorMeasurement(name, measurement, errPlus, errMinus);
            Assert.AreEqual(measurement, mUnit.getValue().value);
        }

        [TestCase("magB", 5.74, 0.02, 0.02, 5.75, 0.03, 0.03)]
        public void CheckNumberErrorMeasurementSetValue(string name, double measurement, double errPlus, double errMinus,
            double newMeasurement, double newErrPlus, double newErrMinus)
        {
            NumberErrorMeasurement mUnit = new NumberErrorMeasurement(name, measurement, errPlus, errMinus);
            mUnit.setValue(newMeasurement, newErrPlus, newErrMinus);
            Assert.AreEqual(newMeasurement, mUnit.getValue().value);
        }

        // NumberMeasurement unit tests

        [TestCase("magB", 5.74)]
        public void CreateNumberErrorMeasurement(string name, double measurement)
        {
            NumberMeasurement mUnit = new NumberMeasurement(name, measurement);
        }

        [TestCase("magB", 5.74)]
        public void CheckNumberErrorMeasurementXML(string name, double measurement)
        {
            NumberMeasurement mUnit = new NumberMeasurement(name, measurement);
            XmlElement element = doc.CreateElement(mUnit.MeasurementName);
            element = mUnit.WriteXmlTag(element);
            Assert.AreEqual(measurement.ToString(), element.InnerText);
        }

        [TestCase("magB", 5.74)]
        public void CheckNumberErrorMeasurementGetValue(string name, double measurement)
        {
            NumberMeasurement mUnit = new NumberMeasurement(name, measurement);
            Assert.AreEqual(measurement, mUnit.getValue().value);
        }

        [TestCase("magB", 5.74, 5.75)]
        public void CheckNumberErrorMeasurementSetValue(string name, double measurement, double newMeasurement)
        {
            NumberMeasurement mUnit = new NumberMeasurement(name, measurement);
            mUnit.setValue(newMeasurement);
            Assert.AreEqual(newMeasurement, mUnit.getValue().value);
        }
    }
}
