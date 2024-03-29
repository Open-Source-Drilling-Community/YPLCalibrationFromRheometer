﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class RheometerMeasurement
    {
        /// <summary>
        /// 
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
        /// Serialize a Cluster to Json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// deserialize a string that is expected to be in Json into an instance of RheometerMeasurement
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static RheometerMeasurement FromJson(string str)
        {
            RheometerMeasurement value = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    value = JsonConvert.DeserializeObject<RheometerMeasurement>(str);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return value;
        }

    }
}
