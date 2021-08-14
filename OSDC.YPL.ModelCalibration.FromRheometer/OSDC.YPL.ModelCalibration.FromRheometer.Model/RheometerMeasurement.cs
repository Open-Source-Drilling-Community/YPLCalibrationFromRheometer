using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    public struct RheometerMeasurement
    {
        /// <summary>
        /// The shear rate is expected in SI unit, i.e. dimension [T^-1](1/s)
        /// </summary>
        public double ShearRate { get; set; }
        /// <summary>
        /// The shear stress is expected in SI unit, i.e., dimension [ML^-1T^-2](Pa)
        /// </summary>
        public double ShearStress { get; set; }

        /// <summary>
        /// initialization constructor
        /// </summary>
        /// <param name="shearRate"></param>
        /// <param name="shearStress"></param>
        public RheometerMeasurement(double shearRate, double shearStress)
        {
            ShearRate = shearRate;
            ShearStress = shearStress;
        }
    }
}
