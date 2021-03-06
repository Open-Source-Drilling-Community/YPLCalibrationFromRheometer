using Newtonsoft.Json;
using System;

namespace YPLCalibrationFromRheometer.ModelClientShared
{
    public partial class YPLCorrection
    {
        /// <summary>
        /// this method is the exact copy of YPLCorrectionFromRheometer.Model.YPLCorrection.Copy(YPLCorrection dest)
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(YPLCorrection dest)
        {
            if (dest != null)
            {
                dest.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                dest.Name = Name;
                dest.Description = Description;
                dest.R1 = R1;
                dest.R2 = R2;
                if (RheogramInput != null)
                {
                    if (dest.RheogramInput == null)
                        dest.RheogramInput = new Rheogram();
                    RheogramInput.Copy(dest.RheogramInput);
                    if (dest.RheogramInput.ID.Equals(Guid.Empty))
                        dest.RheogramInput.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (RheogramFullyCorrected != null)
                {
                    if (dest.RheogramFullyCorrected == null)
                        dest.RheogramFullyCorrected = new Rheogram();
                    RheogramFullyCorrected.Copy(dest.RheogramFullyCorrected);
                    if (dest.RheogramFullyCorrected.ID.Equals(Guid.Empty))
                        dest.RheogramFullyCorrected.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (RheogramShearRateCorrected != null)
                {
                    if (dest.RheogramShearRateCorrected == null)
                        dest.RheogramShearRateCorrected = new Rheogram();
                    RheogramShearRateCorrected.Copy(dest.RheogramShearRateCorrected);
                    if (dest.RheogramShearRateCorrected.ID.Equals(Guid.Empty))
                        dest.RheogramShearRateCorrected.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (RheogramShearStressCorrected != null)
                {
                    if (dest.RheogramShearStressCorrected == null)
                        dest.RheogramShearStressCorrected = new Rheogram();
                    RheogramShearStressCorrected.Copy(dest.RheogramShearStressCorrected);
                    if (dest.RheogramShearStressCorrected.ID.Equals(Guid.Empty))
                        dest.RheogramShearStressCorrected.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize a YPLCorrection to Json
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// deserialize a string that is expected to be in Json into an instance of YPLCorrection
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static YPLCorrection FromJson(string str)
        {
            YPLCorrection values = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    values = JsonConvert.DeserializeObject<YPLCorrection>(str);
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
