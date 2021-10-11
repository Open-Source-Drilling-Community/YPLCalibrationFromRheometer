using System;
using System.Collections.Generic;
using System.Text;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    public class Numeric
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly double UNDEF_DOUBLE = System.Double.NaN;
        /// <summary>
        /// 
        /// </summary>
        public static readonly double MAX_DOUBLE = System.Double.MaxValue;

        /// <summary>
        /// 
        /// </summary>
        public static readonly double MIN_DOUBLE = System.Double.MinValue;

        /// <summary>
        /// Checks if (d1-d2) is greater than acc
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool GT(double d1, double d2, double acc)
        {
            return d1 - acc > d2;
        }

        /// <summary>
        /// Checks if |d2-d1| is less than acc.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool EQ(double d1, double d2, double acc)
        {
            return (Numeric.IsUndefined(d1) && Numeric.IsUndefined(d2)) || System.Math.Abs(d2 - d1) < acc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsUndefined(double d)
        {
            return System.Double.IsNaN(d);
        }

        /// <summary>
        /// Power calculation based on an integral exponent
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Pow(double a, int n)
        {
            double value = 1.0;
            int n1 = (n < 0) ? -n : n;
            for (int i = 0; i < n1; i++)
            {
                value *= a;
            }
            if (n < 0)
            {
                value = 1 / value;
            }
            return value;
        }

    }
}
