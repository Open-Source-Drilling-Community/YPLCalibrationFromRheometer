using System;
using Newtonsoft.Json;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class YPLModel
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public YPLModel() : base()
        {

        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public YPLModel(YPLModel src) : base()
        {
            if (src != null)
            {
                src.Copy(this);
            }
        }

        /// <summary>
        /// copy everything except the ID
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(YPLModel dest)
        {
            if (dest != null)
            {
                dest.Name = Name;
                dest.Description = Description;
                dest.Tau0 = Tau0;
                dest.K = K;
                dest.N = N;
                dest.Chi2 = Chi2;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize a YPLModel to Json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// deserialize a string that is expected to be in Json into an instance of YPLModel
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static YPLModel FromJson(string str)
        {
            YPLModel values = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    values = JsonConvert.DeserializeObject<YPLModel>(str);
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
