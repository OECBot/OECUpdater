using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NUnit.Framework;
using OECLib.Data;
using OECLib.Data.Measurements;
using OECLib.Utilities;
using System.Xml;

namespace UnitTests
{
    [TestFixture]
    class SerializerUnitTests
    {
        string filePath = "OECUpdater/UnitTests/bin/Debug/testPlanet.xml";
        string filePathIn = "OECUpdater/UnitTests/bin/Debug/testSolarSystemIn.xml";
        string filePathOut = "OECUpdater/UnitTests/bin/Debug/testSolarSystemOut.xml";

        SolarSystem solarSystem;
        private NumberErrorMeasurement numErrMeasurement;
        private NumberMeasurement numMeasurement;
        private StringMeasurement strMeasurement;

        [SetUp]
        protected void SetUp()
        {
            solarSystem = new SolarSystem();
            numErrMeasurement = new NumberErrorMeasurement("magB", 5.74, 0.02, 0.03);
            numMeasurement = new NumberMeasurement("magJ", 2.943);
            strMeasurement = new StringMeasurement("spectraltype", "G8 III");

            solarSystem.AddMeasurement(numErrMeasurement);
            solarSystem.AddMeasurement(numMeasurement);
            solarSystem.AddMeasurement(strMeasurement);
        }

        [Test]
        public void CheckDeserializerConstructor()
        {
            XMLDeserializer deserializer = new XMLDeserializer(filePath);
        }

        [Test]
        public void CheckParseXML()
        {
            XMLDeserializer deserializer = new XMLDeserializer(filePath);
            StellarObject generated = deserializer.ParseXML();

            Assert.AreEqual(true, generated.measurements.ContainsKey("magB"));
            Assert.AreEqual(5.74, (double)generated.measurements["magB"].getValue().value);
            Assert.AreEqual(0.02, (double)generated.measurements["magB"].getValue().errorPlus);
            Assert.AreEqual(0.03, (double)generated.measurements["magB"].getValue().errorMinus);

            Assert.AreEqual(true, generated.measurements.ContainsKey("magJ"));
            Assert.AreEqual(2.943, (double)generated.measurements["magJ"].getValue().value);

            Assert.AreEqual(true, generated.measurements.ContainsKey("spectraltype"));
            Assert.AreEqual("G8 III", (string)generated.measurements["spectraltype"].getValue().value);
        }

        [Test]
        public void CheckWriteToXML()
        {
            // Create new XML file
            Serializer.writeToXML(filePathOut, solarSystem);

            // Compare it with an XML file known to have the same contents as the variable solarSystem
            XMLDeserializer deserializer = new XMLDeserializer(filePathIn);
            StellarObject generated = deserializer.ParseXML();

            Assert.AreEqual(true, generated.measurements.ContainsKey("magB"));
            Assert.AreEqual(5.74, (double)generated.measurements["magB"].getValue().value);
            Assert.AreEqual(0.02, (double)generated.measurements["magB"].getValue().errorPlus);
            Assert.AreEqual(0.03, (double)generated.measurements["magB"].getValue().errorMinus);

            Assert.AreEqual(true, generated.measurements.ContainsKey("magJ"));
            Assert.AreEqual(2.943, (double)generated.measurements["magJ"].getValue().value);

            Assert.AreEqual(true, generated.measurements.ContainsKey("spectraltype"));
            Assert.AreEqual("G8 III", (string)generated.measurements["spectraltype"].getValue().value);
        }

    }
}
