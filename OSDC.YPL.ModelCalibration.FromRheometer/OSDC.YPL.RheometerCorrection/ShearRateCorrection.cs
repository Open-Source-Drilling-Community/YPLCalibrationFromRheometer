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
        /// <param name="tau_y"></param>
        /// <param name="k"></param>
        /// <param name="kappa"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double FindMinimumRotationalVelocity(double tau_y, double k, double kappa, double n)
        {
            //tex: Based on equation (7): 
            //$$ \left(\frac{k \Omega^{\star n}}{\tau_y} \right)^\frac 1n = \int_\kappa^1 \frac{1}{\tilde r} \left(\frac{1}{\tilde{r}^2} -1   \right)^{\frac 1n}d\tilde r  $$
           
            double integral = IntegrationFKappaN(kappa, n);
            double factor = System.Math.Pow(k / tau_y, 1.0 / n);
            return integral / factor;
        }

        public static double GetFullyShearedShearRate(double r1, double r2, double k, double n, double tau_y, double omega)
        {
            double c = CalculateC(k, omega, tau_y, n, r1, r2);


            //tex: $\tau(r) = c / r^2$
            double tau_r = c / (r1 * r1); 

            //tex: $$ \dot{\gamma}(r) = \left[ \left(\frac{\tau_y}{k} \right) \left( \frac{\tau (r)}{\tau_y} \right) -1 \right]^{ \frac 1n }$$
            //with $r = R_1$


            return System.Math.Pow((tau_y / k) * (tau_r / tau_y - 1.0), 1.0 / n);        
        }

        public static double GetNonFullyShearedShearRate(double r1, double r2, double k, double n, double tau_y, double omega)
        {
            double rPlug = CalculateRPlug(k, omega, tau_y, n, r1, r2);

            double tau_r = tau_y * rPlug * rPlug / (r1 * r1);

            return System.Math.Pow((tau_y / k) * (tau_r / tau_y - 1.0), 1.0 / n);
        }

        private static double CalculateRPlug(double k, double omega, double tau_y, double n, double r1, double r2)
        { 
            double kappa = r1 / r2;
            double leftHandSide = omega * System.Math.Pow(k / tau_y, 1.0 / n);

            double rp = .5 * (r1 + r2);


            double diff = leftHandSide - IntegrationEquation9(rp, kappa, n, tau_y, r2);

            double rpPrime = rp * 1.01;
            double diffPrime = leftHandSide - IntegrationEquation9(rpPrime, kappa, n, tau_y, r2);

            double derivative = (diffPrime - diff) / (rpPrime - rp);

            int count = 0;

            while (System.Math.Abs(diff) > 1e-8 && count++ < 50)
            {
                rp -= diff / derivative;
                diff = leftHandSide - IntegrationEquation9(rp, kappa, n, tau_y, r2);
                rpPrime = rp * 1.01;
                diffPrime = leftHandSide - IntegrationEquation9(rpPrime, kappa, n, tau_y, r2);
                derivative = (diffPrime - diff) / (rpPrime - rp);
            }
            return rp;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="omega"></param>
        /// <param name="tau_y"></param>
        /// <param name="n"></param>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        private static double CalculateC(double k, double omega, double tau_y, double n, double r1, double r2)
        {
            //tex: finds the constant of integration $c$ from equation (8):
            //$$ \left(\frac{k \Omega^{ n}}{\tau_y} \right)^\frac 1n = \int_\kappa^1 \frac{1}{\tilde r} \left(\frac{\tau(\tilde{r}R_2)}{ \tau_y}   -1   \right)^{\frac 1n}d\tilde r  $$
            //with $\tau(r) = c / r^2$



            double kappa = r1 / r2;

            double leftHandSide =omega* System.Math.Pow( k / tau_y, 1.0 / n);

            //tex: initialize the search with Newtonian value: $c = 2 \mu \Omega \kappa^2 R_2^2 / (1-\kappa^2)$, with $\mu = k$. 
            
            double c = 2 * k * omega * kappa * kappa * r2 * r2 / (1 - kappa * kappa);

            double diff = leftHandSide - IntegrationEquation8(c, kappa, n, tau_y, r2);

            double cPrime = c * 1.01;
            double diffPrime = leftHandSide - IntegrationEquation8(cPrime, kappa, n, tau_y, r2);

            double derivative = (diffPrime - diff) / (cPrime - c);

            int count = 0;

            while (System.Math.Abs(diff) > 1e-8 && count++ < 50)
            {
                c -= diff / derivative;
                diff = leftHandSide - IntegrationEquation8(c, kappa, n, tau_y, r2);
                cPrime = c * 1.01;
                diffPrime = leftHandSide - IntegrationEquation8(cPrime, kappa, n, tau_y, r2);
                derivative = (diffPrime - diff) / (cPrime - c);
            }
            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="kappa"></param>
        /// <param name="n"></param>
        /// <param name="tau_y"></param>
        /// <param name="r2"></param>
        /// <param name="nbOfIntervals"></param>
        /// <returns></returns>
        private static double IntegrationEquation8(double c, double kappa, double n,double tau_y, double r2,  int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (8):
            //$$\int_\kappa^1 \frac{1}{\tilde r} \left(\frac{c}{\tilde{r}^2 R_2 \tau_y}   -1   \right)^{\frac 1n}d\tilde r  $$

            double step = (1.0 - kappa) / nbOfIntervals;
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = kappa + i * step;
                double x_r = kappa + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(c / (r2 * x_l * x_l) - 1, oneOverN);
                double y_r = (1.0 / x_r) * System.Math.Pow(c / (r2 * x_r * x_r) - 1, oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return integral;
        }


        private static double IntegrationEquation9(double rPlug, double kappa, double n, double tau_y, double r2, int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (8):
            //$$\int_\kappa^{\kappa_p} \frac{1}{\tilde r} \left(\frac{R_P^2}{\tilde{r}^2 R_2 }   -1   \right)^{\frac 1n}d\tilde r  $$

            double kappaP = rPlug / r2;
            double step = (kappaP - kappa) / nbOfIntervals;
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = kappa + i * step;
                double x_r = kappa + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(rPlug * rPlug / (r2 *r2 * x_l * x_l) - 1, oneOverN);
                double y_r = (1.0 / x_r) * System.Math.Pow(rPlug * rPlug / (r2 * r2 * x_r * x_r) - 1, oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return integral;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="kappa"></param>
        /// <param name="n"></param>
        /// <param name="nbOfPoints"></param>
        /// <returns></returns>
        public static double IntegrationFKappaN(double kappa, double n, int nbOfIntervals = 100)
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
