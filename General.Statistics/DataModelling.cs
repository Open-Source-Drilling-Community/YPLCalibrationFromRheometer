using System.Collections.Generic;
using NORCE.General.Std;
using NORCE.General.Math;

namespace NORCE.General.Statistics
{
    public static class DataModelling
    {
        private static List<Pair<double, double>> TEMP_ = new List<Pair<double, double>>();

        /// <summary>
        /// Given a list of points (x_i, y_i), and the sequence of standard deviations associated to the measurments
        /// of the y_i's, this routines computes the line y = a+bx, by linear regression technique, 
        /// so that the chi-square (ca. the distance between the curve and the line ) 
        /// is minimal.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="standardDeviations"></param>
        /// <returns></returns>
        public static Pair<double, double> LinearRegression(IList<Pair<double, double>> data, double[] standardDeviations)
        {
            if (data != null && data.Count > 1 && standardDeviations != null && standardDeviations.Length == data.Count)
            {
                if (data.Count == 2)
                {
                    double a, b;
                    if (!Numeric.EQ(data[0].Left, data[1].Left, 1e-8))
                    {
                        b = (data[0].Right - data[1].Right) / (data[0].Left - data[1].Left);
                    }
                    else
                    {
                        b = 0;
                    }
                    a = data[0].Right - b * data[0].Left;
                    return new Pair<double, double>(a, b);

                }
                else
                {
                    int n = data.Count;
                    double t, sxoss, sx, wt, sy, st2, ss, a, b;
                    sx = sy = b = 0.0;
                    st2 = 0.0;
                    ss = (double)n;
                    for (int i = 0; i < n; i++)
                    {

                        wt = 1 / (standardDeviations[i] * standardDeviations[i]);
                        ss += wt;
                        sx += data[i].Left * wt;
                        sy += data[i].Right * wt;
                    }
                    sxoss = sx / ss;
                    for (int i = 0; i < n; i++)
                    {
                        t = (data[i].Left - sxoss) / standardDeviations[i];
                        st2 += t * t;
                        b += t * data[i].Right / standardDeviations[i];
                    }
                    b /= st2;
                    a = (sy - sx * b) / ss;

                    return new Pair<double, double>(a, b);
                }
            }
            else
            {
                return new Pair<double, double>(Numeric.UNDEF_DOUBLE, Numeric.UNDEF_DOUBLE);
            }
        }
        /// <summary>
        /// Given a list of points (x_i, y_i),
        /// this routines computes the line y = a+bx, by linear regression technique, 
        /// so that the chi-square (ca. the distance between the curve and the line ) 
        /// is minimal.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Pair<double, double> LinearRegression(IList<Pair<double, double>> data)
        {
            if (data != null && data.Count > 1)
            {
                if (data.Count == 2)
                {
                    double a, b;
                    if (!Numeric.EQ(data[0].Left, data[1].Left, 1e-8))
                    {
                        b = (data[0].Right - data[1].Right) / (data[0].Left - data[1].Left);
                    }
                    else
                    {
                        b = 0;
                    }
                    a = data[0].Right - b * data[0].Left;
                    return new Pair<double, double>(a, b);
                }
                else
                {
                    int n = data.Count;
                    double t, sxoss, syoss, sx, sy, st2, ss, a, b;
                    sx = sy = b = 0.0;
                    st2 = 0.0;
                    ss = (double)n;
                    for (int i = 0; i < n; i++)
                    {
                        sx += data[i].Left;
                        sy += data[i].Right;
                    }
                    sxoss = sx / ss;
                    syoss = sy / ss;
                    for (int i = 0; i < n; i++)
                    {
                        t = data[i].Left - sxoss;
                        st2 += t * t;
                        b += t * (data[i].Right - syoss);
                    }
                    b /= st2;
                    a = (sy - sx * b) / ss;
                    return new Pair<double, double>(a, b);
                }
            }
            else
            {
                return new Pair<double, double>(Numeric.UNDEF_DOUBLE, Numeric.UNDEF_DOUBLE);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="polynomial"></param>
        public static void LinearRegression(IList<Pair<double, double>> data, ref LinearPolynom polynomial)
        {
            Pair<double, double> p = LinearRegression(data);
            polynomial.Set(p.Right, p.Left);
        }

        /// <summary>
        /// Delegate function to make an evaluation in a generic fitting algorithm such as Levenberg-Marquardt
        /// </summary>
        /// <param name="x">a unary function variable</param>
        /// <param name="list">an array of parameters of the function to minimize</param>
        /// <returns>the evaluated value that should be minimized</returns>
        public delegate double UnaryFunctionDelegate(double x, params double[] p);

        /// <summary>
        /// A generic implementation of the Levenberg-Marquardt algorithm
        /// </summary>
        /// <param name="xs">an array of variables for which the function has been "measured"</param>
        /// <param name="ys">an array of "measurements" corresponding to the variables</param>
        /// <param name="sigs">the array of experimental errors associated with each pair of variable/measurement</param>
        /// <param name="func">the delegate function</param>
        /// <param name="a">an array of parameters of the function to minimize</param>
        /// <param name="ia"></param>
        /// <returns>the evaluated value that should be minimized</returns>
        public static void NonLinearFitting(double[] xs, double[] ys, double[] sigs, double[] a, bool[] ia, UnaryFunctionDelegate func, out double chisq, double nabla = 10.0, int maxIter = 500)
        {
            double[,] covar = new double[a.Length, a.Length];
            double[,] alpha = new double[a.Length, a.Length];
            double alambda = -1;
            int mfit = 0;
            double[] atry = null;
            double[] beta = null;
            double[] da = null;
            double[,] oneda = null;

            // estimate chisquare with initial parameters
            double ichisq = 0;
            for (int i = 0; i < xs.Length; i++)
            {
                double ymod = func(xs[i], a);
                double sig2i = 1 / (sigs[i] * sigs[i]);
                double dy = ys[i] - ymod;
                ichisq += dy * dy * sig2i;
            }

            //initialization
            double ochisq = ichisq;
            atry = new double[a.Length];
            beta = new double[a.Length];
            da = new double[a.Length];
            mfit = 0;
            for (int j = 0; j < a.Length; j++)
            {
                atry[j] = 0;
                beta[j] = 0;
                da[j] = 0;
                if (ia[j])
                {
                    mfit++;
                }
            }
            oneda = new double[mfit, 1];
            for (int j = 0; j < mfit; j++)
            {
                oneda[j, 0] = 0;
            }
            mrqcof(xs, ys, sigs, a, ia, alpha, beta, out ochisq, func);
            for (int j = 0; j < a.Length; j++)
            {
                atry[j] = a[j];
            }
            // end initialization

            // test is we need to increase alambda
            alambda = 0.001;
            double chisq1, chisq2;
            int iter = 0;
            bool success = NonLinearFittingInitialTest(alambda, nabla, xs, ys, sigs, a, ia, func, mfit, covar, alpha, beta, oneda, da, atry, out chisq1, out chisq2);
            while (success && chisq1 > ichisq && chisq2 > ichisq && iter++ < 10)
            {
                alambda *= nabla;
                success = NonLinearFittingInitialTest(alambda, nabla, xs, ys, sigs, a, ia, func, mfit, covar, alpha, beta, oneda, da, atry, out chisq1, out chisq2);
            }
            if (success && (chisq1 <= ichisq || chisq2 <= ichisq))
            {
                bool stop = false;
                iter = 0;
                chisq = ichisq;
                double epsParams = 1e-6;
                double epsChisq = 1e-6;
                do
                {
                    ochisq = chisq;
                    NonLinearFitting(xs, ys, sigs, a, ia, covar, alpha, func, alambda, mfit, out chisq, atry, beta, da, oneda);
                    if (Numeric.IsUndefined(chisq))
                    {
                        break;
                    }
                    if (Numeric.LT(chisq, ochisq))
                    {
                        for (int j = 0; j < mfit; j++)
                        {
                            for (int k = 0; k < mfit; k++)
                            {
                                alpha[j, k] = covar[j, k];
                            }
                            beta[j] = da[j];
                        }
                        bool eq = true;
                        for (int l = 0; l < a.Length; l++)
                        {
                            eq &= Numeric.EQ(a[l], atry[l], epsParams);
                            a[l] = atry[l];
                        }
                        stop |= eq;
                        alambda /= nabla;
                    }
                    else
                    {
                        break;
                    }
                    stop |= Numeric.EQ(ochisq, chisq, epsChisq) || iter++ > maxIter;
                } while (!stop);
            }
            else
            {
                chisq = Numeric.UNDEF_DOUBLE;
            }
        }

        private static bool NonLinearFittingInitialTest(double alambda, double nabla, double[] xs, double[] ys, double[] sigs, double[] a, bool[] ia, UnaryFunctionDelegate func,
            int mfit, double[,] covar, double[,] alpha, double[] beta, double[,] oneda, double[] da, double[] atry, out double chisq1, out double chisq2)
        {
            // apply initial alambda
            for (int j = 0; j < mfit; j++)
            {
                for (int k = 0; k < mfit; k++)
                {
                    covar[j, k] = alpha[j, k];
                }
                covar[j, j] = alpha[j, j] * (1 + alambda);
                oneda[j, 0] = beta[j];
            }
            // matrix solution
            bool ok = gaussj(covar, mfit, oneda, 1);
            if (ok)
            {
                int j = 0;
                for (j = 0; j < mfit; j++)
                {
                    da[j] = oneda[j, 0];
                }
                j = 0;
                for (int l = 0; l < a.Length; l++)
                {
                    if (ia[l])
                    {
                        atry[l] = a[l] + da[j++];
                    }
                }
                mrqcof(xs, ys, sigs, atry, ia, covar, da, out chisq1, func);
                // apply lambda/nabla
                alambda /= nabla;
                // alter linearized fitting matrix by augmenting diagonal elements
                for (j = 0; j < mfit; j++)
                {
                    for (int k = 0; k < mfit; k++)
                    {
                        covar[j, k] = alpha[j, k];
                    }
                    covar[j, j] = alpha[j, j] * (1 + alambda);
                    oneda[j, 0] = beta[j];
                }
                // matrix solution
                ok = gaussj(covar, mfit, oneda, 1);
                if (ok)
                {
                    for (j = 0; j < mfit; j++)
                    {
                        da[j] = oneda[j, 0];
                    }
                    j = 0;
                    for (int l = 0; l < a.Length; l++)
                    {
                        if (ia[l])
                        {
                            atry[l] = a[l] + da[j++];
                        }
                    }
                    mrqcof(xs, ys, sigs, atry, ia, covar, da, out chisq2, func);
                    return true;
                }
                else
                {
                    chisq1 = chisq2 = Numeric.UNDEF_DOUBLE;
                    return false;
                }
            }
            else
            {
                chisq1 = chisq2 = Numeric.UNDEF_DOUBLE;
                return false;
            }
        }

        private static void NonLinearFitting(double[] x, double[] y, double[] sig, double[] a, bool[] ia, double[,] covar, double[,] alpha, UnaryFunctionDelegate func, double alamda, int mfit_, out double chisq,
           double[] atry_, double[] beta_, double[] da_, double[,] oneda_)
        {
            // alter linearized fitting matrix by augmenting diagonal elements
            for (int j = 0; j < mfit_; j++)
            {
                for (int k = 0; k < mfit_; k++)
                {
                    covar[j, k] = alpha[j, k];
                }
                covar[j, j] = alpha[j, j] * (1 + alamda);
                oneda_[j, 0] = beta_[j];
            }
            // matrix solution
            bool ok = gaussj(covar, mfit_, oneda_, 1);
            if (ok)
            {
                int j = 0;
                for (j = 0; j < mfit_; j++)
                {
                    da_[j] = oneda_[j, 0];
                }
                // did the trial succeed?
                j = 0;
                for (int l = 0; l < a.Length; l++)
                {
                    if (ia[l])
                    {
                        atry_[l] = a[l] + da_[j++];
                    }
                }
                mrqcof(x, y, sig, atry_, ia, covar, da_, out chisq, func);
            }
            else
            {
                //failure due to a singular matrix in gaussJordan. increase alamda
                chisq = Numeric.UNDEF_DOUBLE;
            }
        }

        /// <summary>
        /// Used by NonLinearFittin to evaluate the linearized fitting matrices alpha and beta, and calculate the chi square
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sig"></param>
        /// <param name="a"></param>
        /// <param name="ia"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="chisq"></param>
        /// <param name="func"></param>
        private static void mrqcof(double[] x, double[] y, double[] sig, double[] a, bool[] ia, double[,] alpha, double[] beta, out double chisq, UnaryFunctionDelegate func)
        {
            double[] dyda = new double[a.Length];
            int mfit = 0;
            for (int j = 0; j < a.Length; j++)
            {
                if (ia[j])
                {
                    mfit++;
                }
            }
            //initialize alpha and beta
            for (int j = 0; j < mfit; j++)
            {
                for (int k = 0; k <= j; k++)
                {
                    alpha[j, k] = 0;
                }
                beta[j] = 0;
            }
            chisq = 0;
            for (int i = 0; i < x.Length; i++)
            {
                double ymod = EvaluateAndEstimateDerivatives(func, x[i], a, dyda);
                double sig2i = 1 / (sig[i] * sig[i]);
                double dy = y[i] - ymod;
                int j = 0;
                for (int l = 0; l < a.Length; l++)
                {
                    if (ia[l])
                    {
                        double wt = dyda[l] * sig2i;
                        int k = 0;
                        for (int m = 0; m <= l; m++)
                        {
                            if (ia[m])
                            {
                                alpha[j, k++] += wt * dyda[m];
                            }
                        }
                        beta[j] += dy * wt;
                        j++;
                    }
                }
                chisq += dy * dy * sig2i;
            }
            for (int j = 1; j < mfit; j++)
            {
                for (int k = 0; k < j - 1; k++)
                {
                    alpha[k, j] = alpha[j, k];
                }
            }
        }
        private static double EvaluateAndEstimateDerivatives(UnaryFunctionDelegate func, double x, double[] a, double[] derivatives, double da = 0.001)
        {
            if (a != null && derivatives != null && a.Length == derivatives.Length)
            {
                double y0 = func(x, a);
                for (int i = 0; i < a.Length; i++)
                {
                    double delta = System.Math.Abs(da * a[i]);
                    a[i] -= delta;
                    double y1 = func(x, a);
                    a[i] += 2 * delta;
                    double y2 = func(x, a);
                    a[i] -= delta;
                    derivatives[i] = (y2 - y1) / (2 * delta);
                }
                return y0;
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }
        private static void covsrt(double[,] covar, bool[] ia, int mfit)
        {
            for (int i = mfit; i < ia.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    covar[i, j] = 0;
                    covar[j, i] = 0;
                }
            }
            int k = mfit > 0 ? mfit - 1 : 0;
            for (int j = ia.Length - 1; j >= 0; j--)
            {
                if (ia[j])
                {
                    for (int i = 0; i < ia.Length; i++)
                    {
                        double swap = covar[i, k];
                        covar[i, k] = covar[i, j];
                        covar[i, j] = swap;
                    }
                    for (int i = 0; i < ia.Length; i++)
                    {
                        double swap = covar[k, i];
                        covar[k, i] = covar[j, i];
                        covar[j, i] = swap;
                    }
                    k--;
                }
            }
        }

        /// <summary>
        /// Gauss-Jordan elimination
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <param name="b"></param>
        /// <param name="m"></param>
        private static bool gaussj(double[,] a, int n, double[,] b, int m)
        {
            // the integer arrays are used for bookkeeping on the pivoting
            int[] indxc = new int[n];
            int[] indxr = new int[n];
            int[] ipiv = new int[n];
            for (int j = 0; j < n; j++)
            {
                ipiv[j] = 0;
            }

            int irow = 0;
            int icol = 0;
            // this is the main loop over the columns to be reduced
            for (int i = 0; i < n; i++)
            {
                // loop to search the pivot
                double big = 0;
                for (int j = 0; j < n; j++)
                {
                    if (ipiv[j] != 1)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            if (ipiv[k] == 0)
                            {
                                if (Numeric.GE(System.Math.Abs(a[j, k]), big))
                                {
                                    big = System.Math.Abs(a[j, k]);
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (ipiv[k] > 1)
                            {
                                return false;
                            }
                        }
                    }
                }
                ipiv[icol]++;
                // we now have the pivot element, so we interchange rows, if needed, 
                // to put the pivot element on the diagonal. The columns are not physically interchanged,
                // only relabeled.
                if (irow != icol)
                {
                    for (int l = 0; l < n; l++)
                    {
                        double swap = a[irow, l];
                        a[irow, l] = a[icol, l];
                        a[icol, l] = swap;
                    }
                    for (int l = 0; l < m; l++)
                    {
                        double swap = b[irow, l];
                        b[irow, l] = b[icol, l];
                        b[icol, l] = swap;
                    }
                }
                indxr[i] = irow;
                indxc[i] = icol;
                // we are now ready to divide the pivot row by the pivot element
                // located at irow and icol
                if (Numeric.EQ(a[icol, icol], 0, 1e-14))
                {
                    return false;
                }
                double pivinv = 1 / a[icol, icol];
                a[icol, icol] = 1;
                for (int l = 0; l < n; l++)
                {
                    a[icol, l] *= pivinv;
                }
                for (int l = 0; l < m; l++)
                {
                    b[icol, l] *= pivinv;
                }
                //next we reduce the rows ... except for the pivor one, of course
                for (int ll = 0; ll < n; ll++)
                {
                    if (ll != icol)
                    {
                        double dum = a[ll, icol];
                        a[ll, icol] = 0;
                        for (int l = 0; l < n; l++)
                        {
                            a[ll, l] -= a[icol, l] * dum;
                        }
                        for (int l = 0; l < m; l++)
                        {
                            b[ll, l] -= b[icol, l] * dum;
                        }
                    }
                }
            }
            // this is the end of the main loop, It remains to unscramble the solution
            for (int l = n - 1; l >= 0; l--)
            {
                if (indxr[l] != indxc[l])
                {
                    for (int k = 0; k < n; k++)
                    {
                        double swap = a[k, indxr[l]];
                        a[k, indxr[l]] = a[k, indxc[l]];
                        a[k, indxc[k]] = swap;
                    }
                }
            }
            return true;
        }
    }
}
