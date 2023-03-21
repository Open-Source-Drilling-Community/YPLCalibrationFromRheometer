using OSDC.DotnetLibraries.General.Common;
using OSDC.DotnetLibraries.General.Math;
using OSDC.DotnetLibraries.General.Statistics;
using System;
using System.Collections.Generic;

namespace YPLCalibrationFromRheometer.Model
{
    /// <summary>
    /// Description of a Yield Power Law rheological behavior (or Herschel-Bulkley)
    /// tau = Tau0 + K*gamma_dot^N
    /// </summary>
    public class YPLModel : ICloneable, IValuable
    {
        public enum ModelType { YPL, PL, N };

        /// <summary>
        /// an ID for the Rheogram
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// a name for the Rheogram
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// a description for the Rheogram
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Yield stress expected in SI unit, i.e., [ML^-1T^-2](Pa)
        /// </summary>
        public double Tau0 { get; set; } = 0;

        /// <summary>
        /// Consistency index in SI unit, i.e., ML^-1T^(N-2)](Pa.s^N)
        /// </summary>
        public double K { get; set; } = 1;

        /// <summary>
        /// flow behavior index (dimensionless)
        /// </summary>
        public double N { get; set; } = 1;

        /// <summary>
        /// chi-square value corresponding to fitting of the model
        /// A negative chi-square means that the value is undefined.
        /// </summary>
        public double Chi2 { get; set; } = -1;

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
        /// cloning function (including the ID)
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            YPLModel copy = new YPLModel(this)
            {
                ID = ID
            };
            return copy;
        }

        /// <summary>
        /// evaluation of the shear stress as a function of the shear rate
        /// </summary>
        /// <param name="shearRate"></param>
        /// <returns></returns>
        public double Eval(double shearRate)
        {
            return Tau0 + K * System.Math.Pow(shearRate, N);
        }

        /// <summary>
        /// evaluation of the shear stress as a function of the shear rate and external parameters
        /// </summary>
        /// <param name="shearRate">a shear rate value</param>
        /// <param name="p">an array of YPL model parameters (tau0, K, n)</param>
        /// <returns></returns>
        public double ShearStressFromParameters(double shearRate, params double[] p)
        {
            return p[0] + p[1] * System.Math.Pow(shearRate, p[2]);
        }

        #region Kelessidis method
        /// <summary>
        /// Fit the YPL rheological behavior to the rheogram data using the method from Zamora/Kelessidis
        /// (see https://doi.org/10.1016/j.petrol.2006.06.004)
        /// </summary>
        /// <param name="rheogram">the input rheogram used for the YPL model calibration</param>
        /// <param name="model">the type of YPL model to be calibrated (N, PL or YPL)</param>
        /// <returns>the chi-square after fitting</returns>
        public void FitToKelessidis(Rheogram rheogram, ModelType model = ModelType.YPL)
        {
            Tau0 = 0;
            K = 1;
            N = 1;
            Chi2 = -1;
            if (rheogram != null && rheogram.RheometerMeasurementList != null && rheogram.RheometerMeasurementList.Count >= 3)
            {
                List<RheometerMeasurement> rheoMeasList = rheogram.RheometerMeasurementList;
                int rheoMeasCount = rheoMeasList.Count;

                // find the min shear rate
                int i0 = -1;
                double minShearRate = double.MaxValue;
                double minShearStress = 0;
                for (int i = 0; i < rheoMeasCount; i++)
                {
                    if (rheoMeasList[i].ShearRate < minShearRate)
                    {
                        minShearRate = rheoMeasList[i].ShearRate;
                        minShearStress = rheoMeasList[i].ShearStress;
                        i0 = i;
                    }
                }
                // find the next min shear rate
                int i1 = -1;
                double nextMinShearRate = double.MaxValue;
                for (int i = 0; i < rheoMeasCount; i++)
                {
                    if (rheoMeasList[i].ShearRate < nextMinShearRate && rheoMeasList[i].ShearRate > minShearRate)
                    {
                        nextMinShearRate = rheoMeasList[i].ShearRate;
                        i1 = i;
                    }
                }
                if (i0 >= 0 && i1 >= 0 && i0 != i1)
                {
                    // an initial guess for the yield stress is found using a straight line between i1 and i0.
                    double tau0 = rheoMeasList[i0].ShearStress - rheoMeasList[i0].ShearRate * (rheoMeasList[i1].ShearStress - rheoMeasList[i0].ShearStress) / (rheoMeasList[i1].ShearRate - rheoMeasList[i0].ShearRate);
                    // use a Newton-Raphson method to find d(chi2)/d(tau0) = 0
                    double sig = rheogram.ShearStressStandardDeviation;
                    if (sig <= 0)
                    {
                        sig = 0.01;
                    }
                    double[] gammas = new double[rheoMeasCount];
                    double[] sigs = new double[rheoMeasCount];
                    double[] taus = new double[rheoMeasCount];
                    List<double> logGammas = new List<double>();
                    List<double> logTaus = new List<double>();
                    for (int i = 0; i < rheoMeasCount; i++)
                    {
                        gammas[i] = rheoMeasList[i].ShearRate;
                        logGammas.Add(Math.Log(rheoMeasList[i].ShearRate));
                        logTaus.Add(0);
                        taus[i] = rheoMeasList[i].ShearStress;
                        sigs[i] = sig;
                    }

                    double eps = 1e-6;
                    int count = 0;
                    double derivateChi2a = FKelessidis(tau0, logGammas, logTaus, gammas, taus, sigs, out _, out _, out _);
                    double K_;
                    double n_;
                    double chi2_;
                    do
                    {
                        double tau0b = tau0 + 1e-3 * tau0;
                        if (Math.Abs(tau0) < 1e-3)
                        {
                            tau0b = 1e-3;
                        }
                        double derivateChi2b = FKelessidis(tau0b, logGammas, logTaus, gammas, taus, sigs, out _, out _, out _);
                        double differential = (derivateChi2b - derivateChi2a) / (tau0b - tau0);
                        tau0 -= derivateChi2a / differential;
                        if (tau0 > minShearStress)
                        {
                            tau0 = 0.99 * minShearStress;
                        }
                        derivateChi2a = FKelessidis(tau0, logGammas, logTaus, gammas, taus, sigs, out K_, out n_, out chi2_);
                    } while (System.Math.Abs(derivateChi2a) > eps && count++ < 50);
                    if (!Numeric.IsUndefined(tau0) && !Numeric.IsUndefined(K_) && !Numeric.IsUndefined(n_) && !Numeric.IsUndefined(chi2_))
                    {
                        Tau0 = (model == ModelType.N || model == ModelType.PL) ? 0 : tau0;
                        K = K_;
                        N = model == ModelType.N ? 1 : n_;
                        Chi2 = chi2_;
                    }
                }
            }
        }

        private double FKelessidis(double tau0, List<double> logGammas, List<double> logTaus, double[] gammas, double[] taus, double[] sigs, out double K_, out double n_, out double chi2_)
        {
            List<Pair<double, double>> logGammas_and_logTaus = new List<Pair<double, double>>();
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
                logGammas_and_logTaus.Add(new Pair<double, double>(logGammas[i], logTaus[i])); // TODO: complicated data management: simpler would be to add signatures with arrays in Statistics and DataModelling
            }
            double logK;
            Pair<double, double> logK_and_n_ = DataModelling.LinearRegression(logGammas_and_logTaus); // TODO: complicated data management: simpler would be to add signatures with arrays in Statistics and DataModelling
            logK = logK_and_n_.Left;
            n_ = logK_and_n_.Right;
            K_ = Math.Exp(logK);

            double tau0Backup = Tau0;
            double KBackUp = K;
            double nBackUp = N;
            Tau0 = tau0;
            K = K_;
            N = n_;
            chi2_ = Statistics.ChiSquare(gammas, taus, sigs, this);

            logGammas_and_logTaus.Clear();
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
                logGammas_and_logTaus.Add(new Pair<double, double>(logGammas[i], logTaus[i]));
            }
            double logKb, nb; // TODO: these variables are not outed so could be avoided
            Pair<double, double> logKb_and_nb = DataModelling.LinearRegression(logGammas_and_logTaus); // TODO: complicated data management: simpler would be to add signatures with arrays in Statistics and DataModelling
            logKb = logKb_and_nb.Left;
            nb = logKb_and_nb.Right;
            Tau0 = tau0b;
            K = Math.Exp(logKb);
            N = nb;
            double chi2b = Statistics.ChiSquare(gammas, taus, sigs, this);
            double derivative = (chi2b - chi2_) / (tau0b - tau0);
            Tau0 = tau0Backup;
            K = KBackUp;
            N = nBackUp;
            return derivative;
        }
        #endregion

        #region Mullineux method
        /// <summary>
        /// Fit the YPL rheological behavior to the rheogram using the method from Mullineux
        /// (see https://doi.org/10.1016/j.apm.2007.09.010)
        /// </summary>
        /// <param name="rheogram">the input rheogram used for the YPL model calibration</param>
        /// <param name="model">the type of YPL model to be calibrated (N, PL or YPL)</param>
        /// <returns>The chi-square after fitting</returns>
        public void FitToMullineux(Rheogram rheogram, ModelType model = ModelType.YPL)
        {
            Chi2 = -1;
            if (rheogram != null && rheogram.RheometerMeasurementList != null && rheogram.RheometerMeasurementList.Count >= 3)
            {
                List<RheometerMeasurement> rheoMeasList = rheogram.RheometerMeasurementList;
                int rheoMeasCount = rheoMeasList.Count;

                // determine N
                // first attempt with a Newton-Raphson method
                double eps = 1e-5;
                int count = 0;
                double n0 = 1.0;
                double fn0 = FMullineux(rheoMeasList, n0);
                do
                {
                    double dn = n0 / 1000.0;
                    double fn1 = FMullineux(rheoMeasList, n0 + dn);
                    double slope = (fn1 - fn0) / dn;
                    if (!Numeric.EQ(slope, 0, 1e-9))
                    {
                        n0 -= fn0 / slope;
                        fn0 = FMullineux(rheoMeasList, n0);
                    }
                }
                while (System.Math.Abs(fn0) > eps && count++ < 50);
                if (System.Math.Abs(fn0) > eps)
                {
                    // if the Newton-Raphson method has failed, we try with a bisection method between 0.01 and 1.0
                    n0 = 0.01;
                    fn0 = FMullineux(rheoMeasList, n0);
                    double n1 = 1.0;
                    if (fn0 * FMullineux(rheoMeasList, n1) < 0)
                    {
                        count = 0;
                        do
                        {
                            double nx = (n0 + n1) / 2.0;
                            double fnx = FMullineux(rheoMeasList, nx);
                            if (fn0 * fnx < 0)
                            {
                                n1 = nx;
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
                    // when N is found we just fit tau0 and K using a linear regression
                    double sig = rheogram.ShearStressStandardDeviation;
                    if (sig <= 0)
                    {
                        sig = 0.01;
                    }
                    double tau0, k;
                    double[] taus = new double[rheoMeasCount];
                    List<Pair<double, double>> xs_and_taus = new List<Pair<double, double>>(); // TODO: complicated data management: simpler would be to add signatures with arrays in Statistics and DataModelling
                    double[] gammas = new double[rheoMeasCount];
                    double[] sigs = new double[rheoMeasCount];
                    for (int i = 0; i < rheoMeasCount; i++)
                    {
                        taus[i] = rheoMeasList[i].ShearStress;
                        xs_and_taus.Add(new Pair<double, double>(System.Math.Pow(rheoMeasList[i].ShearRate, n0), taus[i]));
                        gammas[i] = rheoMeasList[i].ShearRate;
                        sigs[i] = sig;
                    }
                    Pair<double, double> tau0_and_k = DataModelling.LinearRegression(xs_and_taus);
                    tau0 = tau0_and_k.Left;
                    k = tau0_and_k.Right;

                    Tau0 = (model == ModelType.N || model == ModelType.PL) ? 0 : tau0;
                    K = k;
                    N = (model == ModelType.N) ? 1 : n0;
                    Chi2 = Statistics.ChiSquare(gammas, taus, sigs, this);
                }
            }
        }

        private double FMullineux(List<RheometerMeasurement> samples, double n)
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
        #endregion

        #region Levenberg-Marquardt algorithm
        /// <summary>
        /// Fit the YPL rheological behavior to the rheogram data using the Levenberg-Marquardt algorithm
        /// (see https://people.duke.edu/~hpgavin/ce281/lm.pdf)
        /// </summary>
        /// <param name="rheogram">the input rheogram used for the YPL model calibration</param>
        /// <param name="model">the type of YPL model to be calibrated (N, PL or YPL)</param>
        /// <returns>the chi-square after fitting</returns>
        public void FitToLevenbergMarquardt(Rheogram rheogram, ModelType model = ModelType.YPL)
        {
            Tau0 = 1;
            K = 1;
            N = 1;

            double chisq;
            List<RheometerMeasurement> rheoMeasList = rheogram.RheometerMeasurementList;
            int rheoMeasCount = rheoMeasList.Count;

            double[] gammas = new double[rheoMeasCount];
            double[] taus = new double[rheoMeasCount];
            double[] sigs = new double[rheoMeasCount];
            bool[] ia = new bool[rheoMeasCount];
            for (int i = 0; i < rheoMeasCount; i++)
            {
                gammas[i] = rheoMeasList[i].ShearRate;
                taus[i] = rheoMeasList[i].ShearStress;
                sigs[i] = rheogram.ShearStressStandardDeviation;
                ia[i] = true;
            }
            double[] p = new double[3]; //output Tau0, K, n
            p[0] = Tau0; //Tau0
            p[1] = K; //K
            p[2] = N; //n

            DataModelling.UnaryFunctionDelegate func = new DataModelling.UnaryFunctionDelegate(ShearStressFromParameters);
            DataModelling.NonLinearFitting(gammas, taus, sigs, p, ia, func, out chisq, 10.0, 5000);
            if (Numeric.IsDefined(chisq))
            {
                Tau0 = p[0];
                K = p[1];
                N = p[2];
                double[] tausEstimated = new double[taus.Length];
                for (int i = 0; i < taus.Length; i++)
                {
                    tausEstimated[i] = ShearStressFromParameters(gammas[i], p);
                }
                Chi2 = Statistics.ChiSquare(tausEstimated, taus, sigs);
            }
        }
        #endregion
    }
}