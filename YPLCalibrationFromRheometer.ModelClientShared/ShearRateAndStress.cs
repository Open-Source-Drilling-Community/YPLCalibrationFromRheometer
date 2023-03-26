using Newtonsoft.Json;
using System;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class ShearRateAndStress
    {
        /// <summary>
        /// this method is the exact copy of YPLCalibrationFromRheometer.Model.RheometerMeasurement.Copy(RheometerMeasurement dest)
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
        public static ShearRateAndStress FromJson(string str)
        {
            ShearRateAndStress value = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    value = JsonConvert.DeserializeObject<ShearRateAndStress>(str);
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

