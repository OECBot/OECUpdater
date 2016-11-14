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
        //private SolarSystem solarSystem;
        //private Binary binary;
        //private Star star;
        //private Planet planet;

        private NumberErrorMeasurement numErrMeasurement;
        private NumberMeasurement numMeasurement;
        private StringMeasurement strMeasurement;


        [SetUp]
        protected void SetUp()
        {
            //solarSystem = new SolarSystem();
            //binary = new Binary();
            //star = new Star();
            //planet = new Planet();

            numErrMeasurement = new NumberErrorMeasurement("magB", 5.74, 0.02, 0.03);
            numMeasurement = new NumberMeasurement("magJ", 2.943);
            strMeasurement = new StringMeasurement("spectraltype", "G8 III");
        }

        // Unit tests for Planet

        [Test]
        public void PlanetAddChild()
        {
            Planet currentPlanet = new Planet();

            SolarSystem tempSolarSystem = new SolarSystem();
            bool addSolarSystem = currentPlanet.AddChild(tempSolarSystem);
            Assert.AreEqual(false, addSolarSystem);

            Binary tempBinary = new Binary();
            bool addBinary = currentPlanet.AddChild(tempBinary);
            Assert.AreEqual(false, addBinary);

            Star tempStar = new Star();
            bool addStar = currentPlanet.AddChild(tempStar);
            Assert.AreEqual(false, addStar);

            Planet tempPlanet = new Planet();
            bool addPlanet = currentPlanet.AddChild(tempPlanet);
            Assert.AreEqual(false, addPlanet);
        }

        [Test]
        public void CheckPlanetType()
        {
            Planet tempPlanet = new Planet();
            Assert.AreEqual(false, tempPlanet.IsASystem);
            Assert.AreEqual(false, tempPlanet.IsABinary);
            Assert.AreEqual(false, tempPlanet.IsAStar);
            Assert.AreEqual(true, tempPlanet.IsAPlanet);
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
        public void CheckPlanetXMLTag()
        {
            Planet tempPlanet = new Planet();
            XmlDocument doc = new XmlDocument();
            tempPlanet.AddMeasurement(numErrMeasurement);
            tempPlanet.AddMeasurement(numMeasurement);
            tempPlanet.AddMeasurement(strMeasurement);
            XmlElement element = tempPlanet.XMLTag(doc);

            Assert.AreEqual("planet", element.Name);

            Assert.AreEqual("magB", element.ChildNodes[0].Name);
            Assert.AreEqual("5.74", element.ChildNodes[0].InnerText);
            Assert.AreEqual("0.02", element.ChildNodes[0].Attributes[0].InnerText);
            Assert.AreEqual("0.03", element.ChildNodes[0].Attributes[1].InnerText);

            Assert.AreEqual("magJ", element.ChildNodes[1].Name);
            Assert.AreEqual("2.943", element.ChildNodes[1].InnerText);

            Assert.AreEqual("spectraltype", element.ChildNodes[2].Name);
            Assert.AreEqual("G8 III", element.ChildNodes[2].InnerText);
        }

        // Unit tests for Star

        [Test]
        public void StarAddChild()
        {
            Star currentStar = new Star();

            SolarSystem tempSolarSystem = new SolarSystem();
            bool addSolarSystem = currentStar.AddChild(tempSolarSystem);
            Assert.AreEqual(false, addSolarSystem);

            Binary tempBinary = new Binary();
            bool addBinary = currentStar.AddChild(tempBinary);
            Assert.AreEqual(false, addBinary);

            Star tempStar = new Star();
            bool addStar = currentStar.AddChild(tempStar);
            Assert.AreEqual(false, addStar);

            Planet tempPlanet = new Planet();
            bool addPlanet = currentStar.AddChild(tempPlanet);
            Assert.AreEqual(true, addPlanet);
        }

        [Test]
        public void CheckStarType()
        {
            Star tempStar = new Star();
            Assert.AreEqual(false, tempStar.IsASystem);
            Assert.AreEqual(false, tempStar.IsABinary);
            Assert.AreEqual(true, tempStar.IsAStar);
            Assert.AreEqual(false, tempStar.IsAPlanet);
        }

        [TestCase("spectraltype", "G8 III")]
        public void CheckStarAddStringMeasurement(string name, string measurement)
        {
            Star tempStar = new Star();
            tempStar.AddStringMeasurement(name, measurement);
            Assert.AreEqual(true, tempStar.measurements.ContainsKey(name));
            Assert.AreEqual(measurement, (string)tempStar.measurements[name].getValue().value);
        }

        [TestCase("magB", 5.74)]
        public void CheckStarAddNumberMeasurement(string name, double measurement)
        {
            Star tempStar = new Star();
            tempStar.AddNumberMeasurement(name, measurement);
            Assert.AreEqual(true, tempStar.measurements.ContainsKey(name));
            Assert.AreEqual(measurement, (double)tempStar.measurements[name].getValue().value);
        }

        [TestCase("magB", 5.74, 0.02, 0.02)]
        public void CheckStarAddNumberErrorMeasurement(string name, double measurement, double errPlus, double errMinus)
        {
            Star tempStar = new Star();
            tempStar.AddNumberErrorMeasurement(name, measurement, errPlus, errMinus);
            Assert.AreEqual(true, tempStar.measurements.ContainsKey(name));
            Assert.AreEqual(measurement, (double)tempStar.measurements[name].getValue().value);
            Assert.AreEqual(errPlus, tempStar.measurements[name].getValue().errorPlus);
            Assert.AreEqual(errMinus, tempStar.measurements[name].getValue().errorMinus);
        }

        [Test]
        public void CheckStarAddMeasurement()
        {
            Star tempStar = new Star();
            tempStar.AddMeasurement(numErrMeasurement);
            Assert.AreEqual(true, tempStar.measurements.ContainsKey("magB"));
            Assert.AreEqual(5.74, (double)tempStar.measurements["magB"].getValue().value);
            Assert.AreEqual(0.02, (double)tempStar.measurements["magB"].getValue().errorPlus);
            Assert.AreEqual(0.03, (double)tempStar.measurements["magB"].getValue().errorMinus);

            tempStar.AddMeasurement(numMeasurement);
            Assert.AreEqual(true, tempStar.measurements.ContainsKey("magJ"));
            Assert.AreEqual(2.943, (double)tempStar.measurements["magJ"].getValue().value);

            tempStar.AddMeasurement(strMeasurement);
            Assert.AreEqual(true, tempStar.measurements.ContainsKey("spectraltype"));
            Assert.AreEqual("G8 III", (string)tempStar.measurements["spectraltype"].getValue().value);
        }

        [Test]
        public void CheckStarXMLTag()
        {
            Star tempStar = new Star();
            XmlDocument doc = new XmlDocument();
            tempStar.AddMeasurement(numErrMeasurement);
            tempStar.AddMeasurement(numMeasurement);
            tempStar.AddMeasurement(strMeasurement);
            XmlElement element = tempStar.XMLTag(doc);

            Assert.AreEqual("star", element.Name);

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
