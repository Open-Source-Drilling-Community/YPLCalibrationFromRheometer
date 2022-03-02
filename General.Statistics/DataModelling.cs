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
    }
}
