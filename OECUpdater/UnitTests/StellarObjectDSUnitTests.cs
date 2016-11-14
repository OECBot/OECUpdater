using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OECLib.Data;
using OECLib.Data.Measurements;
using System.Xml;

namespace UnitTests
{
    [TestFixture]
    class StellarObjectDSUnitTests
    {
        private SolarSystem solarSystem;
        private Binary binary;
        private Star star;
        private Planet planet;
        //private XmlDocument doc;

        private NumberErrorMeasurement numErrMeasurement;
        private NumberMeasurement numMeasurement;
        private StringMeasurement strMeasurement;


        [SetUp]
        protected void SetUp()
        {
            solarSystem = new SolarSystem();
            binary = new Binary();
            star = new Star();
            planet = new Planet();
            //doc = new XmlDocument();

            numErrMeasurement = new NumberErrorMeasurement("magB", 5.74, 0.02, 0.03);
            numMeasurement = new NumberMeasurement("magJ", 2.943);
            strMeasurement = new StringMeasurement("spectraltype", "G8 III");
        }

        [Test]
        public void PlanetAddChild()
        {
            SolarSystem tempSolarSystem = new SolarSystem();
            bool addSolarSystem = planet.AddChild(tempSolarSystem);
            Assert.AreEqual(false, addSolarSystem);

            Binary tempBinary = new Binary();
            bool addBinary = planet.AddChild(tempBinary);
            Assert.AreEqual(false, addBinary);

            Star tempStar = new Star();
            bool addStar = planet.AddChild(tempStar);
            Assert.AreEqual(false, addStar);

            Planet tempPlanet = new Planet();
            bool addPlanet = planet.AddChild(tempPlanet);
            Assert.AreEqual(false, addPlanet);
        }

        [Test]
        public void CheckPlanetType()
        {
            Planet tempPlanet = new Planet();
            Assert.AreEqual(false, planet.IsASystem);
            Assert.AreEqual(false, planet.IsABinary);
            Assert.AreEqual(false, planet.IsAStar);
            Assert.AreEqual(true, planet.IsAPlanet);
        }

        [TestCase("spectraltype", "G8 III")]
        public void CheckPlanetAddStringMeasurement(string name, string measurement)
        {
            Planet tempPlanet = new Planet();
            tempPlanet.AddStringMeasurement(name, measurement);
            Assert.AreEqual(true, tempPlanet.measurements.ContainsKey(name));
            Assert.AreEqual(measurement, (string)tempPlanet.measurements[name].getValue().value);
        }

        [TestCase("magB", 5.74)]
        public void CheckPlanetAddNumberMeasurement(string name, double measurement)
        {
            Planet tempPlanet = new Planet();
            tempPlanet.AddNumberMeasurement(name, measurement);
            Assert.AreEqual(true, tempPlanet.measurements.ContainsKey(name));
            Assert.AreEqual(measurement, (double)tempPlanet.measurements[name].getValue().value);
        }

        [TestCase("magB", 5.74, 0.02, 0.02)]
        public void CheckPlanetAddNumberErrorMeasurement(string name, double measurement, double errPlus, double errMinus)
        {
            Planet tempPlanet = new Planet();
            tempPlanet.AddNumberErrorMeasurement(name, measurement, errPlus, errMinus);
            Assert.AreEqual(true, tempPlanet.measurements.ContainsKey(name));
            Assert.AreEqual(measurement, (double)tempPlanet.measurements[name].getValue().value);
            Assert.AreEqual(errPlus, tempPlanet.measurements[name].getValue().errorPlus);
            Assert.AreEqual(errMinus, tempPlanet.measurements[name].getValue().errorMinus);
        }

        [Test]
        public void CheckPlanetAddMeasurement()
        {
            Planet tempPlanet = new Planet();
            tempPlanet.AddMeasurement(numErrMeasurement);
            Assert.AreEqual(true, tempPlanet.measurements.ContainsKey("magB"));
            Assert.AreEqual(5.74, (double)tempPlanet.measurements["magB"].getValue().value);
            Assert.AreEqual(0.02, (double)tempPlanet.measurements["magB"].getValue().errorPlus);
            Assert.AreEqual(0.03, (double)tempPlanet.measurements["magB"].getValue().errorMinus);

            tempPlanet.AddMeasurement(numMeasurement);
            Assert.AreEqual(true, tempPlanet.measurements.ContainsKey("magJ"));
            Assert.AreEqual(2.943, (double)tempPlanet.measurements["magJ"].getValue().value);

            tempPlanet.AddMeasurement(strMeasurement);
            Assert.AreEqual(true, tempPlanet.measurements.ContainsKey("spectraltype"));
            Assert.AreEqual("G8 III", (string)tempPlanet.measurements["spectraltype"].getValue().value);
        }

        [Test]
        public void CheckPlanetAddMeasurementsTag()
        {
            Planet tempPlanet = new Planet();
            XmlDocument doc = new XmlDocument();
            tempPlanet.AddMeasurement(numErrMeasurement);
            tempPlanet.AddMeasurement(numMeasurement);
            tempPlanet.AddMeasurement(strMeasurement);
            XmlElement element = tempPlanet.XMLTag(doc);

            Assert.AreEqual("magB", element.ChildNodes[0].Name);
            Assert.AreEqual("5.74", element.ChildNodes[0].InnerText);
            Assert.AreEqual("0.02", element.ChildNodes[0].Attributes[0].InnerText);
            Assert.AreEqual("0.03", element.ChildNodes[0].Attributes[1].InnerText);

            Assert.AreEqual("magJ", element.ChildNodes[1].Name);
            Assert.AreEqual("2.943", element.ChildNodes[1].InnerText);

            Assert.AreEqual("spectraltype", element.ChildNodes[2].Name);
            Assert.AreEqual("G8 III", element.ChildNodes[2].InnerText);
        }

    }
}
