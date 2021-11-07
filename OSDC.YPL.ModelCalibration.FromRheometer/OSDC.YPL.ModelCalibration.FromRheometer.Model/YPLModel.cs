using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    /// <summary>
    /// Description of a Yield Power Law rheological behavior (or Herschel-Bulkley)
    /// tau = Tau0 + K*gamma_dot^n
    /// </summary>
    public class YPLModel : IValuable
    {
        public enum ModelType{ YPL, PL , N};

        /// <summary>
        /// Yield stress expected in SI unit, i.e., [ML^-1T^-2](Pa)
        /// </summary>
        [Display(Name = "Yield stress (Pa)")]
        [DisplayFormat(
               ApplyFormatInEditMode = false,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double Tau0 { get; set; } = 0;
        /// <summary>
        /// Consistency index in SI unit, i.e., ML^-1T^(n-2)](Pa.s^n)
        /// </summary>
        [Display(Name = "Consistency index (Pa.s^n)")]
        [DisplayFormat(
               ApplyFormatInEditMode = false,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double K { get; set; } = 1;
        /// <summary>
        /// flow behavior index (dimensionless)
        /// </summary>
        [Display(Name = "Flow behavior index")]
        [DisplayFormat(
               ApplyFormatInEditMode = false,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double n { get; set; } = 1;
        /// <summary>
        /// chi-square value corresponding to fitting of the model
        /// A negative chi-square means that the value is undefined.
        /// </summary>
        [Display(Name = "Chi-square")]
        [DisplayFormat(
               ApplyFormatInEditMode = false,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double Chi2 { get; set; } = -1;
        /// <summary>
        /// the reference rheogram used to calibrate the rheological behavior
        /// </summary>
        public Rheogram Rheogram { get; set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        public YPLModel()
        {

        }
        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="source"></param>
        public YPLModel(YPLModel source)
        {
            if (source != null)
            {
                Tau0 = source.Tau0;
                K = source.K;
                n = source.n;
                Chi2 = source.Chi2;
            }
        }
        /// <summary>
        /// evaluation of the shear stress as a function of the shear rate
        /// </summary>
        /// <param name="shearRate"></param>
        /// <returns></returns>
        public double Eval(double shearRate)
        {
            return Tau0 + K * System.Math.Pow(shearRate, n);
        }
        /// <summary>
        /// Fit the YPL rheological behavior to the rheogram data using the method from Zamora/Kelessidis
        /// (see https://doi.org/10.1016/j.petrol.2006.06.004)
        /// </summary>
        /// <param name="rheogram"></param>
        /// <returns>the chi-square after fitting</returns>
        public void FitToKelessidis(Rheogram rheogram, ModelType model = ModelType.YPL)
        {
            Tau0 = 0;
            K = 1;
            n = 1;
            Chi2 = -1;
            if (rheogram != null && rheogram.Measurements != null && rheogram.Measurements.Count >= 3)
            {
                // find the min shear rate
                int i0 = -1;
                double minShearRate = double.MaxValue;
                double minShearStress = 0;
                for (int i = 0; i < rheogram.Measurements.Count; i++)
                {
                    if ( rheogram.Measurements[i].ShearRate < minShearRate)
                    {
                        minShearRate = rheogram.Measurements[i].ShearRate;
                        minShearStress = rheogram.Measurements[i].ShearStress;
                        i0 = i;
                    }
                }
                // find the next min shear rate
                int i1 = -1;
                double nextMinShearRate = double.MaxValue;
                for (int i = 0; i < rheogram.Measurements.Count; i++)
                {
                    if (rheogram.Measurements[i].ShearRate < nextMinShearRate && rheogram.Measurements[i].ShearRate > minShearRate)
                    {
                        nextMinShearRate = rheogram.Measurements[i].ShearRate;
                        i1 = i;
                    }
                }
                if (i0 >= 0 && i1 >= 0 && i0 != i1)
                {
                    // an initial guess for the yield stress is found using a straight line between i1 and i0.
                    double tau0 = rheogram.Measurements[i0].ShearStress - rheogram.Measurements[i0].ShearRate * (rheogram.Measurements[i1].ShearStress - rheogram.Measurements[i0].ShearStress) / (rheogram.Measurements[i1].ShearRate - rheogram.Measurements[i0].ShearRate);
                    // use a Newton-Raphson method to find d(chi2)/d(tau0) = 0
                    double sig = rheogram.ShearStressStandardDeviation;
                    if (sig <= 0)
                    {
                        sig = 0.01;
                    }
                    double[] gammas = new double[rheogram.Measurements.Count];
                    double[] sigs = new double[rheogram.Measurements.Count];
                    double[] taus = new double[rheogram.Measurements.Count];
                    List<double> logGammas = new List<double>();
                    List<double> logTaus = new List<double>();
                    for (int i = 0; i < rheogram.Measurements.Count; i++)
                    {
                        gammas[i] = rheogram.Measurements[i].ShearRate;
                        logGammas.Add(Math.Log(rheogram.Measurements[i].ShearRate));
                        logTaus.Add(0);
                        taus[i] = rheogram.Measurements[i].ShearStress;
                        sigs[i] = sig;
                    }

                    double eps = 1e-6;
                    int count = 0;
                    double K_, n_, chi2_;
                    double derivateChi2a = FP(tau0, logGammas, logTaus, gammas, taus, sigs,  out K_, out n_, out chi2_);
                    do
                    {
                        double tau0b = tau0 + 1e-3 * tau0;
                        if (Math.Abs(tau0) < 1e-3)
                        {
                            tau0b = 1e-3;
                        }
                        double derivateChi2b = FP(tau0b, logGammas, logTaus, gammas, taus, sigs, out K_, out n_, out chi2_);
                        double differential = (derivateChi2b - derivateChi2a) / (tau0b - tau0);
                        tau0 -= derivateChi2a / differential;
                        if (tau0 > minShearStress)
                        {
                            tau0 = 0.99 * minShearStress;
                        }
                        derivateChi2a = FP(tau0, logGammas, logTaus, gammas, taus, sigs, out K_, out n_, out chi2_);
                    } while (System.Math.Abs(derivateChi2a) > eps && count++ < 50);
                    if (!Numeric.IsUndefined(tau0) && !Numeric.IsUndefined(K_) && !Numeric.IsUndefined(n_) && !Numeric.IsUndefined(chi2_))
                    {
                        Tau0 = (model == ModelType.N || model == ModelType.PL) ? 0 : tau0;
                        K = K_;
                        n = model == ModelType.N ? 1 : n_;
                        Chi2 = chi2_;
                    }
                }
            }
        }
        /// <summary>
        /// Fit the YPL rheological behavior to the rheogram using the method from Mullineux
        /// (see https://doi.org/10.1016/j.apm.2007.09.010)
        /// </summary>
        /// <param name="rheogram"></param>
        /// <returns>The chi-square after fitting</returns>
        public void FitToMullineux(Rheogram rheogram, ModelType model = ModelType.YPL)
        {
            Tau0 = 0;
            K = 1;
            n = 1;
            Chi2 = -1;
            if (rheogram != null && rheogram.Measurements != null && rheogram.Measurements.Count >= 3)
            {
                // determine n
                // first attempt with a Newton-Raphson method
                double eps = 1e-5;
                int count = 0;
                double n0 = 1.0;
                double fn0 = F(rheogram.Measurements, n0);
                do
                {
                    double dn = n0 / 1000.0;
                    double fn1 = F(rheogram.Measurements, n0 + dn);
                    double slope = (fn1 - fn0) / dn;
                    if (!Numeric.EQ(slope, 0, 1e-9))
                    {
                        n0 -= fn0 / slope;
                        fn0 = F(rheogram.Measurements, n0);
                    }
                }
                while (System.Math.Abs(fn0) > eps && count++ < 50);
                if (System.Math.Abs(fn0) > eps)
                {
                    // if the Newton-Raphson method has failed, we try with a bisection method between 0.01 and 1.0
                    n0 = 0.01;
                    fn0 = F(rheogram.Measurements, n0);
                    double n1 = 1.0;
                    double fn1 = F(rheogram.Measurements, n1);
                    if (fn0 * fn1 < 0)
                    {
                        count = 0;
                        do
                        {
                            double nx = (n0 + n1) / 2.0;
                            double fnx = F(rheogram.Measurements, nx);
                            if (fn0 * fnx < 0)
                            {
                                n1 = nx;
                                fn1 = fnx;
                            }
                            else
                            {
                                n0 = nx;
                                fn0 = fnx;
                            }
                        } while (System.Math.Abs(fn0) > eps && count++ < 50);
                    }
                }
                if (Numeric.EQ(System.Math.Abs(fn0), 0.0, eps))
                {
                    // when n is found we just fit tau0 and K using a linear regression
                    double sig = rheogram.ShearStressStandardDeviation;
                    if (sig <= 0)
                    {
                        sig = 0.01;
                    }
                    double tau0, k;
                    double[] xs = new double[rheogram.Measurements.Count];
                    double[] gammas = new double[rheogram.Measurements.Count];
                    double[] taus = new double[rheogram.Measurements.Count];
                    double[] sigs = new double[rheogram.Measurements.Count];
                    for (int i = 0; i < rheogram.Measurements.Count; i++)
                    {
                        gammas[i] = rheogram.Measurements[i].ShearRate;
                        xs[i] = System.Math.Pow(rheogram.Measurements[i].ShearRate, n0);
                        taus[i] = rheogram.Measurements[i].ShearStress;
                        sigs[i] = sig;
                    }
                    DataModelling.LinearRegression(xs, taus, out tau0, out k);
                    Tau0 = (model == ModelType.N || model == ModelType.PL) ? 0 : tau0;
                    K = k;
                    n = (model == ModelType.N) ? 1 : n0;
                    Chi2 = DataModelling.ChiSquare(gammas, taus, sigs, this);
                }
            }
        }

        private double FP(double tau0, List<double> logGammas, List<double> logTaus, double[] gammas, double[] taus, double[] sigs,  out double K_, out double n_, out double chi2_)
        {
            for (int i = 0; i < taus.Length; i++)
            {
                if ((taus[i] - tau0) <= 0)
                {
                    logTaus[i] = -1e9;
                }
                else
                {
                    logTaus[i] = System.Math.Log(taus[i] - tau0);
                }
            }
            double logK;
            DataModelling.LinearRegression(logGammas, logTaus, out logK, out n_);
            K_ = Math.Exp(logK);

            double tau0Backup = Tau0;
            double KBackUp = K;
            double nBackUp = n;
            Tau0 = tau0;
            K = K_;
            n = n_;
            chi2_ = DataModelling.ChiSquare(gammas, taus, sigs, this);

            double tau0b = tau0 + 1e-3 * tau0;
            if (System.Math.Abs(tau0) < 1e-3)
            {
                tau0b = 1e-3;
            }
            for (int i = 0; i < taus.Length; i++)
            {
                if ((taus[i] - tau0) <= 0)
                {
                    logTaus[i] = -1e9;
                }
                else
                {
                    logTaus[i] = System.Math.Log(taus[i] - tau0b);
                }
            }
            double logKb, nb;
            DataModelling.LinearRegression(logGammas, logTaus, out logKb, out nb);
            Tau0 = tau0b;
            K = Math.Exp(logKb);
            n = nb;
            double chi2b = DataModelling.ChiSquare(gammas, taus, sigs, this);
            double derivative = (chi2b - chi2_) / (tau0b - tau0);
            Tau0 = tau0Backup;
            K = KBackUp;
            n = nBackUp;
            return derivative;
        }

        private double F(List<RheometerMeasurement> samples, double n)
        {
            if (samples != null)
            {
                double a00 = samples.Count;
                double a01 = 0.0;
                for (int i = 0; i < samples.Count; i++)
                {
                    a01 += System.Math.Pow(samples[i].ShearRate, n);
                }

                double a02 = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    a02 += System.Math.Log(samples[i].ShearRate) * System.Math.Pow(samples[i].ShearRate, n);
                }
                double a10 = a01;
                double a11 = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    a11 += System.Math.Pow(samples[i].ShearRate, 2.0 * n);
                }
                double a12 = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    a12 += System.Math.Log(samples[i].ShearRate) * System.Math.Pow(samples[i].ShearRate, 2.0 * n);
                }
                double a20 = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    a20 += samples[i].ShearStress;
                }
                double a21 = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    a21 += System.Math.Pow(samples[i].ShearRate, n) * samples[i].ShearStress;
                }
                double a22 = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    a22 += System.Math.Log(samples[i].ShearRate) * System.Math.Pow(samples[i].ShearRate, n) * samples[i].ShearStress;
                }
                return a00 * a11 * a22 + a10 * a21 * a02 + a20 * a01 * a12 - (a20 * a11 * a02 + a00 * a21 * a12 + a10 * a01 * a22);
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }

    }
}
