using OSDC.DotnetLibraries.General.Common;
using System;

namespace YPLCalibrationFromRheometer.Model
{
    /// <summary>
    /// a data class instantiated as a list of objects in the parent class Rheogram
    /// </summary>
    [Serializable]
    public class RheometerMeasurement : ICloneable
    {
        /// <summary>
        /// the rotational speed of the rotor or bob depending of the type of rheometer, in [rev/s]
        /// </summary>
        public double RotationalSpeed { get; set; }
        /// <summary>
        /// the measured torque at the bob, in [N.m]
        /// </summary>
        public double Torque { get; set; }
        /// <summary>
        /// the shear rate in the middle of the gap (ISO convention) utilizing the Newtonian fluid hypothesis, in [1/s]
        /// </summary>
        public double ISONewtonianShearRate { get; set; }
        /// <summary>
        /// the shear stress in the middle of the gap (ISO convention) utilizing the Newtonian fluid hypothesis, in [Pa]
        /// </summary>
        public double ISONewtonianShearStress { get; set; }
        /// <summary>
        /// the shear rate at the bob wall utilizing the Newtonian fluid hypothesis, in [1/s]
        /// </summary>
        public double BobNewtonianShearRate { get; set; }
        /// <summary>
        /// the shear stress at the bob wall utilizing the Newtonian fluid hypothesis, in [Pa]
        /// </summary>
        public double BobNewtonianShearStress { get; set; }
        /// <summary>
        /// default constructor
        /// </summary>
        public RheometerMeasurement() : base()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public RheometerMeasurement(double rate, double stressOrTorque, Rheogram.RateSourceEnum rateSource, Rheogram.StressSourceEnum stressSource) : base()
        {
            switch (rateSource)
            {
                case Rheogram.RateSourceEnum.RotationalSpeed:
                    this.RotationalSpeed = rate;
                    break;
                case Rheogram.RateSourceEnum.ISONewtonianShearRate:
                    ISONewtonianShearRate = rate;
                    break;
                default:
                    BobNewtonianShearRate = rate;
                    break;
            }
            switch (stressSource)
            {
                case Rheogram.StressSourceEnum.Torque:
                    Torque = stressOrTorque;
                    break;
                case Rheogram.StressSourceEnum.ISONewtonianShearStress:
                    ISONewtonianShearStress = stressOrTorque;
                    break;
                default:
                    BobNewtonianShearStress = stressOrTorque;
                    break;
            }
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public RheometerMeasurement(RheometerMeasurement src) : base()
        {
            if (src != null)
            {
                src.Copy(this);
            }
        }

        /// <summary>
        /// copy all RheometerMeasurement attributes except the ID
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(RheometerMeasurement dest)
        {
            if (dest != null)
            {
                dest.RotationalSpeed = RotationalSpeed;
                dest.Torque = Torque;
                dest.ISONewtonianShearRate = ISONewtonianShearRate;
                dest.ISONewtonianShearStress = ISONewtonianShearStress;
                dest.BobNewtonianShearRate = BobNewtonianShearRate;
                dest.BobNewtonianShearStress = BobNewtonianShearStress;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// cloning (including the ID)
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            RheometerMeasurement iterData1 = new RheometerMeasurement(this);
            return iterData1;
        }
        public int Compare(RheometerMeasurement x, RheometerMeasurement y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            else
            {
                if (Numeric.EQ(x.BobNewtonianShearRate, y.BobNewtonianShearRate, 1e-6))
                {
                    return 0;
                }
                else if (Numeric.GT(x.BobNewtonianShearRate, y.BobNewtonianShearRate, 1e-6))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        public void Calculate(CouetteRheometer rheometer, Rheogram.RateSourceEnum rateSource, Rheogram.StressSourceEnum stressSource)
        {
            if (rheometer != null && !Numeric.EQ(rheometer.BobRadius, 0) && !Numeric.EQ(rheometer.Gap, 0) && !Numeric.EQ(rheometer.BobLength, 0))
            {
                double ksi = (rheometer.BobRadius + rheometer.Gap) / rheometer.BobRadius;
                double omega;
                switch (rateSource)
                {
                    case Rheogram.RateSourceEnum.RotationalSpeed:
                        omega = RotationalSpeed * 2.0 * Math.PI;
                        ISONewtonianShearRate = (1.0 + ksi * ksi) * omega / (ksi * ksi - 1.0);
                        BobNewtonianShearRate = 2.0 * ksi * ksi * omega / (ksi * ksi - 1.0);
                        break;
                    case Rheogram.RateSourceEnum.ISONewtonianShearRate:
                        omega = ISONewtonianShearRate * (ksi * ksi - 1.0) / (1 + ksi * ksi);
                        RotationalSpeed = omega / (2.0 * Math.PI);
                        BobNewtonianShearRate = 2.0 * ksi * ksi * omega / (ksi * ksi - 1.0);
                        break;
                    default:
                        omega = BobNewtonianShearRate * (ksi * ksi - 1) / (2.0 * ksi * ksi);
                        RotationalSpeed = omega / (2.0 * Math.PI);
                        ISONewtonianShearRate = (1.0 + ksi * ksi) * omega / (ksi * ksi - 1.0);
                        break;
                }
                switch (stressSource)
                {
                    case Rheogram.StressSourceEnum.Torque:
                        ISONewtonianShearStress = (1.0 + ksi * ksi) * Torque / (2.0 * ksi * ksi * 2.0 * Math.PI * rheometer.BobRadius * rheometer.BobRadius * rheometer.BobLength * rheometer.NewtonianEndEffectCorrection);
                        BobNewtonianShearStress = 2.0 * ksi * ksi * ISONewtonianShearStress / (1.0 + ksi * ksi);
                        break;
                    case Rheogram.StressSourceEnum.ISONewtonianShearStress:
                        Torque = ISONewtonianShearStress * 2.0 * ksi * ksi * 2.0 * Math.PI * rheometer.BobRadius * rheometer.BobRadius * rheometer.BobLength * rheometer.NewtonianEndEffectCorrection / (1.0 + ksi * ksi);
                        BobNewtonianShearStress = 2.0 * ksi * ksi * ISONewtonianShearStress / (1.0 + ksi * ksi);
                        break;
                    default:
                        ISONewtonianShearStress = BobNewtonianShearStress * (1.0 + ksi * ksi) / (2.0 * ksi * ksi);
                        Torque = ISONewtonianShearStress * 2.0 * ksi * ksi * 2.0 * Math.PI * rheometer.BobRadius * rheometer.BobRadius * rheometer.BobLength * rheometer.NewtonianEndEffectCorrection / (1.0 + ksi * ksi);
                        break;
                }
            }
        }
    }
}
