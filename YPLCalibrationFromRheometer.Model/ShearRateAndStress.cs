using OSDC.DotnetLibraries.General.Common;
using System;

namespace YPLCalibrationFromRheometer.Model
{
    /// <summary>
    /// a data class instantiated as a list of objects in the parent class Rheogram
    /// </summary>
    [Serializable]
    public class ShearRateAndStress : ICloneable
    {
        /// <summary>
        /// the shear rate is expected in SI unit, i.e. dimension [T^-1](1/s)
        /// </summary>
        public double ShearRate { get; set; }

        /// <summary>
        /// the shear stress is expected in SI unit, i.e., dimension [ML^-1T^-2](Pa)
        /// </summary>
        public double ShearStress { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public ShearRateAndStress() : base()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public ShearRateAndStress(double shearRate, double shearStress) : base()
        {
            ShearRate = shearRate;
            ShearStress = shearStress;
        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public ShearRateAndStress(ShearRateAndStress src) : base()
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
        public bool Copy(ShearRateAndStress dest)
        {
            if (dest != null)
            {
                dest.ShearRate = ShearRate;
                dest.ShearStress = ShearStress;
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
            ShearRateAndStress iterData1 = new ShearRateAndStress(this);
            return iterData1;
        }

        public int Compare(ShearRateAndStress x, ShearRateAndStress y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            else
            {
                if (Numeric.EQ(x.ShearRate, y.ShearRate, 1e-6))
                {
                    return 0;
                }
                else if (Numeric.GT(x.ShearRate, y.ShearRate, 1e-6))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
