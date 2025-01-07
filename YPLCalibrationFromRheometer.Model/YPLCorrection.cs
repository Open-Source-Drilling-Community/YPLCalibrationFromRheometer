using OSDC.DotnetLibraries.General.Common;
using System;
using System.Collections.Generic;

namespace YPLCalibrationFromRheometer.Model
{
    public class YPLCorrection : ICloneable, INamable, IIdentifiable
    {
        private double defaultBobRadius = 0.017245;
        private double defaultGap = 0.00117;
        /// <summary>
        /// an ID for the YPLCorrection, typed as a string to support GUID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// name of the YPLCorrection
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// a description for the YPLCorrection
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the input Rheogram
        /// </summary>
        public Rheogram RheogramInput { get; set; }
        public YPLModel YPLModelBasedOnNewtonianInputs { get; set; }

        /// <summary>
        /// the output Rheogram after shear rate and shear stress correction algorithms have been applied
        /// </summary>
        public List<ShearRateAndStress> RheogramFullyCorrected { get; set; }

        public YPLModel YPLModelFullyCorrected { get; set; }

        /// <summary>
        /// the output Rheogram after shear rate correction algorithm has been applied
        /// </summary>
        public List<ShearRateAndStress> RheogramShearRateCorrected { get; set; }
        public YPLModel YPLModelShearRateCorrected { get; set; }

        /// <summary>
        /// the output Rheogram after shear stress correction algorithm has been applied
        /// </summary>
        public List<ShearRateAndStress> RheogramShearStressCorrected { get; set; }
        public YPLModel YPLModelShearStressCorrected { get; set; }

        /// <summary>
        /// parameterless constructor to allow for deserialization
        /// </summary>
        public YPLCorrection() : base()
        {
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public YPLCorrection(YPLCorrection src) : base()
        {
            if (src != null)
            {
                src.Copy(this);
            }
        }

        private double GetR1()
        {
            if (RheogramInput != null && RheogramInput.GetRheometer() != null)
            {
                return RheogramInput.GetRheometer().BobRadius;
            }
            else
            {
                return defaultBobRadius;
            }
        }

        private double GetR2()
        {
            if (RheogramInput != null && RheogramInput.GetRheometer() != null)
            {
                return RheogramInput.GetRheometer().BobRadius + RheogramInput.GetRheometer().Gap;
            }
            else
            {
                return defaultBobRadius + defaultGap;
            }
        }

        /// <summary>
        /// copy everything except the ID
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(YPLCorrection dest)
        {
            if (dest != null)
            {
                dest.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                dest.Name = Name;
                dest.Description = Description;
                if (RheogramInput != null)
                {
                    if (dest.RheogramInput == null)
                        dest.RheogramInput = new Rheogram();
                    RheogramInput.Copy(dest.RheogramInput);
                    if (dest.RheogramInput.ID.Equals(Guid.Empty))
                        dest.RheogramInput.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (YPLModelBasedOnNewtonianInputs != null)
                {
                    if (dest.YPLModelBasedOnNewtonianInputs == null)
                    {
                        dest.YPLModelBasedOnNewtonianInputs = new YPLModel();
                    }
                    YPLModelBasedOnNewtonianInputs.Copy(dest.YPLModelBasedOnNewtonianInputs);
                    if (dest.YPLModelBasedOnNewtonianInputs.ID.Equals(Guid.Empty))
                    {
                        dest.YPLModelBasedOnNewtonianInputs.ID = Guid.NewGuid();
                    }
                }
                if (RheogramFullyCorrected != null)
                {
                    if (dest.RheogramFullyCorrected == null)
                        dest.RheogramFullyCorrected = new List<ShearRateAndStress>();
                    foreach (var v in RheogramFullyCorrected)
                    {
                        dest.RheogramFullyCorrected.Add(new ShearRateAndStress(v));
                    }
                }
                if (YPLModelFullyCorrected != null)
                {
                    if (dest.YPLModelFullyCorrected == null)
                    {
                        dest.YPLModelFullyCorrected = new YPLModel();
                    }
                    YPLModelFullyCorrected.Copy(dest.YPLModelFullyCorrected);
                    if (dest.YPLModelFullyCorrected.ID.Equals(Guid.Empty))
                    {
                        dest.YPLModelFullyCorrected.ID = Guid.NewGuid();
                    }
                }
                if (RheogramShearRateCorrected != null)
                {
                    if (dest.RheogramShearRateCorrected == null)
                        dest.RheogramShearRateCorrected = new List<ShearRateAndStress>();
                    foreach (var v in RheogramShearRateCorrected)
                    {
                        dest.RheogramShearRateCorrected.Add(new ShearRateAndStress(v));
                    }
                }
                if (YPLModelShearRateCorrected != null)
                {
                    if (dest.YPLModelShearRateCorrected == null)
                    {
                        dest.YPLModelShearRateCorrected = new YPLModel();
                    }
                    YPLModelShearRateCorrected.Copy(dest.YPLModelShearRateCorrected);
                    if (dest.YPLModelShearRateCorrected.ID.Equals(Guid.Empty))
                    {
                        dest.YPLModelShearRateCorrected.ID = Guid.NewGuid();
                    }
                }
                if (RheogramShearStressCorrected != null)
                {
                    if (dest.RheogramShearStressCorrected == null)
                        dest.RheogramShearStressCorrected = new List<ShearRateAndStress>();
                    foreach (var v in RheogramShearStressCorrected)
                    {
                        dest.RheogramShearStressCorrected.Add(new ShearRateAndStress(v));
                    }
                }
                if (YPLModelShearStressCorrected != null)
                {
                    if (dest.YPLModelShearStressCorrected == null)
                    {
                        dest.YPLModelShearStressCorrected = new YPLModel();
                    }
                    YPLModelShearStressCorrected.Copy(dest.YPLModelShearStressCorrected);
                    if (dest.YPLModelShearStressCorrected.ID.Equals(Guid.Empty))
                    {
                        dest.YPLModelShearStressCorrected.ID = Guid.NewGuid();
                    }
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
            YPLCorrection copy = new YPLCorrection(this)
            {
                ID = ID
            };
            return copy;
        }

        public bool CalculateInputRheogram()
        {
            bool success = true;
            if (RheogramInput != null)
            {
                if (YPLModelBasedOnNewtonianInputs == null)
                {
                    YPLModelBasedOnNewtonianInputs = new YPLModel();
                }
                if (YPLModelBasedOnNewtonianInputs.ID == Guid.Empty)
                {
                    YPLModelBasedOnNewtonianInputs.ID = Guid.NewGuid();
                }
                if (string.IsNullOrEmpty(YPLModelBasedOnNewtonianInputs.Name))
                {
                    YPLModelBasedOnNewtonianInputs.Name = "From Input Rheogram";
                }
                if (string.IsNullOrEmpty(YPLModelBasedOnNewtonianInputs.Description))
                {
                    YPLModelBasedOnNewtonianInputs.Description = "From Input Rheogram";
                }
                YPLModelBasedOnNewtonianInputs.FitToMullineux(RheogramInput.Measurements, RheogramInput.GetMeasurementPrecision());
                success = true;
            }
            else
            {
                success = false;
            }
            return success;
        }
        /// <summary>
        ///  calculate the shear rate and shear stress corrected Rheogram after Skadsem and Saasen(2019) "Concentric cylinder viscometer flows of Herschel-Bulkley fluids" and Lac and Parry (2017) "Non-Newtonian end-effects in standard oilfield rheometers"
        /// </summary>
        /// <returns></returns>
        public bool CalculateFullyCorrected(Rheogram.CalibrationMethodEnum YPLCalibrationMethod)
        {
            bool success = true;
            if (RheogramInput != null)
            {
                List<RheometerMeasurement> inputDataList = RheogramInput.Measurements;
                if (inputDataList != null && inputDataList.Count > 0)
                {
                    if (RheogramFullyCorrected == null)
                        RheogramFullyCorrected = new List<ShearRateAndStress>(); // this precaution should not be necessary while it is instantiated at construction, but it is actually necessary because the jsonified version of this class in ModelClientShared does not transfer attributes' default values
                    // this case should not arrive since RheometerMeasurementList are instantiated in Rheogram ctor, still risk exists as it is a public settable attribute
                    RheogramFullyCorrected.Clear();

                    if (success)
                    {
                        try
                        {
                            int nMeas = RheogramInput.Measurements.Count;
                            double[] velocities = new double[nMeas];
                            double[] shearRates = new double[nMeas];
                            double[] shearStresses = new double[nMeas];
                            double[] phi = new double[nMeas];
                            YPLModel model = new YPLModel();
                            model.Tau0 = 0;
                            model.K = 1.0;
                            model.N = 1.0;

                            // Converting back assumed Newtonian shear rates to rheometer-dependent rotational velocities
                            for (int i = 0; i < nMeas; ++i)
                            {
                                velocities[i] = GetNewtonianRotationalVelocity(RheogramInput.Measurements[i].BobNewtonianShearRate, GetR1() / GetR2());
                                double phiNewtonian = GetShearStressCorrectionFactor(GetR1(), GetR2(), model.K, model.N, model.Tau0, velocities[i]);
                                shearStresses[i] = RheogramInput.Measurements[i].BobNewtonianShearStress; // note that for this shear stress, the Newtonian correction has already been removed
                            }

                            List<ShearRateAndStress> shearRateAndStresses = new List<ShearRateAndStress>();
                            bool isFullySheared = false;
                            double eps = 1e-5;
                            int count = 0;
                            double dChi2;
                            bool error = false;
                            do
                            {
                                for (int i = 0; i < velocities.Length; ++i)
                                {
                                    // Converting rotational velocities to YPL shear rates for current YPL model parameters
                                    shearRates[i] = GetShearRate(GetR1(), GetR2(), model.K, model.N, model.Tau0, velocities[i], out isFullySheared);
                                    // Computing shear stress correction factor for current YPL model parameters
                                    phi[i] = GetShearStressCorrectionFactor(GetR1(), GetR2(), model.K, model.N, model.Tau0, velocities[i]);
                                    shearRateAndStresses.Add(new ShearRateAndStress(shearRates[i], shearStresses[i] / (1 + phi[i])));
                                }
                                dChi2 = model.Chi2;
                                switch (YPLCalibrationMethod)
                                {
                                    case Rheogram.CalibrationMethodEnum.Kelessidis:
                                        model.FitToLevenbergMarquardt(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    case Rheogram.CalibrationMethodEnum.LevenbergMarquardt:
                                        model.FitToLevenbergMarquardt(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    default:
                                        model.FitToMullineux(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                }
                                if (model.Tau0 < 0)
                                {
                                    error = true;
                                    break;
                                }
                                shearRateAndStresses.Clear();
                                dChi2 -= model.Chi2;
                            } while (System.Math.Abs(dChi2) > eps && count++ < 40);
                            if (!error && count < 40)
                            {
                                for (int i = 0; i < nMeas; ++i)
                                {
                                    ShearRateAndStress meas = new ShearRateAndStress(shearRates[i], shearStresses[i] / (1 + phi[i]));
                                    RheogramFullyCorrected.Add(meas);
                                }
                                if (YPLModelFullyCorrected == null)
                                {
                                    YPLModelFullyCorrected = new YPLModel();
                                }
                                if (YPLModelFullyCorrected.ID == Guid.Empty)
                                {
                                    YPLModelFullyCorrected.ID = Guid.NewGuid();
                                }
                                if (string.IsNullOrEmpty(YPLModelFullyCorrected.Name))
                                {
                                    YPLModelFullyCorrected.Name = "From Fully Corrected Rheogram";
                                }
                                if (string.IsNullOrEmpty(YPLModelFullyCorrected.Description))
                                {
                                    YPLModelFullyCorrected.Description = "From Fully Corrected Rheogram";
                                }
                                switch (YPLCalibrationMethod)
                                {
                                    case Rheogram.CalibrationMethodEnum.Kelessidis:
                                        YPLModelFullyCorrected.FitToLevenbergMarquardt(RheogramFullyCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    case Rheogram.CalibrationMethodEnum.LevenbergMarquardt:
                                        YPLModelFullyCorrected.FitToLevenbergMarquardt(RheogramFullyCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    default:
                                        YPLModelFullyCorrected.FitToMullineux(RheogramFullyCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                }
                            }
                            else
                            {
                                YPLModelFullyCorrected = null;
                                System.Diagnostics.Debug.WriteLine("Full correction calculation did not converge");
                            }
                            return true;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("{0} Exception caught during the correction from Newtonian shear rates to Yield-Power-Law ones", ex);
                            return false;
                        };
                    }
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        ///  calculate the shear rate corrected Rheogram after Skadsem and Saasen(2019) "Concentric cylinder viscometer flows of Herschel-Bulkley fluids"
        /// </summary>
        /// <returns></returns>
        public bool CalculateShearRateCorrected(Rheogram.CalibrationMethodEnum YPLCalibrationMethod)
        {
            bool success = true;
            if (RheogramInput != null)
            {
                List<RheometerMeasurement> inputDataList = RheogramInput.Measurements;
                if (inputDataList != null && inputDataList.Count > 0)
                {
                    if (RheogramShearRateCorrected == null)
                        RheogramShearRateCorrected = new List<ShearRateAndStress>(); // this precaution should not be necessary while it is instantiated at construction, but it is actually necessary because the jsonified version of this class in ModelClientShared does not transfer attributes' default values
                    // this case should not arrive since RheometerMeasurementList are instantiated in Rheogram ctor, still risk exists as it is a public settable attribute
                    RheogramShearRateCorrected.Clear();

                    if (success)
                    {
                        try
                        {
                            int nMeas = RheogramInput.Measurements.Count;
                            double[] velocities = new double[nMeas];
                            double[] shearRates = new double[nMeas];
                            double[] shearStresses = new double[nMeas];
                            YPLModel model = new YPLModel();

                            // Converting back assumed Newtonian shear rates to rheometer-dependent rotational velocities
                            for (int i = 0; i < nMeas; ++i)
                            {
                                velocities[i] = GetNewtonianRotationalVelocity(RheogramInput.Measurements[i].BobNewtonianShearRate, GetR1() / GetR2());
                                shearStresses[i] = RheogramInput.Measurements[i].BobNewtonianShearStress;
                            }

                            List<ShearRateAndStress> shearRateAndStresses = new List<ShearRateAndStress>();
                            bool isFullySheared = false;
                            double eps = 1e-5;
                            int count = 0;
                            double dChi2;
                            bool error = false;
                            do
                            {
                                for (int i = 0; i < velocities.Length; ++i)
                                {
                                    // Converting rotational velocities to YPL shear rates for current YPL model parameters
                                    shearRates[i] = GetShearRate(GetR1(), GetR2(), model.K, model.N, model.Tau0, velocities[i], out isFullySheared);
                                    shearRateAndStresses.Add(new ShearRateAndStress(shearRates[i], shearStresses[i]));
                                }
                                dChi2 = model.Chi2;
                                switch (YPLCalibrationMethod)
                                {
                                    case Rheogram.CalibrationMethodEnum.Kelessidis:
                                        model.FitToLevenbergMarquardt(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    case Rheogram.CalibrationMethodEnum.LevenbergMarquardt:
                                        model.FitToLevenbergMarquardt(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    default:
                                        model.FitToMullineux(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                }
                                if (model.Tau0 < 0)
                                {
                                    error = true;
                                    break;
                                }
                                shearRateAndStresses.Clear();
                                dChi2 -= model.Chi2;
                            } while (System.Math.Abs(dChi2) > eps && count++ < 40);
                            if (!error && count < 40)
                            {
                                for (int i = 0; i < nMeas; ++i)
                                {
                                    ShearRateAndStress meas = new ShearRateAndStress(shearRates[i], shearStresses[i]);
                                    RheogramShearRateCorrected.Add(meas);
                                }
                                if (YPLModelShearRateCorrected == null)
                                {
                                    YPLModelShearRateCorrected = new YPLModel();
                                }
                                if (YPLModelShearRateCorrected.ID == Guid.Empty)
                                {
                                    YPLModelShearRateCorrected.ID = Guid.NewGuid();
                                }
                                if (string.IsNullOrEmpty(YPLModelShearRateCorrected.Name))
                                {
                                    YPLModelShearRateCorrected.Name = "From Shear-rate Corrected Rheogram";
                                }
                                if (string.IsNullOrEmpty(YPLModelShearRateCorrected.Description))
                                {
                                    YPLModelShearRateCorrected.Description = "From Shear-rate Corrected Rheogram";
                                }
                                switch (YPLCalibrationMethod)
                                {
                                    case Rheogram.CalibrationMethodEnum.Kelessidis:
                                        YPLModelShearRateCorrected.FitToLevenbergMarquardt(RheogramShearRateCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    case Rheogram.CalibrationMethodEnum.LevenbergMarquardt:
                                        YPLModelShearRateCorrected.FitToLevenbergMarquardt(RheogramShearRateCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    default:
                                        YPLModelShearRateCorrected.FitToMullineux(RheogramShearRateCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                }
                            }
                            else
                            {
                                YPLModelShearRateCorrected = null;
                                System.Diagnostics.Debug.WriteLine("Shear rate correction calculation did not converge");
                            }
                            return true;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("{0} Exception caught during the correction from Newtonian shear rates to Yield-Power-Law ones", ex);
                            return false;
                        };
                    }
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        /// <summary>
        ///  calculate the shear stress corrected Rheogram after Lac and Parry (2017) "Non-Newtonian end-effects in standard oilfield rheometers"
        /// </summary>
        /// <returns></returns>
        public bool CalculateShearStressCorrected(Rheogram.CalibrationMethodEnum YPLCalibrationMethod)
        {
            bool success = true;
            if (RheogramInput != null)
            {
                List<RheometerMeasurement> inputDataList = RheogramInput.Measurements;
                if (inputDataList != null && inputDataList.Count > 0)
                {
                    if (RheogramShearStressCorrected == null)
                        RheogramShearStressCorrected = new List<ShearRateAndStress>(); // this precaution should not be necessary while it is instantiated at construction, but it is actually necessary because the jsonified version of this class in ModelClientShared does not transfer attributes' default values
                    // this case should not arrive since RheometerMeasurementList are instantiated in Rheogram ctor, still risk exists as it is a public settable attribute
                    RheogramShearStressCorrected.Clear();

                    if (success)
                    {
                        try
                        {
                            int nMeas = RheogramInput.Measurements.Count;
                            double[] velocities = new double[nMeas];
                            double[] shearRates = new double[nMeas];
                            double[] shearStresses = new double[nMeas];
                            double[] phi = new double[nMeas];
                            YPLModel model = new YPLModel();

                            // Converting back assumed Newtonian shear rates to rheometer-dependent rotational velocities
                            for (int i = 0; i < nMeas; ++i)
                            {
                                shearRates[i] = RheogramInput.Measurements[i].BobNewtonianShearRate;
                                velocities[i] = GetNewtonianRotationalVelocity(RheogramInput.Measurements[i].BobNewtonianShearRate, GetR1() / GetR2());
                                shearStresses[i] = RheogramInput.Measurements[i].BobNewtonianShearStress;
                            }

                            List<ShearRateAndStress> shearRateAndStresses = new List<ShearRateAndStress>();
                            double eps = 1e-5;
                            int count = 0;
                            double dChi2;
                            bool error = false;
                            do
                            {
                                for (int i = 0; i < velocities.Length; ++i)
                                {
                                    // Computing shear stress correction factor for current YPL model parameters
                                    phi[i] = GetShearStressCorrectionFactor(GetR1(), GetR2(), model.K, model.N, model.Tau0, velocities[i]);
                                    shearRateAndStresses.Add(new ShearRateAndStress(shearRates[i], shearStresses[i] / (1 + phi[i])));
                                }
                                dChi2 = model.Chi2;
                                switch (YPLCalibrationMethod)
                                {
                                    case Rheogram.CalibrationMethodEnum.Kelessidis:
                                        model.FitToLevenbergMarquardt(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    case Rheogram.CalibrationMethodEnum.LevenbergMarquardt:
                                        model.FitToLevenbergMarquardt(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    default:
                                        model.FitToMullineux(shearRateAndStresses, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                }
                                if (model.Tau0 < 0)
                                {
                                    error = true;
                                    break;
                                }
                                shearRateAndStresses.Clear();
                                dChi2 -= model.Chi2;
                            } while (System.Math.Abs(dChi2) > eps && count++ < 40);
                            if (!error && count < 40)
                            {
                                for (int i = 0; i < nMeas; ++i)
                                {
                                    ShearRateAndStress meas = new ShearRateAndStress(shearRates[i], shearStresses[i] / (1 + phi[i]));
                                    RheogramShearStressCorrected.Add(meas);
                                }
                                if (YPLModelShearStressCorrected == null)
                                {
                                    YPLModelShearStressCorrected = new YPLModel();
                                }
                                if (YPLModelShearStressCorrected.ID == Guid.Empty)
                                {
                                    YPLModelShearStressCorrected.ID = Guid.NewGuid();
                                }
                                if (string.IsNullOrEmpty(YPLModelShearStressCorrected.Name))
                                {
                                    YPLModelShearStressCorrected.Name = "From Shear-stress Corrected Rheogram";
                                }
                                if (string.IsNullOrEmpty(YPLModelShearStressCorrected.Description))
                                {
                                    YPLModelShearStressCorrected.Description = "From Shear-stress Corrected Rheogram";
                                }
                                switch (YPLCalibrationMethod)
                                {
                                    case Rheogram.CalibrationMethodEnum.Kelessidis:
                                        YPLModelShearStressCorrected.FitToLevenbergMarquardt(RheogramShearStressCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    case Rheogram.CalibrationMethodEnum.LevenbergMarquardt:
                                        YPLModelShearStressCorrected.FitToLevenbergMarquardt(RheogramShearStressCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                    default:
                                        YPLModelShearStressCorrected.FitToMullineux(RheogramShearStressCorrected, RheogramInput.GetMeasurementPrecision(), YPLModel.ModelType.YPL);
                                        break;
                                }
                            }
                            else
                            {
                                YPLModelShearStressCorrected = null;
                                System.Diagnostics.Debug.WriteLine("Shear stress correction calculation did not converge");

                            }
                            return true;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("{0} Exception caught during the correction of shear stress", ex);
                            return false;
                        };
                    }
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = false;
            }
            return success;
        }

        public static double GetShearRate(double r1, double r2, double k, double n, double tau_y, double omega, out bool isFullySheared)
        {
            if (tau_y < 0 || (n == 1 & tau_y == 0))
            {
                // the case tau_y is not compatible with the YPL parameter identification process, hence the algorithm will simply return the uncorrected Newtonian shear rate values
                isFullySheared = true;
                return GetNewtonianShearRate(omega, r1 / r2);
            }
            else if (tau_y == 0)
            {
                isFullySheared = true;
                return GetPowerLawShearRate(omega, r1 / r2, n);
            }
            else
            {
                double minV = FindMinimumRotationalVelocity(tau_y, k, r1 / r2, n);
                if (omega > minV)
                {
                    isFullySheared = true;
                    return GetShearRateFullySheared(r1, r2, k, n, tau_y, omega);
                }
                else
                {
                    isFullySheared = false;
                    return GetShearRateNonFullySheared(r1, r2, k, n, tau_y, omega);
                }
            }
        }

        // Get shear rate at all r between r1 and r2
        public static void GetShearRate(double[] r, double r1, double r2, double k, double n, double tau_y, double omega, out bool isFullySheared, out double[] sr)
        {
            isFullySheared = true;
            if (tau_y <= 0)
            {
                GetPowerLawShearRate(r, r1 ,r2, n, omega, out sr); // encompasses both the Newtonian and Power Law cases
            }
            else
            {
                double minV = FindMinimumRotationalVelocity(tau_y, k, r1 / r2, n);
                if (omega > minV)
                {
                    GetShearRateFullySheared(r, r1, r2, k, n, tau_y, omega, out sr);
                }
                else
                {
                    isFullySheared = false;
                    GetShearRateNonFullySheared(r, r1, r2, k, n, tau_y, omega, out sr);
                }
            }
        }

        public static double GetNewtonianShearRate(double omega, double kappa)
        {
            return 2.0 * omega / (1 - kappa * kappa);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tau_y"></param>
        /// <param name="k"></param>
        /// <param name="kappa"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double FindMinimumRotationalVelocity(double tau_y, double k, double kappa, double n)
        {
            //tex: Based on equation (7): 
            //$$ \left(\frac{k \Omega^{\star n}}{\tau_y} \right)^\frac 1n = \int_\kappa^1 \frac{1}{\tilde r} \left(\frac{1}{\tilde{r}^2} -1   \right)^{\frac 1n}d\tilde r  $$

            double integral = IntegrationFKappaN(kappa, n);
            double factor = System.Math.Pow(k / tau_y, 1.0 / n);
            return integral / factor;
        }

        // Get shear rate at all r between r1 and r2
        public static void GetPowerLawShearRate(double[] r, double r1, double r2, double n, double omega, out double[] sr)
        {
            int nbPoints = r.Length;
            sr = new double[nbPoints];

            for (int i = 0; i < r.Length; ++i)
            {
                sr[i] = 2.0 * omega / (n * (1 - System.Math.Pow(r1 / r2, 2.0 / n))) * System.Math.Pow(r1 / r[i], 2.0 / n);
            }
        }

        // Get shear rate at the wall only
        public static double GetPowerLawShearRate(double omega, double kappa, double n)
        {
            return 2 * omega / n / (1 - System.Math.Pow(kappa, 2.0 / n));
        }

        public static double CalculateRPlug(double k, double omega, double tau_y, double n, double r1, double r2)
        {
            // Newton-Raphson scheme is used to determine Rplug from Eq. (9)
            double epsilon = 1e-16; // for low values of n (0.2), the value of integral in Eq. (9) around 1e-13 or less: reduce espilon if evaluating even lower values of n
            double kappa = r1 / r2;
            double leftHandSide = omega * System.Math.Pow(k / tau_y, 1.0 / n);

            double rp = .5 * (r1 + r2);
            double diff = leftHandSide - IntegrationEquation9(rp, kappa, n, r2);

            double rpPrime = rp * 1.01;
            double diffPrime = leftHandSide - IntegrationEquation9(rpPrime, kappa, n, r2);

            double derivative = (diffPrime - diff) / (rpPrime - rp);

            int count = 0;

            while (System.Math.Abs(diff) > epsilon && count++ < 100)
            {
                rp -= diff / derivative;
                diff = leftHandSide - IntegrationEquation9(rp, kappa, n, r2);
                rpPrime = rp * 1.01;
                diffPrime = leftHandSide - IntegrationEquation9(rpPrime, kappa, n, r2);
                derivative = (diffPrime - diff) / (rpPrime - rp);
            }
            if (count > 100)
            {
                System.Diagnostics.Debug.WriteLine("Numerical error in CalculateRPlug");
            }
            return rp;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="omega"></param>
        /// <param name="tau_y"></param>
        /// <param name="n"></param>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static double CalculateC(double k, double omega, double tau_y, double n, double r1, double r2)
        {
            //tex: Finds the constant of integration $c$ from equation (8):
            //$$ \left(\frac{k \Omega^{ n}}{\tau_y} \right)^\frac 1n = \int_\kappa^1 \frac{1}{\tilde r} \left(\frac{\tau(\tilde{r}R_2)}{ \tau_y}   -1   \right)^{\frac 1n}d\tilde r  $$
            //with $\tau(r) = c / r^2$
            //tex: The existence and unicity of C for any value of n is guaranteed by the analysis of the variations of Eq. (8)
            // as a function f of C and Leibnitz' theorem.
            // It can be shown that 1) f(C) strictly increases over $ ]R_2^2 \tau_y, +\infty[ $
            // and 2) C exists if:
            //$$ \Omega > \Omega^\star = (\frac \tau k)^{\frac 1n} * f(\kappa,n) $$

            // Newton-Raphson scheme is used to determine C from Eq. (8)

            double kappa = r1 / r2;
            double leftHandSide = omega * System.Math.Pow(k / tau_y, 1.0 / n);

            //tex: for all non integer values of 1/n, the definition interval of the intragel is limited to: $[R_2^2 * \tau_y, +\infty[$ 

            //tex: interestingly for n=0.5, C can take 2 values. The higher one is the true solution since the lower one comes from the
            // square elevation of Eq. (3b) to obtain Eq. (6)

            double c = 1.01 * r2 * r2 * tau_y;
            double diff = leftHandSide - IntegrationEquation8(c, kappa, n, tau_y, r2);
            double cPrime = c * 1.01;
            double diffPrime = leftHandSide - IntegrationEquation8(cPrime, kappa, n, tau_y, r2);

            double derivative = (diffPrime - diff) / (cPrime - c);

            int count = 0;

            while (System.Math.Abs(diff) > 1e-8 && count++ < 100)
            {
                c -= diff / derivative;
                diff = leftHandSide - IntegrationEquation8(c, kappa, n, tau_y, r2);
                cPrime = c * 1.01;
                diffPrime = leftHandSide - IntegrationEquation8(cPrime, kappa, n, tau_y, r2);
                derivative = (diffPrime - diff) / (cPrime - c);
            }
            if (count > 100)
            {
                System.Diagnostics.Debug.WriteLine("Numerical error in CalculateC");
            }
            return c;
        }

        // Get shear rate at all r between r1 and r2
        public static void GetShearRateFullySheared(double[] r, double r1, double r2, double k, double n, double tau_y, double omega, out double[] sr)
        {
            int nbPoints = r.Length;
            sr = new double[nbPoints];
            double c = CalculateC(k, omega, tau_y, n, r1, r2);

            for (int i = 0; i < r.Length; ++i)
            {
                //tex: $\tau(r) = c / r^2$
                double tau_r = c / (r[i] * r[i]);

                //tex: $$ \dot{\gamma}(r) = \left[ \left(\frac{\tau_y}{k} \right) \left( \frac{\tau (r)}{\tau_y} \right) -1 \right]^{ \frac 1n }$$
                //with $r = R_1$
                sr[i] = System.Math.Pow((tau_y / k) * (tau_r / tau_y - 1.0), 1.0 / n);
            }
        }

        // Get fully sheared shear rate at the wall only
        public static double GetShearRateFullySheared(double r1, double r2, double k, double n, double tau_y, double omega)
        {
            double c = CalculateC(k, omega, tau_y, n, r1, r2);

            //tex: $\tau(r) = c / r^2$
            double tau_r = c / (r1 * r1);

            //tex: $$ \dot{\gamma}(r) = \left[ \left(\frac{\tau_y}{k} \right) \left( \frac{\tau (r)}{\tau_y} \right) -1 \right]^{ \frac 1n }$$
            //with $r = R_1$


            return System.Math.Pow((tau_y / k) * (tau_r / tau_y - 1.0), 1.0 / n);
        }

        // Get non fully sheared shear rate at all r between r1 and r2
        public static void GetShearRateNonFullySheared(double[] r, double r1, double r2, double k, double n, double tau_y, double omega, out double[] sr)
        {
            int nbPoints = r.Length;
            sr = new double[nbPoints];
            double rPlug = CalculateRPlug(k, omega, tau_y, n, r1, r2);
            double tau_r;

            for (int i = 0; i < r.Length; ++i)
            {
                if (r[i] < rPlug)
                {
                    tau_r = tau_y * rPlug * rPlug / (r[i] * r[i]);
                    sr[i] = System.Math.Pow((tau_y / k) * (tau_r / tau_y - 1.0), 1.0 / n);
                }
                else
                {
                    sr[i] = 0;
                }

            }
        }

        // Get shear rate at the wall only
        public static double GetShearRateNonFullySheared(double r1, double r2, double k, double n, double tau_y, double omega)
        {
            double rPlug = CalculateRPlug(k, omega, tau_y, n, r1, r2);
            double tau_r = tau_y * rPlug * rPlug / (r1 * r1);
            return System.Math.Pow((tau_y / k) * (tau_r / tau_y - 1.0), 1.0 / n);
        }

        public static double GetNewtonianRotationalVelocity(double shearRate, double kappa)
        {
            return shearRate * (1 - kappa * kappa) / 2.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="k"></param>
        /// <param name="tau_y"></param>
        /// <param name="n"></param>
        /// <param name="r1"></param>
        /// <param name="r"></param>
        /// <param name="sr"></param>
        /// <param name="nbOfIntervals"></param>
        /// <returns></returns>
        public static double IntegrationEquation6(double c, double k, double tau_y, double n, double r1, double r, int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (6):
            //$$\int_{R_1}^r \frac{\dot\gamma(r)}{r}dr $$

            double step = (r - r1) / (nbOfIntervals);
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = r1 + i * step;
                double x_r = r1 + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(System.Math.Abs(c / (tau_y * x_l * x_l) - 1), oneOverN); // absolute value avoids rounding errors near operand's nullity
                double y_r = (1.0 / x_r) * System.Math.Pow(System.Math.Abs(c / (tau_y * x_r * x_r) - 1), oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return System.Math.Pow(tau_y / k, oneOverN) * integral;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="kappa"></param>
        /// <param name="n"></param>
        /// <param name="tau_y"></param>
        /// <param name="r2"></param>
        /// <param name="nbOfIntervals"></param>
        /// <returns></returns>
        public static double IntegrationEquation8(double c, double kappa, double n, double tau_y, double r2, int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (8):
            //$$\int_\kappa^1 \frac{1}{\tilde r} \left(\frac{c}{\tilde{r}^2 R_2^2 \tau_y}   -1   \right)^{\frac 1n}d\tilde r  $$

            double step = (1.0 - kappa) / (nbOfIntervals);
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = kappa + i * step;
                double x_r = kappa + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(System.Math.Abs(c / (tau_y * r2 * r2 * x_l * x_l) - 1), oneOverN); // absolute value avoids rounding errors near operand's nullity
                double y_r = (1.0 / x_r) * System.Math.Pow(System.Math.Abs(c / (tau_y * r2 * r2 * x_r * x_r) - 1), oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return integral;
        }

        public static double IntegrationEquation9(double rPlug, double kappa, double n, double r2, int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (8):
            //$$\int_\kappa^{\kappa_p} \frac{1}{\tilde r} \left(\frac{R_P^2}{\tilde{r}^2 R_2 }   -1   \right)^{\frac 1n}d\tilde r  $$

            double kappaP = rPlug / r2;
            double step = (kappaP - kappa) / nbOfIntervals;
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = kappa + i * step;
                double x_r = kappa + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(System.Math.Abs(rPlug * rPlug / (r2 * r2 * x_l * x_l) - 1), oneOverN); // absolute value avoids rounding errors near operand's nullity
                double y_r = (1.0 / x_r) * System.Math.Pow(System.Math.Abs(rPlug * rPlug / (r2 * r2 * x_r * x_r) - 1), oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return integral;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kappa"></param>
        /// <param name="n"></param>
        /// <param name="nbOfPoints"></param>
        /// <returns></returns>
        public static double IntegrationFKappaN(double kappa, double n, int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (7):
            //$$\int_\kappa^1 \frac{1}{\tilde r} \left(\frac{1}{\tilde{r}^2} -1   \right)^{\frac 1n}d\tilde r  $$

            double step = (1.0 - kappa) / nbOfIntervals;
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = kappa + i * step;
                double x_r = kappa + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(1.0 / (x_l * x_l) - 1, oneOverN);
                double y_r = (1.0 / x_r) * System.Math.Pow(1.0 / (x_r * x_r) - 1, oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return integral;
        }

        public static double GetShearStressCorrectionFactor(double r1, double r2, double k, double n, double tau_y, double omega)
        {
            double eps = 1e-5;
            if (Numeric.EQ(r1, 0.017245, eps) && Numeric.EQ(r2, 0.018415, eps))
            {
                // Fann35 R1B1
                double phiInf = 0.2693;
                double kFit = 1.007;
                double alpha = 2.12;
                double beta = 0.9136;
                double q = 1.195;
                return EvaluateFitFunction(k, n, tau_y, phiInf, phiInf, kFit, alpha, beta, q, omega);
            }
            else if (Numeric.EQ(r1, 0.015987, eps) && Numeric.EQ(r2, 0.018415, eps))
            {
                // Fann35 R1B5
                double phiInf = 0.2537;
                double kFit = 0.5555;
                double alpha = 0.99;
                double beta = 0.8478;
                double q = 1.269;
                return EvaluateFitFunction(k, n, tau_y, phiInf, phiInf, kFit, alpha, beta, q, omega);
            }
            else if (Numeric.EQ(r1, 0.012276, eps) && Numeric.EQ(r2, 0.018415, eps))
            {
                // Fann35 R1B2
                double phiInf = 0.225;
                double phiInf2 = 0.198;
                double kFit = 0.05;
                double alpha = 12.48;
                double beta = 7.576;
                double q = 1.094;
                return EvaluateFitFunction(k, n, tau_y, phiInf, phiInf2, kFit, alpha, beta, q, omega);
            }
            else
            {
                return 0.064;
            }
        }

        public static double EvaluateFitFunction(double k, double n, double tau_y, double phiInf, double phiInf2,
            double kFit, double alpha, double beta, double q, double omega)
        {
            double B0 = Math.Pow(alpha * Math.Pow(n, 2) + beta, 2);
            double B = tau_y / (k * omega);
            double phi0 = phiInf2 * Math.Exp(-kFit * n);
            return phiInf + (phi0 - phiInf) / (1 + Math.Pow(B / B0, q));
        }
    }
}