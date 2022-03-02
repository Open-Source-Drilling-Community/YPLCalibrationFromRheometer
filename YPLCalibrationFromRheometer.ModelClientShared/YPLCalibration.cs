using Newtonsoft.Json;
using System;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class YPLCalibration
    {
        /// <summary>
        /// this method is the exact copy of YPLCalibrationFromRheometer.Model.YPLCalibration.Copy(YPLCalibration dest)
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
                if (YPLModelMullineux != null)
                {
                    if (dest.YPLModelMullineux == null)
                        dest.YPLModelMullineux = new YPLModel();
                    YPLModelMullineux.Copy(dest.YPLModelMullineux);
                    if (dest.YPLModelMullineux.ID.Equals(Guid.Empty))
                        dest.YPLModelMullineux.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
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
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return values;
        }
    }
}
