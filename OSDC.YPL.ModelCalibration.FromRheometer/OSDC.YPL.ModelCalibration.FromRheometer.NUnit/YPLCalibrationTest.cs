using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace Tests
{
    public class YPLCalibrationTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestNewtonianZamora()
        {
            double tau0 = 0;
            double K = 0.75;
            double n = 1.0;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToKelessidis(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        [Test]
        public void TestNewtonianMullineux()
        {
            double tau0 = 0;
            double K = 0.75;
            double n = 1.0;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToMullineux(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        [Test]
        public void TestPowerLawShearThinningZamora()
        {
            double tau0 = 0;
            double K = 0.75;
            double n = 0.5;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToKelessidis(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        [Test]
        public void TestPowerLawShearThinningMullineux()
        {
            double tau0 = 0;
            double K = 0.75;
            double n = 0.5;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToMullineux(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        [Test]
        public void TestPowerLawShearThickenningZamora()
        {
            double tau0 = 0;
            double K = 0.75;
            double n = 1.5;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToKelessidis(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        /* Mullineux's algorithm does not work for shear thickenning
        [Test]
        public void TestPowerLawShearThickenningMullineux()
        {
            double tau0 = 0;
            double K = 0.75;
            double n = 1.5;
            Rheogram rheogram = new Rheogram();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new RheometerMeasurement[size];
            int idx = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements[idx++] = new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n));
            }
            YPLModel model = new YPLModel();
            model.FitToMullineux(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        */
        [Test]
        public void TestBinghamPlasticZamora()
        {
            double tau0 = 2.0;
            double K = 0.75;
            double n = 1.0;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToKelessidis(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 5e-3);
            Assert.AreEqual(K, model.K, 5e-3);
            Assert.AreEqual(n, model.n, 5e-3);
        }
        [Test]
        public void TestBinghamPlasticMullineux()
        {
            double tau0 = 2.0;
            double K = 0.75;
            double n = 1.0;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToMullineux(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        [Test]
        public void TestHerschelBulkleyShearThinningZamora()
        {
            double tau0 = 2.0;
            double K = 0.75;
            double n = 0.5;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToKelessidis(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 5e-3);
            Assert.AreEqual(K, model.K, 5e-3);
            Assert.AreEqual(n, model.n, 5e-3);
        }
        [Test]
        public void TestHerschelBulkleyShearThinningMullineux()
        {
            double tau0 = 2.0;
            double K = 0.75;
            double n = 0.5;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToMullineux(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        
        [Test]
        public void TestHerschelBulkleyShearThickenningZamora()
        {
            double tau0 = 2.0;
            double K = 0.75;
            double n = 1.5;
            Rheogram rheogram = new();
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram.Measurements = new List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new();
            model.FitToKelessidis(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 5e-3);
            Assert.AreEqual(K, model.K, 5e-3);
            Assert.AreEqual(n, model.n, 5e-3);
        }
        /* Mullineux's algorithm does not work for shear thickenning
        [Test]
        public void TestHerschelBulkleyShearThickenningMullineux()
        {
            double tau0 = 2.0;
            double K = 0.75;
            double n = 1.5;
            Rheogram rheogram = new Rheogram();
            rheogram.Measurements = new System.Collections.Generic.List<RheometerMeasurement>();
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                rheogram.Measurements.Add(new RheometerMeasurement(gammaDot, tau0 + K * Math.Pow(gammaDot, n)));
            }
            YPLModel model = new YPLModel();
            model.FitToMullineux(rheogram);
            Assert.AreEqual(tau0, model.Tau0, 1e-3);
            Assert.AreEqual(K, model.K, 1e-3);
            Assert.AreEqual(n, model.n, 1e-3);
        }
        */
    }
}