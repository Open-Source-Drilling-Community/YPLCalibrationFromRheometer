using NORCE.General.Std;
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
        /// an ID for the RheometerMeasurement, typed as a string to support GUID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// name of the RheometerMeasurement
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// a description for the RheometerMeasurement
        /// </summary>
        public string Description { get; set; }

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
        public RheometerMeasurement() : base()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        public RheometerMeasurement(double shearRate, double shearStress) : base()
        {
            ShearRate = shearRate;
            ShearStress = shearStress;
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
