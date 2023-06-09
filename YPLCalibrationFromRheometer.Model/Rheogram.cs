using OSDC.DotnetLibraries.General.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;

namespace YPLCalibrationFromRheometer.Model
{
    public class Rheogram : ICloneable, INamable, IIdentifiable
    {
        public enum RateSourceEnum { RotationalSpeed, ISONewtonianShearRate, BobNewtonianShearRate }
        public enum StressSourceEnum { Torque, ISONewtonianShearStress, BobNewtonianShearStress }

        public enum CalibrationMethodEnum { Mullineux, LevenbergMarquardt, Kelessidis }
        public enum ShearRateCorrectionEnum { SkadsemSaasen, None }
        public enum ShearStressCorrectionEnum { LacParry, None }

        private static double DefaultMeasurementPrecision = 0.25; // default measurement precision of a Fann35 R1B1 is 0.5° # 0.5 lbf/100ft^2 # 0.25 Pa

        private CouetteRheometer rheometer_ = null;
        /// <summary>
        /// an ID for the Rheogram
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// a name for the Rheogram
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// a description for the Rheogram
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// the ID of the Couette Rheometer
        /// </summary>
        public Guid CouetteRheometerID { get; set; }
        /// <summary>
        /// the original source for the rate
        /// </summary>
        public RateSourceEnum RateSource { get; set; }
        /// <summary>
        /// the original source for the stress/torque
        /// </summary>
        public StressSourceEnum StressSource { get; set; }
        /// <summary>
        ///  a list of Measurements for Rheogram
        /// </summary>
        public List<RheometerMeasurement> Measurements { get; set; } = new List<RheometerMeasurement>();
        /// <summary>
        /// the choice of shear rate correction
        /// </summary>
        public ShearRateCorrectionEnum ShearRateCorrection { get; set; }
        /// <summary>
        /// the choice of shear stress correction
        /// </summary>
        public ShearStressCorrectionEnum ShearStressCorrection { get; set; }
        /// <summary>
        /// the choice of calibration method
        /// </summary>
        public CalibrationMethodEnum CalibrationMethod { get; set; }
        /// <summary>
        /// the corrected list of shear rates and stresses
        /// </summary>
        public List<ShearRateAndStress> CorrectedFlowCurve { get; set; } = new List<ShearRateAndStress>();
        /// <summary>
        /// the calibrated YPL model to the corrected list of shear rates and stresses
        /// </summary>
        public YPLModel CalibratedYPLModel { get; set; }

        public int Count => Measurements.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// default constructor
        /// </summary>
        public Rheogram() : base()
        {

        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public Rheogram(Rheogram src) : base()
        {
            if (src != null)
            {
                src.Copy(this);
            }
        }

        /// <summary>
        /// retrieve the measurement precision from the rheometer
        /// </summary>
        /// <returns></returns>
        public double GetMeasurementPrecision()
        {
            if (GetRheometer() != null)
            {
                return GetRheometer().MeasurementPrecision;
            }
            else
            {
                return DefaultMeasurementPrecision;
            }
        }
        public CouetteRheometer GetRheometer()
        {
            return rheometer_;
        }
        public void SetRheometer(CouetteRheometer rheometer)
        {
            rheometer_ = rheometer;
        }

        /// <summary>
        /// copy everything except the ID
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(Rheogram dest)
        {
            if (dest != null)
            {
                dest.Name = Name;
                dest.Description = Description;
                dest.CouetteRheometerID = CouetteRheometerID;
                dest.RateSource = RateSource;
                dest.StressSource = StressSource;
                dest.SetRheometer(GetRheometer());
                if (dest.Measurements == null)
                {
                    dest.Measurements = new List<RheometerMeasurement>();
                }
                dest.Measurements.Clear();
                if (Measurements != null)
                {
                    foreach (RheometerMeasurement itData in Measurements)
                    {
                        RheometerMeasurement iterData1 = new RheometerMeasurement();
                        itData.Copy(iterData1);
                        dest.Measurements.Add(iterData1);
                    }
                }
                dest.CalibrationMethod = CalibrationMethod;
                dest.ShearRateCorrection = ShearRateCorrection;
                dest.ShearStressCorrection = ShearStressCorrection;
                if (dest.CorrectedFlowCurve == null)
                {
                    dest.CorrectedFlowCurve = new List<ShearRateAndStress>();
                }
                dest.CorrectedFlowCurve.Clear();
                if (CorrectedFlowCurve != null)
                {
                    foreach (var v in CorrectedFlowCurve)
                    {
                        if (v != null)
                        {
                            dest.CorrectedFlowCurve.Add(new ShearRateAndStress(v));
                        }
                    }
                }
                if (CalibratedYPLModel != null)
                {
                    dest.CalibratedYPLModel = new YPLModel(CalibratedYPLModel);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// cloning function (including the ID)
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Rheogram copy = new Rheogram(this)
            {
                ID = ID
            };
            return copy;
        }
        /// <summary>
        /// Calculate the other components of the RheometerMeasurements
        /// </summary>
        public void Calculate()
        {
            bool areMeasurementsSet = true;
            CouetteRheometer rheometer = GetRheometer();
            if (Measurements != null && rheometer != null)
            {
                foreach (var m in Measurements)
                {
                    // first convert input rheometer measurements
                    m.Calculate(rheometer, RateSource, StressSource);
                    // then, if stress measurements have not all been set yet, short cut the subsequent correction and calibration steps
                    if (m.Torque == 0)
                        areMeasurementsSet = false;
                }
                if (areMeasurementsSet)
                {
                    YPLCorrection corrector = new YPLCorrection();
                    corrector.RheogramInput = this;
                    if (ShearRateCorrection == ShearRateCorrectionEnum.SkadsemSaasen && ShearStressCorrection == ShearStressCorrectionEnum.LacParry)
                    {
                        corrector.CalculateFullyCorrected(CalibrationMethod);
                        CorrectedFlowCurve = corrector.RheogramFullyCorrected;
                    }
                    else if (ShearRateCorrection == ShearRateCorrectionEnum.SkadsemSaasen)
                    {
                        corrector.CalculateShearRateCorrected(CalibrationMethod);
                        CorrectedFlowCurve = corrector.RheogramShearRateCorrected;
                    }
                    else if (ShearStressCorrection == ShearStressCorrectionEnum.LacParry)
                    {
                        corrector.CalculateShearStressCorrected(CalibrationMethod);
                        CorrectedFlowCurve = corrector.RheogramShearStressCorrected;
                    }
                    else
                    {
                        CorrectedFlowCurve = new List<ShearRateAndStress>();
                        if (Measurements != null)
                        {
                            foreach (var v in Measurements)
                            {
                                CorrectedFlowCurve.Add(new ShearRateAndStress(v.BobNewtonianShearRate, v.BobNewtonianShearStress));
                            }
                        }
                    }
                    if (CorrectedFlowCurve != null)
                    {
                        if (CalibratedYPLModel == null)
                        {
                            CalibratedYPLModel = new YPLModel();
                        }
                        switch (CalibrationMethod)
                        {
                            case CalibrationMethodEnum.Kelessidis:
                                CalibratedYPLModel.FitToKelessidis(CorrectedFlowCurve, GetMeasurementPrecision());
                                break;
                            case CalibrationMethodEnum.LevenbergMarquardt:
                                CalibratedYPLModel.FitToLevenbergMarquardt(CorrectedFlowCurve, GetMeasurementPrecision());
                                break;
                            default:
                                CalibratedYPLModel.FitToMullineux(CorrectedFlowCurve, GetMeasurementPrecision());
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// index accessor. It makes sure that the start of the section is actually the end of the previous section (if any).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RheometerMeasurement this[int index]
        {
            get => Measurements[index];
            set
            {
                Measurements[index] = value;
            }
        }

        /// <summary>
        /// ensure to link start to the end of the previous section
        /// </summary>
        /// <param name="item"></param>
        public void Add(RheometerMeasurement item)
        {
            Measurements.Add(item);
        }

        public void Clear()
        {
            Measurements.Clear();
        }

        public bool Contains(RheometerMeasurement item)
        {
            return Measurements.Contains(item);
        }

        public void CopyTo(RheometerMeasurement[] array, int arrayIndex)
        {
            Measurements.CopyTo(array, arrayIndex);
        }

        public IEnumerator<RheometerMeasurement> GetEnumerator()
        {
            return Measurements.GetEnumerator();
        }

        public int IndexOf(RheometerMeasurement item)
        {
            return Measurements.IndexOf(item);
        }

        public void Insert(int index, RheometerMeasurement item)
        {
            Measurements.Insert(index, item);
        }

        public bool Remove(RheometerMeasurement item)
        {
            return Measurements.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Measurements.RemoveAt(index);
        }

    }
}
