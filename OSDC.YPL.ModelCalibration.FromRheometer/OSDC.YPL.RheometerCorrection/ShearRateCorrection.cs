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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kappa"></param>
        /// <param name="n"></param>
        /// <param name="nbOfPoints"></param>
        /// <returns></returns>
        private static double IntegrationFKappaN(double kappa, double n, int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (7):
            //$$\int_\kappa^1 \frac{1}{\tilde r} \left(\frac{1}{\tilde{r}^2} -1   \right)^{\frac 1n}d\tilde r  $$

            double step = (1.0 - kappa) / nbOfIntervals;
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = kappa + i * step;
                double x_r = kappa + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(1.0 / (x_l * x_l) - 1, oneOverN);
                double y_r = (1.0 / x_r) * System.Math.Pow(1.0 / (x_r * x_r) - 1, oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return integral;
        }
    }
}
