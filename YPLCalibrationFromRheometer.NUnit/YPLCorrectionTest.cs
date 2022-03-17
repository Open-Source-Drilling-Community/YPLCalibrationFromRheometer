using System;
using System.Linq;
using NUnit.Framework;
using YPLCalibrationFromRheometer.Model;

namespace Tests
{
    public class YPLCorrectionTests
    {
        private const double FANN35_R1B1_STRESS_FACTOR = 0.5107;
        private const double eps = 1.0e-1;
        private const double r1 = .017245;
        private const double r2 = .018415;

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestNewtonianWBM()
        {

            double[] newtonianShearRates = { 1021.4, 510.7, 340.5, 170.2, 10.2, 5.1 };
            double[] yplShearRates = { 1106.2, 558, 374.5, 190.2, 13.4, 7.2 };
            double[] shearStresses = { 60, 45.5, 37.5, 29, 14, 12 };

            YPLCorrection yplCorrection = new YPLCorrection()
            {
                ID = Guid.NewGuid()
            };
            yplCorrection.Name = "DefaultName-" + yplCorrection.ID.ToString()[..8];
            yplCorrection.Description = "DefaultDescription-" + yplCorrection.ID.ToString()[..8];
            yplCorrection.R1 = r1;
            yplCorrection.R2 = r2;
            yplCorrection.RheogramInput = new Rheogram
            {
                ID = Guid.NewGuid(),
                Name = yplCorrection.Name + "-input"
            };
            yplCorrection.RheogramShearRateCorrected = new Rheogram
            {
                ID = Guid.NewGuid(),
                Name = yplCorrection.Name + "-calculated-shearRateCorrected"
            };

            shearStresses = shearStresses.Select(d => d * FANN35_R1B1_STRESS_FACTOR).ToArray();

            for (int i = 0; i < newtonianShearRates.Length; ++i)
                yplCorrection.RheogramInput.RheometerMeasurementList.Add(new RheometerMeasurement(newtonianShearRates[i], shearStresses[i]));

            yplCorrection.CalculateRheogramShearRateCorrected();

            for (int i = 0; i < newtonianShearRates.Length; ++i)
            {
                Assert.AreEqual(yplShearRates[i], yplCorrection.RheogramShearRateCorrected.RheometerMeasurementList[i].ShearRate, eps);
            }
        }

        [Test]
        public void TestNewtonianSpacer()
        {
            double[] newtonianShearRates = { 1021.4, 510.7, 340.5, 170.2, 102.1, 51.1, 10.2, 5.1 };
            double[] yplShearRates = { 1100.6, 555.4, 373, 189.6, 115.7, 59.7, 13.6, 7.4 };
            double[] shearStresses = { 78, 58.5, 49, 37, 31, 24.5, 18, 16 };

            YPLCorrection calculationData = new YPLCorrection()
            {
                ID = Guid.NewGuid()
            };
            calculationData.Name = "DefaultName-" + calculationData.ID.ToString()[..8];
            calculationData.Description = "DefaultDescription-" + calculationData.ID.ToString()[..8];
            calculationData.R1 = r1;
            calculationData.R2 = r2;
            calculationData.RheogramInput = new Rheogram
            {
                ID = Guid.NewGuid(),
                Name = calculationData.Name + "-input"
            };
            calculationData.RheogramShearRateCorrected = new Rheogram
            {
                ID = Guid.NewGuid(),
                Name = calculationData.Name + "-calculated-shearRateCorrected"
            };

            shearStresses = shearStresses.Select(d => d * FANN35_R1B1_STRESS_FACTOR).ToArray();

            for (int i = 0; i < newtonianShearRates.Length; ++i)
                calculationData.RheogramInput.RheometerMeasurementList.Add(new RheometerMeasurement(newtonianShearRates[i], shearStresses[i]));

            calculationData.CalculateRheogramShearRateCorrected();

            for (int i = 0; i < newtonianShearRates.Length; ++i)
            {
                Assert.AreEqual(yplShearRates[i], calculationData.RheogramShearRateCorrected.RheometerMeasurementList[i].ShearRate, eps);
            }
        }

        [Test]
        public void TestNewtonianSlurry()
        {
            double[] newtonianShearRates = { 510.7, 340.5, 170.2, 102.1, 51.1, 10.2, 5.1 };
            double[] yplShearRates = { 526.3, 351, 175.7, 105.5, 52.9, 10.7, 5.4 };
            double[] shearStresses = { 160, 123, 75, 54, 36, 12, 9 };

            YPLCorrection calculationData = new YPLCorrection()
            {
                ID = Guid.NewGuid()
            };
            calculationData.Name = "DefaultName-" + calculationData.ID.ToString()[..8];
            calculationData.Description = "DefaultDescription-" + calculationData.ID.ToString()[..8];
            calculationData.R1 = r1;
            calculationData.R2 = r2;
            calculationData.RheogramInput = new Rheogram
            {
                ID = Guid.NewGuid(),
                Name = calculationData.Name + "-input"
            };
            calculationData.RheogramShearRateCorrected = new Rheogram
            {
                ID = Guid.NewGuid(),
                Name = calculationData.Name + "-calculated-shearRateCorrected"
            };

            shearStresses = shearStresses.Select(d => d * FANN35_R1B1_STRESS_FACTOR).ToArray();

            for (int i = 0; i < newtonianShearRates.Length; ++i)
                calculationData.RheogramInput.RheometerMeasurementList.Add(new RheometerMeasurement(newtonianShearRates[i], shearStresses[i]));

            calculationData.CalculateRheogramShearRateCorrected();

            for (int i = 0; i < newtonianShearRates.Length; ++i)
            {
                Assert.AreEqual(yplShearRates[i], calculationData.RheogramShearRateCorrected.RheometerMeasurementList[i].ShearRate, eps);
            }
        }
    }
}