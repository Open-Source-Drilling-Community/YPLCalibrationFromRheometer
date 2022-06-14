using Newtonsoft.Json;
using System;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class YPLCalibration
    {
        /// <summary>
        /// copy everything except the ID
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(YPLCalibration dest)
        {
            if (dest != null)
            {
                dest.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                dest.Name = Name;
                dest.Description = Description;
                if (RheogramInput != null)
                {
                    if (dest.RheogramInput == null)
                        dest.RheogramInput = new Rheogram();
                    RheogramInput.Copy(dest.RheogramInput);
                    if (dest.RheogramInput.ID.Equals(Guid.Empty))
                        dest.RheogramInput.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (YPLModelKelessidis != null)
                {
                    if (dest.YPLModelKelessidis == null)
                        dest.YPLModelKelessidis = new YPLModel();
                    YPLModelKelessidis.Copy(dest.YPLModelKelessidis);
                    if (dest.YPLModelKelessidis.ID.Equals(Guid.Empty))
                        dest.YPLModelKelessidis.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (YPLModelLevenbergMarquardt != null)
                {
                    if (dest.YPLModelLevenbergMarquardt == null)
                        dest.YPLModelLevenbergMarquardt = new YPLModel();
                    YPLModelLevenbergMarquardt.Copy(dest.YPLModelLevenbergMarquardt);
                    if (dest.YPLModelLevenbergMarquardt.ID.Equals(Guid.Empty))
                        dest.YPLModelLevenbergMarquardt.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize a YPLCalibration to Json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// deserialize a string that is expected to be in Json into an instance of YPLCalibration
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static YPLCalibration FromJson(string str)
        {
            YPLCalibration values = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    values = JsonConvert.DeserializeObject<YPLCalibration>(str);
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
