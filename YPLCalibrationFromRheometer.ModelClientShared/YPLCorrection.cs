using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
                if (RheogramInput != null)
                {
                    if (dest.RheogramInput == null)
                        dest.RheogramInput = new Rheogram();
                    RheogramInput.Copy(dest.RheogramInput);
                    if (dest.RheogramInput.ID.Equals(Guid.Empty))
                        dest.RheogramInput.ID = Guid.NewGuid(); // must be ID'ed for further update or addition to the database
                }
                if (YPLModelBasedOnNewtonianInputs != null)
                {
                    if (dest.YPLModelBasedOnNewtonianInputs == null)
                    {
                        dest.YPLModelBasedOnNewtonianInputs = new YPLModel();
                    }
                    YPLModelBasedOnNewtonianInputs.Copy(dest.YPLModelBasedOnNewtonianInputs);
                    if (dest.YPLModelBasedOnNewtonianInputs.ID.Equals(Guid.Empty))
                    {
                        dest.YPLModelBasedOnNewtonianInputs.ID = Guid.NewGuid();
                    }
                }
                if (RheogramFullyCorrected != null)
                {
                    if (dest.RheogramFullyCorrected == null)
                        dest.RheogramFullyCorrected = new List<ShearRateAndStress>();
                    foreach (var v in RheogramFullyCorrected)
                    {
                        dest.RheogramFullyCorrected.Add(v);
                    }
                }
                if (YPLModelFullyCorrected != null)
                {
                    if (dest.YPLModelFullyCorrected == null)
                    {
                        dest.YPLModelFullyCorrected = new YPLModel();
                    }
                    YPLModelFullyCorrected.Copy(dest.YPLModelFullyCorrected);
                    if (dest.YPLModelFullyCorrected.ID.Equals(Guid.Empty))
                    {
                        dest.YPLModelFullyCorrected.ID = Guid.NewGuid();
                    }
                }
                if (RheogramShearRateCorrected != null)
                {
                    if (dest.RheogramShearRateCorrected == null)
                        dest.RheogramShearRateCorrected = new List<ShearRateAndStress>();
                    foreach (var v in RheogramShearRateCorrected)
                    {
                        dest.RheogramShearRateCorrected.Add(v);
                    }
                }
                if (YPLModelShearRateCorrected != null)
                {
                    if (dest.YPLModelShearRateCorrected == null)
                    {
                        dest.YPLModelShearRateCorrected = new YPLModel();
                    }
                    YPLModelShearRateCorrected.Copy(dest.YPLModelShearRateCorrected);
                    if (dest.YPLModelShearRateCorrected.ID.Equals(Guid.Empty))
                    {
                        dest.YPLModelShearRateCorrected.ID = Guid.NewGuid();
                    }
                }
                if (RheogramShearStressCorrected != null)
                {
                    if (dest.RheogramShearStressCorrected == null)
                        dest.RheogramShearStressCorrected = new List<ShearRateAndStress>();
                    foreach (var v in RheogramShearStressCorrected)
                    {
                        dest.RheogramShearStressCorrected.Add(v);
                    }
                }
                if (YPLModelShearStressCorrected != null)
                {
                    if (dest.YPLModelShearStressCorrected == null)
                    {
                        dest.YPLModelShearStressCorrected = new YPLModel();
                    }
                    YPLModelShearStressCorrected.Copy(dest.YPLModelShearStressCorrected);
                    if (dest.YPLModelShearStressCorrected.ID.Equals(Guid.Empty))
                    {
                        dest.YPLModelShearStressCorrected.ID = Guid.NewGuid();
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
