using System;
using System.Collections.Generic;
using System.Text;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    public class DataModelling
    {
        /// <summary>
        /// Calculates the chi square for a fit y(i) = func(x[i]), with standard
        /// deviations std[i]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="std"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ChiSquare(double[] x, double[] y, double[] std, IValuable obj)
        {
            double chi2 = 0;
            if (x != null && y != null && std != null && x.Length == y.Length && x.Length == std.Length)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    chi2 += Numeric.Pow((y[i] - obj.Eval(x[i])) / std[i], 2);
                }
            }
            return chi2;
        }

        /// <summary>
        /// Given a list of points (x_i, y_i),
        /// this routines computes the line y = a+bx, by linear regression technique, 
        /// so that the chi-square (ca. the distance between the curve and the line ) 
        /// is minimal.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void LinearRegression(IList<double> xs, IList<double> ys, out double a, out double b)
        {
            if (xs != null && ys != null && xs.Count > 1 && xs.Count == ys.Count)
            {
                if (xs.Count == 2)
                {
                    if (!Numeric.EQ(xs[0], xs[1], 1e-8))
                    {
                        b = (ys[0] - ys[1]) / (xs[0] - xs[1]);
                    }
                    else
                    {
                        b = 0;
                    }
                    a = ys[0] - b * xs[0];
                }
                else
                {
                    int n = xs.Count;
                    double t, sxoss, syoss, sx, sy, st2, ss;
                    sx = sy = b = 0.0;
                    st2 = 0.0;
                    ss = n;
                    for (int i = 0; i < n; i++)
                    {
                        sx += xs[i];
                        sy += ys[i];
                    }
                    sxoss = sx / ss;
                    syoss = sy / ss;
                    for (int i = 0; i < n; i++)
                    {
                        t = xs[i] - sxoss;
                        st2 += t * t;
                        b += t * (ys[i] - syoss);
                    }
                    b /= st2;
                    a = (sy - sx * b) / ss;
                }
            }
            else
            {
                a = Numeric.UNDEF_DOUBLE;
                b = Numeric.UNDEF_DOUBLE;
            }
        }
    }
}
