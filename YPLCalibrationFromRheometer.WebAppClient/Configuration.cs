using System;
using Newtonsoft.Json;

namespace YPLCalibrationFromRheometer.WebAppClient
{
    public class Configuration
    {
        public string HostURL { get; set; } = "https://app.DigiWells.no/";

        /// <summary>
        /// Serialize a Geodetic to Json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// deserialize a string that is expected to be in Json into an instance of Geodetic
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Configuration FromJson(string str)
        {
            Configuration values = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    values = JsonConvert.DeserializeObject<Configuration>(str);
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
