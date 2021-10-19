using System;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.RheometerCorrection
{
    public static class ShearStressCorrection
    {
        /// <summary>
        /// Non-Newtonian end-effects in standard oilfield rheometers
        /// Etienne Lac and Andrew Parry
        /// 
        /// 
        /// Solving the Couette inverse problem
        /// using a wavelet-vaguelette decomposition
        /// Christophe Ancey
        /// <param name="uncorrectedRheogram"></param>
        /// <param name="correctedRheogram"></param>
        /// <returns></returns>
        public static bool CorrectShearStress(Rheogram uncorrectedRheogram, out Rheogram correctedRheogram)
        {
            correctedRheogram = new Rheogram();


            return true;
        }
    }
}
