using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class Rheogram
    {
        /// <summary>
        /// this method is the exact copy of YPLCalibrationFromRheometer.Model.Rheogram.Copy(Rheogram dest)
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(Rheogram dest)
        {
            if (dest != null)
            {
                dest.Name = Name;
                dest.Description = Description;
                dest.ShearStressStandardDeviation = ShearStressStandardDeviation;
                if (dest.RheometerMeasurementList == null)
                {
                    dest.RheometerMeasurementList = new List<RheometerMeasurement>();
                }
                dest.RheometerMeasurementList.Clear();
                if (RheometerMeasurementList != null)
                {
                    foreach (RheometerMeasurement itData in RheometerMeasurementList)
                    {
                        RheometerMeasurement iterData1 = new RheometerMeasurement();
                        itData.Copy(iterData1);
                        dest.RheometerMeasurementList.Add(iterData1);
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
        /// Serialize a Rheogram to Json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// deserialize a string that is expected to be in Json into an instance of Rheogram
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Rheogram FromJson(string str)
        {
            Rheogram values = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    values = JsonConvert.DeserializeObject<Rheogram>(str);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return values;
        }
    }
}
