using Newtonsoft.Json;
using System;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class RheometerMeasurement
    {
        /// <summary>
        /// this method is the exact copy of YPLCalibrationFromRheometer.Model.RheometerMeasurement.Copy(RheometerMeasurement dest)
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
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return value;
        }
    }
}

