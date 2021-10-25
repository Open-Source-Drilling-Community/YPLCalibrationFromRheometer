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


        public static double GetShearRate(double r1, double r2, double k, double n, double tau_y, double omega, out bool isFullySheared)
        {
            double minV = FindMinimumRotationalVelocity(tau_y, k, r1 / r2, n);
            if (omega > minV)
            {
                isFullySheared = true;
                return GetFullyShearedShearRate(r1, r2, k, n, tau_y, omega);
            }
            else
            {
                isFullySheared = false;
                return GetNonFullyShearedShearRate(r1, r2, k, n, tau_y, omega);
            }
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

        public static double GetNewtonianShearRate(double omega, double kappa)
        {
            return 2 * omega / (1 - kappa * kappa);
        }

        public static double CalculateRPlug(double k, double omega, double tau_y, double n, double r1, double r2)
        {
            // Newton-Raphson scheme is used to determine Rplug from Eq. (9)
            double epsilon = 1e-16; // for low values of n (0.2), the value of integral in Eq. (9) around 1e-13 or less: reduce espilon if evaluating even lower values of n
            double kappa = r1 / r2;
            double leftHandSide = omega * System.Math.Pow(k / tau_y, 1.0 / n);

            double rp = .5 * (r1 + r2);
            double diff = leftHandSide - IntegrationEquation9(rp, kappa, n, r2);

            double rpPrime = rp * 1.01;
            double diffPrime = leftHandSide - IntegrationEquation9(rpPrime, kappa, n, r2);

            double derivative = (diffPrime - diff) / (rpPrime - rp);

            int count = 0;

            while (System.Math.Abs(diff) > epsilon && count++ < 100)
            {
                rp -= diff / derivative;
                diff = leftHandSide - IntegrationEquation9(rp, kappa, n, r2);
                rpPrime = rp * 1.01;
                diffPrime = leftHandSide - IntegrationEquation9(rpPrime, kappa, n, r2);
                derivative = (diffPrime - diff) / (rpPrime - rp);
            }
            if (count > 70)
            {
                System.Diagnostics.Debug.WriteLine("Numerical error in CalculateRPlug");
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
        public static double CalculateC(double k, double omega, double tau_y, double n, double r1, double r2)
        {
            //tex: Finds the constant of integration $c$ from equation (8):
            //$$ \left(\frac{k \Omega^{ n}}{\tau_y} \right)^\frac 1n = \int_\kappa^1 \frac{1}{\tilde r} \left(\frac{\tau(\tilde{r}R_2)}{ \tau_y}   -1   \right)^{\frac 1n}d\tilde r  $$
            //with $\tau(r) = c / r^2$
            //tex: The existence and unicity of C for any value of n is guaranteed by the analysis of the variations of Eq. (8)
            // as a function f of C and Leibnitz' theorem.
            // It can be shown that 1) f(C) strictly increases over $ ]R_2^2 \tau_y, +\infty[ $
            // and 2) C exists if:
            //$$ \Omega > \Omega^\star = (\frac \tau k)^{\frac 1n} * f(\kappa,n) $$
            
            // Newton-Raphson scheme is used to determine C from Eq. (8)

            double kappa = r1 / r2;
            double leftHandSide =omega* System.Math.Pow( k / tau_y, 1.0 / n);

            //tex: for all non integer values of 1/n, the definition interval of the intragel is limited to: $[R_2^2 * \tau_y, +\infty[$ 
            
            //tex: interestingly for n=0.5, C can take 2 values. The higher one is the true solution since the lower one comes from the
            // square elevation of Eq. (3b) to obtain Eq. (6)

            double c = 1.01 * r2 * r2 * tau_y;
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
            if (count > 40)
            {
                System.Diagnostics.Debug.WriteLine("Numerical error in CalculateC");
            }
            return c;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="k"></param>
        /// <param name="tau_y"></param>
        /// <param name="n"></param>
        /// <param name="r1"></param>
        /// <param name="r"></param>
        /// <param name="sr"></param>
        /// <param name="nbOfIntervals"></param>
        /// <returns></returns>
        public static double IntegrationEquation6(double c, double k, double tau_y, double n, double r1, double r, int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (6):
            //$$\int_{R_1}^r \frac{\dot\gamma(r)}{r}dr $$

            double step = (r - r1) / (nbOfIntervals);
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = r1 + i * step;
                double x_r = r1 + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(System.Math.Abs(c / (tau_y * x_l * x_l) - 1), oneOverN); // absolute value avoids rounding errors near operand's nullity
                double y_r = (1.0 / x_r) * System.Math.Pow(System.Math.Abs(c / (tau_y * x_r * x_r) - 1), oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return System.Math.Pow(tau_y / k, oneOverN) * integral;
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
        public static double IntegrationEquation8(double c, double kappa, double n,double tau_y, double r2,  int nbOfIntervals = 100)
        {
            //tex: Performs the integration from equation (8):
            //$$\int_\kappa^1 \frac{1}{\tilde r} \left(\frac{c}{\tilde{r}^2 R_2^2 \tau_y}   -1   \right)^{\frac 1n}d\tilde r  $$

            double step = (1.0 - kappa) / (nbOfIntervals);
            double oneOverN = 1.0 / n;

            double integral = 0;

            for (int i = 0; i < nbOfIntervals; i++)
            {
                double x_l = kappa + i * step;
                double x_r = kappa + (i + 1) * step;
                double y_l = (1.0 / x_l) * System.Math.Pow(System.Math.Abs(c / (tau_y * r2 * r2 * x_l * x_l) - 1), oneOverN); // absolute value avoids rounding errors near operand's nullity
                double y_r = (1.0 / x_r) * System.Math.Pow(System.Math.Abs(c / (tau_y * r2 * r2 * x_r * x_r) - 1), oneOverN);

                integral += (y_l + y_r) * .5 * step;
            }
            return integral;
        }


        public static double IntegrationEquation9(double rPlug, double kappa, double n, double r2, int nbOfIntervals = 100)
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
                double y_l = (1.0 / x_l) * System.Math.Pow(System.Math.Abs(rPlug * rPlug / (r2 * r2 * x_l * x_l) - 1), oneOverN); // absolute value avoids rounding errors near operand's nullity
                double y_r = (1.0 / x_r) * System.Math.Pow(System.Math.Abs(rPlug * rPlug / (r2 * r2 * x_r * x_r) - 1), oneOverN);

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
