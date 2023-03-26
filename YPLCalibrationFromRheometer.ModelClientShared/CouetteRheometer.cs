using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class CouetteRheometer
    {
        /// <summary>
        /// Serialize a CouetteRheometer to Json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// deserialize a string that is expected to be in Json into an instance of CouetteRheometer
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static CouetteRheometer FromJson(string str)
        {
            CouetteRheometer values = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    values = JsonConvert.DeserializeObject<CouetteRheometer>(str);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return values;
        }

    }
}
