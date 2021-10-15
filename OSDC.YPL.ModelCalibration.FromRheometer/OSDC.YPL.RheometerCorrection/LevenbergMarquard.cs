using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDC.YPL.RheometerCorrection
{
    public interface ILevenbergMarquardInput
    {
        void Eval(double x, IList<double> input, ref double result, double[] epsilons, ref double[] dyda);
    }


    public static class LevenbergMarquard
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="function">The model whose coefficients have to be tuned</param>
        /// <param name="x">Typically the indices i for each measurement, x_i</param>
        /// <param name="y">"Measured" values to be matched</param>
        /// <param name="sigmas">Standard deviation attached to each parameter</param>
        /// <param name="epsilons">Epsilons used for derivative computations</param>
        /// <param name="initialGuess"></param>
        /// <param name="tol">Used</param>
        /// <param name="maxIterations"></param>
        /// <param name="selectParameters"></param>
        /// <returns></returns>
        public static IList<double> LeastSquare(ILevenbergMarquardInput function, IList<double> x, IList<double> y, IList<double> sigmas, double[] epsilons, IList<double> initialGuess, double tol, int maxIterations, bool[] selectParameters = null)
        {
            bool useDoubleMeaningForTolerance = false;

            if (selectParameters == null)
            {
                selectParameters = new bool[epsilons.Length];
                for (int i = 0; i < selectParameters.Length; i++)
                {
                    selectParameters[i] = true;
                }
            }

            int MAX_DONE = 4;
            int MAX_ITERATIONS = maxIterations;
            double alamda = 0.001;

            int ndat, ma, mfit, j, k, l, iter, done;
            done = 0;
            ndat = x.Count;
            ma = initialGuess.Count;





            mfit = selectParameters.Sum(b => b ? 1 : 0);

            double[] atry = new double[ma];
            double[] beta = new double[ma];
            double[] da = new double[ma];
            double[,] oneda = new double[mfit, 1];
            double[,] temp = new double[mfit, mfit];
            double[,] alpha = new double[ma, ma];
            double[,] covar = new double[ma, ma];
            double[] a = initialGuess.ToArray();
            double chiSquare = 0;

            mrqCof(function, x, y, sigmas, a, alpha, beta, ref chiSquare, ndat, ma, mfit, epsilons, selectParameters);
            for (j = 0; j < ma; j++)
            {
                atry[j] = a[j];
            }

            double oldChisq = chiSquare;

            for (iter = 0; iter < MAX_ITERATIONS; iter++)
            {
                if (done == MAX_DONE)
                {
                    alamda = 0;
                }

                for (j = 0; j < mfit; j++)
                {
                    for (k = 0; k < mfit; k++)
                    {
                        covar[j, k] = alpha[j, k];
                    }
                    covar[j, j] = alpha[j, j] * (1.0 + alamda);
                    for (k = 0; k < mfit; k++)
                    {
                        temp[j, k] = covar[j, k];
                    }
                    oneda[j, 0] = beta[j];
                }

                gaussJ(temp, oneda);

                for (j = 0; j < mfit; j++)
                {
                    for (k = 0; k < mfit; k++)
                    {
                        covar[j, k] = temp[j, k];
                    }
                    da[j] = oneda[j, 0];
                }
                if (done == MAX_DONE)
                {
                    covsrt(covar, mfit, ma, selectParameters);
                    covsrt(alpha, mfit, ma, selectParameters);
                    return a;
                }
                for (j = 0, l = 0; l < ma; l++)
                {
                    if (selectParameters[l])
                    {
                        atry[l] = a[l] + da[j++];
                    }
                }

                mrqCof(function, x, y, sigmas, atry, covar, da, ref chiSquare, ndat, ma, mfit, epsilons, selectParameters);
                if (System.Math.Abs(chiSquare - oldChisq) < tol || (useDoubleMeaningForTolerance && System.Math.Abs(chiSquare - oldChisq) < tol * chiSquare))
                {
                    done++;
                }

                if (chiSquare < oldChisq)
                {
                    alamda *= 0.1;
                    oldChisq = chiSquare;
                    for (j = 0; j < mfit; j++)
                    {
                        for (k = 0; k < mfit; k++)
                        {
                            alpha[j, k] = covar[j, k];
                        }
                        beta[j] = da[j];
                    }
                    for (l = 0; l < ma; l++)
                    {
                        a[l] = atry[l];
                    }
                }
                else
                {
                    alamda *= 10;
                    chiSquare = oldChisq;
                }
            }



            return null;
        }

        //private static void gaussJ(double[,] m, double[,] a)
        //{
        //    SquaredMatrix<double, DoubleCalculator> matrix = new SquaredMatrix<double, DoubleCalculator>(m.GetLength(0));
        //    Vector<double, DoubleCalculator> vector = new Vector<double, DoubleCalculator>(a.GetLength(0));
        //    for (int i = 0; i < m.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < m.GetLength(0); j++)
        //        {
        //            matrix[i, j] = m[j, i];//consider transposing the matrix as numerical recipes uses other conventions than us.
        //        }
        //    }
        //    matrix.InvertAssign();
        //    for (int i = 0; i < a.GetLength(1); i++)
        //    {
        //        for (int j = 0; j < a.GetLength(0); j++)
        //        {
        //            vector[j] = a[j, i];
        //        }

        //        IVector<double> ivector = new Vector<double, DoubleCalculator>(a.GetLength(0));
        //        matrix.Time(vector, ref ivector);
        //        for (int j = 0; j < a.GetLength(0); j++)
        //        {
        //            a[j, i] = ivector[j];
        //        }
        //    }
        //    for (int i = 0; i < m.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < m.GetLength(0); j++)
        //        {
        //            m[i, j] = matrix[j, i];//consider transposing the matrix as numerical recipes uses other conventions than us.
        //        }
        //    }
        //}


        private static void gaussJ(double[,] m, double[,] a)
        {
            var matrix =  MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(m);
            matrix.Inverse();
            var results = matrix.Multiply(MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.DenseOfArray(a));

            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    m[i, j] = matrix[i, j];
                }
            }

            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    a[i, j] = results[i, j];
                }
            }


            //SquaredMatrix<double, DoubleCalculator> matrix = new SquaredMatrix<double, DoubleCalculator>(m.GetLength(0));
            //Vector<double, DoubleCalculator> vector = new Vector<double, DoubleCalculator>(a.GetLength(0));
            //for (int i = 0; i < m.GetLength(0); i++)
            //{
            //    for (int j = 0; j < m.GetLength(0); j++)
            //    {
            //        matrix[i, j] = m[j, i];//consider transposing the matrix as numerical recipes uses other conventions than us.
            //    }
            //}
            //matrix.InvertAssign();
            //for (int i = 0; i < a.GetLength(1); i++)
            //{
            //    for (int j = 0; j < a.GetLength(0); j++)
            //    {
            //        vector[j] = a[j, i];
            //    }

            //    IVector<double> ivector = new Vector<double, DoubleCalculator>(a.GetLength(0));
            //    matrix.Time(vector, ref ivector);
            //    for (int j = 0; j < a.GetLength(0); j++)
            //    {
            //        a[j, i] = ivector[j];
            //    }
            //}
            //for (int i = 0; i < m.GetLength(0); i++)
            //{
            //    for (int j = 0; j < m.GetLength(0); j++)
            //    {
            //        m[i, j] = matrix[j, i];//consider transposing the matrix as numerical recipes uses other conventions than us.
            //    }
            //}
        }


        private static void covsrt(double[,] covar, int mfit, int ma, bool[] selectParameters)
        {
            int i, j, k;
            for (i = mfit; i < ma; i++)
            {
                for (j = 0; j < i + 1; j++)
                {
                    covar[i, j] = covar[j, i] = 0;
                }
            }
            k = mfit - 1;
            for (j = ma - 1; j >= 0; j--)
            {
                if (selectParameters[j])
                {
                    for (i = 0; i < ma; i++)
                    {
                        double temp = covar[i, k];
                        covar[i, k] = covar[i, j];
                        covar[i, j] = temp;
                    }
                    for (i = 0; i < ma; i++)
                    {
                        double temp = covar[k, i];
                        covar[k, i] = covar[j, i];
                        covar[j, i] = temp;
                    }
                    k--;
                }
            }
        }
        private static void mrqCof(ILevenbergMarquardInput function, IList<double> x, IList<double> y, IList<double> sigmas, IList<double> a, double[,] alpha, double[] beta, ref double chiSquare, int ndat, int ma, int mfit, double[] epsilons, bool[] selectParameters)
        {
            int i, j, k, l, m;
            double ymod, wt, sig2i, dy;
            ymod = double.NaN;

            double[] dyda = new double[ma];
            for (j = 0; j < mfit; j++)
            {
                for (k = 0; k <= j; k++)
                {
                    alpha[j, k] = 0;
                }
                beta[j] = 0;
            }
            chiSquare = 0;
            for (i = 0; i < ndat; i++)
            {
                function.Eval(x[i], a, ref ymod, epsilons, ref dyda);
                sig2i = 1.0 / (sigmas[i] * sigmas[i]);
                dy = y[i] - ymod;
                for (j = 0, l = 0; l < ma; l++)
                {
                    if (selectParameters[l])
                    {
                        wt = dyda[l] * sig2i;
                        for (k = 0, m = 0; m < l + 1; m++)
                        {
                            alpha[j, k++] += wt * dyda[m];
                        }
                        beta[j++] += dy * wt;
                    }
                }
                chiSquare += dy * dy * sig2i;
            }
            for (j = 1; j < mfit; j++)
            {
                for (k = 0; k < j; k++)
                {
                    alpha[k, j] = alpha[j, k];
                }
            }

            chiSquare /= ndat;
            chiSquare = System.Math.Sqrt(chiSquare);


        }


    }
}
