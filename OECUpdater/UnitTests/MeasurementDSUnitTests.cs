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
            Assert.AreEqual(newMeasurement, (double)mUnit.getValue().value);
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
            Assert.AreEqual(newMeasurement, (double)mUnit.getValue().value);
        }

        // StringMeasurement unit tests

        [TestCase("lastupdate", "15/09/20")]
        public void CreateStringMeasurement(string name, string measurement)
        {
            StringMeasurement mUnit = new StringMeasurement(name, measurement);
        }

        [TestCase("lastupdate", "15/09/20")]
        public void CheckStringMeasurementXML(string name, string measurement)
        {
            StringMeasurement mUnit = new StringMeasurement(name, measurement);
            XmlElement element = doc.CreateElement(mUnit.MeasurementName);
            element = mUnit.WriteXmlTag(element);
            Assert.AreEqual(measurement.ToString(), element.InnerText);
        }

        [TestCase("lastupdate", "15/09/20")]
        public void CheckStringMeasurementGetValue(string name, string measurement)
        {
            StringMeasurement mUnit = new StringMeasurement(name, measurement);
            Assert.AreEqual(measurement, mUnit.getValue().value);
        }

        [TestCase("lastupdate", "15/09/20", "16/10/21")]
        public void CheckSStringMeasurementSetValue(string name, string measurement, string newMeasurement)
        {
            StringMeasurement mUnit = new StringMeasurement(name, measurement);
            mUnit.setValue(newMeasurement);
            Assert.AreEqual(newMeasurement, (string)mUnit.getValue().value);
        }

        // MeasurementUnit unit tests

        [TestCase("12 20 43")]
        [TestCase(5.74)]
        public void CreateMeasurementUnit1Input(object value)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value);
        }

        [TestCase("12 20 43", 0.02)]
        [TestCase(5.74, 0.02)]
        public void CreateMeasurementUnit2Input(object value, double errorplus)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value, errorplus);
        }

        [TestCase("12 20 43", 0.02, 0.03)]
        [TestCase(5.74, 0.02, 0.03)]
        public void CreateMeasurementUnit3Input(object value, double errorplus, double errorminus)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value, errorplus, errorminus);
        }

        [TestCase("12 20 43")]
        public void CheckMeasurementUnitStrValue1Input(object value)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value);
            Assert.AreEqual((string)value, (string)mUnit.value);
        }

        [TestCase("12 20 43", 0.02)]
        public void CheckMeasurementUnitStrValue2Input(object value, double errorplus)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value, errorplus);
            Assert.AreEqual((string)value, (string)mUnit.value);
        }

        [TestCase("12 20 43", 0.02, 0.03)]
        public void CheckMeasurementUnitStrValue3Input(object value, double errorplus, double errorminus)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value, errorplus, errorminus);
            Assert.AreEqual((string)value, (string)mUnit.value);
        }

        [TestCase(5.74)]
        public void CheckMeasurementUnitDoubleValue1Input(object value)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value);
            Assert.AreEqual((double)value, (double)mUnit.value);
        }

        [TestCase(5.74, 0.02)]
        public void CheckMeasurementUnitDoubleValue2Input(object value, double errorplus)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value, errorplus);
            Assert.AreEqual((double)value, (double)mUnit.value);
        }

        [TestCase(5.74, 0.02, 0.03)]
        public void CheckMeasurementUnitDoubleValue3Input(object value, double errorplus, double errorminus)
        {
            MeasurementUnit mUnit = new MeasurementUnit(value, errorplus, errorminus);
            Assert.AreEqual((double)value, (double)mUnit.value);
        }
    }
}
