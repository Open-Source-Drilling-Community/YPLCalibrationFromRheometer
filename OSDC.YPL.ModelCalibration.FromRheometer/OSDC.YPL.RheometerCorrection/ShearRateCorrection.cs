using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.RheometerCorrection
{
    public static class ShearRateCorrection
    {
        /// <summary>
        ///	Skadsem and Saasen(2019) :
        ///	Concentric cylinder viscometer flows ofHerschel-Bulkley fluids
        ///	
        /// Solving the Couette inverse problem
        ///using a wavelet-vaguelette decomposition
        ///Christophe Ancey
        /// </summary>
        /// <param name="uncorrectedRheogram"></param>
        /// <param name="correctedRheogram"></param>
        /// <returns></returns>
    public static bool CorrectShearRate(Rheogram uncorrectedRheogram, out Rheogram correctedRheogram)
        {
            correctedRheogram = new Rheogram();


            return true;
        }
    
    
    }
}
