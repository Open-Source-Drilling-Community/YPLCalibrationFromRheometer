using System;
using System.Collections.Generic;
using System.Text;

namespace YPLCalibrationFromRheometer.Model
{
    public class YPLCalibrationMaster
    {
        /// master class used to generate a JsonSchema common to calculation classes YPLCalibration and YPLCorrection
        
        public YPLCalibration YPLCalibration;

        public YPLCorrection YPLCorrection;

        /// <summary>
        /// default constructor
        /// </summary>
        public YPLCalibrationMaster() : base()
        {

        }
    }
}
