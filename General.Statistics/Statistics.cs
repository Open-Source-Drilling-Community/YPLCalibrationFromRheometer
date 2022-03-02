using System;
using System.Collections.Generic;
using NORCE.General.Std;
using NORCE.General.Math;

namespace NORCE.General.Statistics
{
    public static class Statistics
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double Average(IList<double> data)
        {
            if (data != null && data.Count > 0)
            {
                double sum = 0;
                foreach (double val in data)
                {
                    sum += val;
                }
                return sum / data.Count;
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }

        public static double Min(IList<double> data)
        {
            if (data != null && data.Count > 0)
            {
                double min = Numeric.MAX_DOUBLE;
                foreach (double val in data)
                {
                    if (val < min)
                    {
                        min = val;
                    }
                }
                return min;
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }
        public static double Max(IList<double> data)
        {
            if (data != null && data.Count > 0)
            {
                double max = Numeric.MIN_DOUBLE;
                foreach (double val in data)
                {
                    if (val > max)
                    {
                        max = val;
                    }
                }
                return max;
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }


        /// <summary>
        ///       
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double StandardDeviation(IList<double> data)
        {
            if (data != null && data.Count > 0)
            {
                double total, sum;
                total = sum = 0;
                foreach (double val in data)
                {
                    sum += val;
                    total += val * val;
                }
                sum /= data.Count;
                return Numeric.SqrtEqual(total / data.Count - sum * sum);
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }

        /// <summary>
        /// Returns the median of the list. 
        /// 
        /// Sorts the list and returns element number n/2
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double Median(List<double> data)
        {
            if (data != null && data.Count >= 2)
            {
                data.Sort();
                if (data.Count % 2 == 0)
                {
                    return (data[data.Count / 2 - 1] + data[data.Count / 2]) / 2;
                }
                else
                {
                    return data[data.Count / 2];
                }
            }
            return Numeric.UNDEF_DOUBLE;
        }
        /// <summary>
        /// Returns the median of the list, taking weights into account
        /// </summary>
        /// <param name="weightedData">List of pairs (value, weight)</param>
        /// <returns></returns>
        public static double Median(List<Pair<double, double>> weightedData)
        {
            if (weightedData != null && weightedData.Count > 1)
            {
                double medianWeight = 0;
                for (int i = 0; i < weightedData.Count; i++)
                {
                    medianWeight += weightedData[i].Right;
                }
                medianWeight /= 2;
                weightedData.Sort(PairComparer);
                double temp = 0;
                for (int i = 0; i < weightedData.Count; i++)
                {
                    temp += weightedData[i].Right;
                    if (Numeric.GE(temp, medianWeight))
                    {
                        return weightedData[i].Left;
                    }
                }
                return weightedData[weightedData.Count - 1].Left;
            }
            return Numeric.UNDEF_DOUBLE;
        }
        private static int PairComparer(Pair<double, double> p1, Pair<double, double> p2)
        {
            return Comparer<double>.Default.Compare(p1.Left, p2.Left);
        }

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
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static double ChiSquare(IList<Pair<double, double>> data, IValuable obj)
        {
            if (data != null)
            {
                double chi2 = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    double temp = obj.Eval(data[i].Left);
                    chi2 += (temp - data[i].Right) * (temp - data[i].Right);
                }
                return chi2;
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }

        public static double ChiSquare(double[] yEstimated, double[] yMeasured)
        {
            double chi2 = 0;
            if (yEstimated != null && yMeasured != null && yEstimated.Length == yMeasured.Length)
            {
                for (int i = 0; i < yMeasured.Length; i++)
                {
                    chi2 += (yEstimated[i] - yMeasured[i]) * (yEstimated[i] - yMeasured[i]);
                }
            }
            return chi2;
        }

        public static double ChiSquare(double[] yEstimated, double[] yMeasured, double std)
        {
            if (!Numeric.EQ(std, 0))
            {
                double chi2 = 0;
                if (yEstimated != null && yMeasured != null && yEstimated.Length == yMeasured.Length)
                {
                    for (int i = 0; i < yMeasured.Length; i++)
                    {
                        chi2 += (yEstimated[i] - yMeasured[i]) * (yEstimated[i] - yMeasured[i]) / (std * std);
                    }
                }
                return chi2;
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }

        public static double ChiSquare(double[] yEstimated, double[] yMeasured, double[] std)
        {
            if (std == null)
            {
                return ChiSquare(yEstimated, yMeasured);
            }
            else
            {
                double chi2 = 0;
                if (yEstimated != null && yMeasured != null && yEstimated.Length == yMeasured.Length)
                {
                    for (int i = 0; i < yMeasured.Length; i++)
                    {
                        if (!Numeric.EQ(std[i], 0))
                        {
                            chi2 += (yEstimated[i] - yMeasured[i]) * (yEstimated[i] - yMeasured[i]) / (std[i] * std[i]);
                        }
                    }
                }
                return chi2;
            }
        }
    }
}
