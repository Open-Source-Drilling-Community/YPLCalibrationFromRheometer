using System;
using System.Collections.Generic;
using System.Globalization;

namespace NORCE.General.Std
{
    public class Numeric
    {
        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("en-US")
        /// </summary>
        public static CultureInfo US_CULTURE = new System.Globalization.CultureInfo("en-US");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("nb-NO")
        /// </summary>
        public static CultureInfo NO_CULTURE = new System.Globalization.CultureInfo("nb-NO");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("fr-FR")
        /// </summary>
        public static CultureInfo FR_CULTURE = new System.Globalization.CultureInfo("fr-FR");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("it-IT")
        /// </summary>
        public static CultureInfo IT_CULTURE = new System.Globalization.CultureInfo("it-IT");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("es-ES")
        /// </summary>
        public static CultureInfo SP_CULTURE = new System.Globalization.CultureInfo("es-ES");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("en-GB")
        /// </summary>
        public static CultureInfo UK_CULTURE = new System.Globalization.CultureInfo("en-GB");

        /// <summary>
        /// Shorthand for System.Globalization.CultureInfo("pt-PT")
        /// </summary>
        public static CultureInfo PT_CULTURE = new System.Globalization.CultureInfo("pt-PT");

        /// <summary>
        /// Shorthand for US_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo US_NUMBER_FORMAT = US_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for NO_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo NO_NUMBER_FORMAT = NO_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for FR_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo FR_NUMBER_FORMAT = FR_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for IT_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo IT_NUMBER_FORMAT = IT_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for SP_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo SP_NUMBER_FORMAT = SP_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for UK_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo UK_NUMBER_FORMAT = UK_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for PT_CULTURE.NumberFormat
        /// </summary>
        public static NumberFormatInfo PT_NUMBER_FORMAT = PT_CULTURE.NumberFormat;

        /// <summary>
        /// Shorthand for US_CULTURE.DateTimeFormat
        /// </summary>
        public static DateTimeFormatInfo US_DATETIME_FORMAT = US_CULTURE.DateTimeFormat;

        /// <summary>
        /// Shorthand for NO_CULTURE.DateTimeFormat
        /// </summary>
        public static DateTimeFormatInfo NO_DATETIME_FORMAT = NO_CULTURE.DateTimeFormat;

        /// <summary>
        /// Shorthand for FR_CULTURE.DateTimeFormat
        /// </summary>
        public static DateTimeFormatInfo FR_DATETIME_FORMAT = FR_CULTURE.DateTimeFormat;

        /// <summary>
        /// Shorthand for IT_CULTURE.DateTimeFormat
        /// </summary>
        public static DateTimeFormatInfo IT_DATETIME_FORMAT = IT_CULTURE.DateTimeFormat;

        /// <summary>
        /// Shorthand for SP_CULTURE.DateTimeFormat
        /// </summary>
        public static DateTimeFormatInfo SP_DATETIME_FORMAT = SP_CULTURE.DateTimeFormat;

        /// <summary>
        /// Shorthand for UK_CULTURE.DateTimeFormat
        /// </summary>
        public static DateTimeFormatInfo UK_DATETIME_FORMAT = UK_CULTURE.DateTimeFormat;

        /// <summary>
        /// Shorthand for PT_CULTURE.DateTimeFormat
        /// </summary>
        public static DateTimeFormatInfo PT_DATETIME_FORMAT = PT_CULTURE.DateTimeFormat;

        /// <summary>
        /// 
        /// </summary>
        public static readonly double NAN_DOUBLE = System.Double.NaN;

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
        /// Default tolerance in comparisons of type double numbers
        /// </summary>
        public static readonly double DOUBLE_ACCURACY = 1e-9;

        /// <summary>
        /// Default tolerance in comparisons of type TimeSpan
        /// </summary>
        public static readonly TimeSpan DATETIME_ACCURACY = new System.TimeSpan(0, 0, 0, 0, 1);
        /// <summary>
        /// Default accuracy for depth
        /// </summary>
        public static readonly double DEPTH_ACCURACY = 1e-3;
        /// <summary>
        /// 
        /// </summary>
        public static readonly double ANGLE_ACCURACY = 0.01 * PI / 180;

        /// <summary>
        /// NORCE-specific definition of undefined value.
        /// </summary>
        public static readonly DateTime UNDEF_DATETIME = DateTime.MaxValue;

        /// <summary>
        /// NORCE-specific definition of undefined value.
        /// </summary>
        public static readonly float NAN_FLOAT = System.Single.NaN;

        /// <summary>
        /// NORCE-specific definition of undefined value.
        /// </summary>
        public static readonly float UNDEF_FLOAT = System.Single.NaN;

        /// <summary>
        /// NORCE-specific definition of extreme value.
        /// </summary>
        public static readonly float MAX_FLOAT = System.Single.MaxValue;

        /// <summary>
        /// NORCE-specific definition of extreme value.
        /// </summary>
        public static readonly float MIN_FLOAT = System.Single.MinValue;

        /// <summary>
        /// 
        /// </summary>
        public static readonly float FLOAT_ACCURACY = 1e-6f;

        /// <summary>
        /// NORCE-specific definition of extreme value.
        /// </summary>
        public static readonly short MIN_SHORT = System.Int16.MinValue;

        /// <summary>
        /// NORCE-specific definition of extreme value.
        /// </summary>
        public static readonly short MAX_SHORT = System.Int16.MaxValue;

        /// <summary>
        /// NORCE-specific definition of extreme value.
        /// </summary>
        public static readonly int MIN_INT = System.Int32.MinValue;

        /// <summary>
        /// NORCE-specific definition of extreme value.
        /// </summary>
        public static readonly int MAX_INT = System.Int32.MaxValue;

        /// <summary>
        /// NORCE-specific definition of extreme value.
        /// </summary>
        public static readonly long MIN_LONG = System.Int64.MinValue;

        /// <summary>
        /// NORCE-specific definition of extreme value.
        /// </summary>
        public static readonly long MAX_LONG = System.Int64.MaxValue;

        /// <summary>
        /// NORCE-specific definition of ratio of circumference of circle to diameter.
        /// </summary>
        public static readonly double PI = System.Math.PI;

        /// <summary>
        /// NORCE-specific definition of Euler constant.
        /// </summary>
        public static readonly double E = System.Math.E;

        public static readonly double ZERO_CELSIUS = 273.15;

        public static readonly double ATMOSPHERIC_PRESSURE = 101325;
        /// <summary>
        /// NORCE-specific definition of the Euler-Mascheroni constant
        /// </summary>
        public static readonly double EULER_MASCHERONI_CONSTANT = 0.577215664901532; //Accuracy within 15 decimal places

        /// <summary>
        /// Acceleration due to Earth's gravity at mean sea level (m/s^2).
        /// </summary>
        public static readonly double G = 9.80665;

        public static readonly double STEEL_MASS_DENSITY = 7840.0;

        public static readonly double FANN35_CONSTANT = 1.703;

        public static readonly double ANTON_PAAR_CONSTANT = 1.288;

        public static readonly double ANTON_PAAR_BOB_DIAMETER = 0.026658;

        public static readonly double ANTON_PAAR_CUP_DIAMETER = 0.028920;

        /// <summary>
        /// Checks if d1 is significantly less than d2, i.e. by more than DOUBLE_ACCURACY.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool LT(double d1, double d2)
        {
            return d1 + DOUBLE_ACCURACY < d2;
        }

        public static bool LT(double? d1, double? d2)
        {
            if (d1 != null && d2 != null)
            {
                return d1 + DOUBLE_ACCURACY < d2;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if d2 is significantly greater than d1, i.e. by more than DOUBLE_ACCURACY.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool LE(double d1, double d2)
        {
            return d1 - DOUBLE_ACCURACY <= d2;
        }

        public static bool LE(double? d1, double? d2)
        {
            if (d1 != null && d2 != null)
            {
                return d1 - DOUBLE_ACCURACY <= d2;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if d1 is significantly greater than d2, i.e. by more than DOUBLE_ACCURACY.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool GT(double d1, double d2)
        {
            return d1 - DOUBLE_ACCURACY > d2;
        }

        public static bool GT(double? d1, double? d2)
        {
            if (d1 != null && d2 != null)
            {
                return d1 - DOUBLE_ACCURACY > d2;
            } 
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if d2 is significantly less than d1, i.e. by more than DOUBLE_ACCURACY.
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool GE(double d1, double d2)
        {
            return d1 + DOUBLE_ACCURACY >= d2;
        }

        public static bool GE(double? d1, double? d2)
        {
            if (d1 != null && d2 != null)
            {
                return d1 + DOUBLE_ACCURACY >= d2;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if |d2-d1| is less than DOUBLE_ACCURACY
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool EQ(double d1, double d2)
        {
            return (Numeric.IsUndefined(d1) && Numeric.IsUndefined(d2)) || System.Math.Abs(d2 - d1) < DOUBLE_ACCURACY;
        }

        public static bool EQ(double? d1, double? d2)
        {
            return (Numeric.IsUndefined(d1) && Numeric.IsUndefined(d2)) || System.Math.Abs((double)d2 - (double)d1) < DOUBLE_ACCURACY;
        }
        /// <summary>
        /// Checks if (d1-d2) is less than acc
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool LT(double d1, double d2, double acc)
        {
            return d1 + acc < d2;
        }

        public static bool LT(double? d1, double? d2, double acc)
        {
            if (d1 != null && d2 != null)
            {
                return d1 + acc < d2;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if (d1-d2) is less than or equal to acc
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool LE(double d1, double d2, double acc)
        {
            return d1 - acc <= d2;
        }

        public static bool LE(double? d1, double? d2, double acc)
        {
            if (d1 != null && d2 != null)
            {
                return d1 - acc <= d2;
            } else
            {
                return false;
            }
        }
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

        public static bool GT(double? d1, double? d2, double acc)
        {
            if (d1 != null && d2 != null)
            {
                return d1 - acc > d2;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks if (d1-d2) is greater or equal to acc
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool GE(double d1, double d2, double acc)
        {
            return d1 + acc >= d2;
        }

        public static bool GE(double? d1, double? d2, double acc)
        {
            if (d1 != null && d2 != null)
            {
                return d1 + acc >= d2;
            }
            else
            {
                return false;
            }
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

        public static bool EQ(double? d1, double? d2, double acc)
        {
            if (d1 != null && d2 != null)
            {
                return (Numeric.IsUndefined(d1) && Numeric.IsUndefined(d2)) || System.Math.Abs((double)d2 - (double)d1) < acc;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool EQ(DateTime d1, DateTime d2)
        {
            return EQ(d1, d2, DATETIME_ACCURACY);
        }

        public static bool EQ(DateTime? d1, DateTime? d2)
        {
            return EQ(d1, d2, DATETIME_ACCURACY);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool EQ(DateTime d1, DateTime d2, TimeSpan acc)
        {
            return EQ(d1, d2, acc.TotalSeconds);
        }

        public static bool EQ(DateTime? d1, DateTime? d2, TimeSpan acc)
        {
            return EQ(d1, d2, acc.TotalSeconds);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool EQ(DateTime d1, DateTime d2, double acc)
        {
            return (IsUndefined(d1) && IsUndefined(d2)) || System.Math.Abs((d2 - d1).TotalSeconds) <= acc;
        }

        public static bool EQ(DateTime? d1, DateTime? d2, double acc)
        {
            return (IsUndefined(d1) && IsUndefined(d2)) || System.Math.Abs(((DateTime)d2 - (DateTime)d1).TotalSeconds) <= acc;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool GT(DateTime d1, DateTime d2)
        {
            return GT(d1, d2, DATETIME_ACCURACY);
        }

        public static bool GT(DateTime? d1, DateTime? d2)
        {
            return GT(d1, d2, DATETIME_ACCURACY);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool GT(DateTime d1, DateTime d2, double acc)
        {
            return GT(d1, d2, TimeSpan.FromSeconds(acc));
        }

        public static bool GT(DateTime? d1, DateTime? d2, double acc)
        {
            return GT(d1, d2, TimeSpan.FromSeconds(acc));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool GT(DateTime d1, DateTime d2, TimeSpan acc)
        {
            return (IsMin(d2, acc) && !IsMin(d1, acc)) ||
                   (IsMax(d1, acc) && !IsMax(d2, acc)) ||
                   (!IsUndefined(d1, acc) && !IsUndefined(d2, acc) && (d1 - acc).CompareTo(d2) > 0);
        }

        public static bool GT(DateTime? d1, DateTime? d2, TimeSpan acc)
        {
            return (IsMin(d2, acc) && !IsMin(d1, acc)) ||
                   (IsMax(d1, acc) && !IsMax(d2, acc)) ||
                   (!IsUndefined(d1, acc) && !IsUndefined(d2, acc) && ((DateTime)d1 - acc).CompareTo(d2) > 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool GE(DateTime d1, DateTime d2)
        {
            return GE(d1, d2, DATETIME_ACCURACY);
        }

        public static bool GE(DateTime? d1, DateTime? d2)
        {
            return GE(d1, d2, DATETIME_ACCURACY);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool GE(DateTime d1, DateTime d2, double acc)
        {
            return GE(d1, d2, TimeSpan.FromSeconds(acc));
        }

        public static bool GE(DateTime? d1, DateTime? d2, double acc)
        {
            return GE(d1, d2, TimeSpan.FromSeconds(acc));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool GE(DateTime d1, DateTime d2, TimeSpan acc)
        {
            return IsMax(d1, acc) ||
                   IsMin(d2, acc) ||
                   (d1 + acc).CompareTo(d2) >= 0;
        }

        public static bool GE(DateTime? d1, DateTime? d2, TimeSpan acc)
        {
            return IsMax(d1, acc) ||
                   IsMin(d2, acc) ||
                   ((DateTime)d1 + acc).CompareTo(d2) >= 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool LT(DateTime d1, DateTime d2)
        {
            return LT(d1, d2, DATETIME_ACCURACY);
        }

        public static bool LT(DateTime? d1, DateTime? d2)
        {
            return LT(d1, d2, DATETIME_ACCURACY);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool LT(DateTime d1, DateTime d2, double acc)
        {
            return LT(d1, d2, TimeSpan.FromSeconds(acc));
        }

        public static bool LT(DateTime? d1, DateTime? d2, double acc)
        {
            return LT(d1, d2, TimeSpan.FromSeconds(acc));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool LT(DateTime d1, DateTime d2, TimeSpan acc)
        {
            return (IsMin(d1, acc) && !IsMin(d2, acc)) ||
                   (IsMax(d2, acc) && !IsMax(d1, acc)) ||
                   (!IsUndefined(d1, acc) && !IsUndefined(d2, acc) && (d1 + acc).CompareTo(d2) < 0);
        }

        public static bool LT(DateTime? d1, DateTime? d2, TimeSpan acc)
        {
            return (IsMin(d1, acc) && !IsMin(d2, acc)) ||
                   (IsMax(d2, acc) && !IsMax(d1, acc)) ||
                   (!IsUndefined(d1, acc) && !IsUndefined(d2, acc) && ((DateTime)d1 + acc).CompareTo(d2) < 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public static bool LE(DateTime d1, DateTime d2)
        {
            return LE(d1, d2, DATETIME_ACCURACY);
        }

        public static bool LE(DateTime? d1, DateTime? d2)
        {
            return LE(d1, d2, DATETIME_ACCURACY);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool LE(DateTime d1, DateTime d2, double acc)
        {
            return LE(d1, d2, TimeSpan.FromSeconds(acc));
        }

        public static bool LE(DateTime? d1, DateTime? d2, double acc)
        {
            return LE(d1, d2, TimeSpan.FromSeconds(acc));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool LE(DateTime d1, DateTime d2, TimeSpan acc)
        {
            return IsMin(d1, acc) ||
                   IsMax(d2, acc) ||
                   (d1 - acc).CompareTo(d2) <= 0;
        }

        public static bool LE(DateTime? d1, DateTime? d2, TimeSpan acc)
        {
            if (d1 != null && d2 != null)
            {
                return IsMin(d1, acc) ||
                       IsMax(d2, acc) ||
                       ((DateTime)d1 - acc).CompareTo(d2) <= 0;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static DateTime Max(DateTime t1, DateTime t2)
        {
            if (LT(t1, t2))
            {
                return t2;
            }
            else
            {
                return t1;
            }
        }

        public static DateTime? Max(DateTime? t1, DateTime? t2)
        {
            if (t1 != null && t2 != null)
            {
                if (LT(t1, t2))
                {
                    return t2;
                }
                else
                {
                    return t1;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static double Max(double val1, double val2)
        {
            if (IsDefined(val1) && IsDefined(val2))
            {
                return System.Math.Max(val1, val2);
            }
            else if (IsDefined(val1))
            {
                return val1;
            }
            else
            {
                return val2;
            }
        }

        public static double? Max(double? val1, double? val2)
        {
            if (val1 != null && val2 != null)
            {
                if (IsDefined(val1) && IsDefined(val2))
                {
                    return System.Math.Max((double)val1, (double)val2);
                }
                else if (IsDefined(val1))
                {
                    return val1;
                }
                else
                {
                    return val2;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public static double Min(double val1, double val2)
        {
            if (IsDefined(val1) && IsDefined(val2))
            {
                return System.Math.Min(val1, val2);
            }
            else if (IsDefined(val1))
            {
                return val1;
            }
            else
            {
                return val2;
            }
        }

        public static double? Min(double? val1, double? val2)
        {
            if (val1 != null && val2 != null)
            {
                if (IsDefined(val1) && IsDefined(val2))
                {
                    return System.Math.Min((double)val1, (double)val2);
                }
                else if (IsDefined(val1))
                {
                    return val1;
                }
                else
                {
                    return val2;
                }
            }else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static DateTime Min(DateTime t1, DateTime t2)
        {
            if (LT(t1, t2))
            {
                return t1;
            }
            else
            {
                return t2;
            }
        }

        public static DateTime? Min(DateTime? t1, DateTime? t2)
        {
            if (t1 == null || t2 == null)
            {
                return null;
            }
            else
            {
                if (LT(t1, t2))
                {
                    return t1;
                }
                else
                {
                    return t2;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsUndefined(DateTime d)
        {
            return IsUndefined(d, DATETIME_ACCURACY);
        }

        public static bool IsUndefined(DateTime? d)
        {
            return IsUndefined(d, DATETIME_ACCURACY);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool IsUndefined(DateTime d, double acc)
        {
            return IsUndefined(d, TimeSpan.FromSeconds(acc));
        }

        public static bool IsUndefined(DateTime? d, double acc)
        {
            return IsUndefined(d, TimeSpan.FromSeconds(acc));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool IsUndefined(DateTime d, TimeSpan acc)
        {
            return IsMin(d, acc) || IsMax(d, acc);
        }

        public static bool IsUndefined(DateTime? d, TimeSpan acc)
        {
            return IsMin(d, acc) || IsMax(d, acc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsMax(DateTime d)
        {
            return IsMax(d, DATETIME_ACCURACY);
        }

        public static bool IsMax(DateTime? d)
        {
            return IsMax(d, DATETIME_ACCURACY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool IsMax(DateTime d, double acc)
        {
            return IsMax(d, TimeSpan.FromSeconds(acc));
        }

        public static bool IsMax(DateTime? d, double acc)
        {
            return IsMax(d, TimeSpan.FromSeconds(acc));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool IsMax(DateTime d, TimeSpan acc)
        {
            return (DateTime.MaxValue - d).CompareTo(acc) <= 0;
        }

        public static bool IsMax(DateTime? d, TimeSpan acc)
        {
            if (d != null)
            {
                return (DateTime.MaxValue - (DateTime)d).CompareTo(acc) <= 0;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsMin(DateTime d)
        {
            return IsMin(d, DATETIME_ACCURACY);
        }

        public static bool IsMin(DateTime? d)
        {
            return IsMin(d, DATETIME_ACCURACY);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool IsMin(DateTime d, double acc)
        {
            return IsMin(d, TimeSpan.FromSeconds(acc));
        }

        public static bool IsMin(DateTime? d, double acc)
        {
            return IsMin(d, TimeSpan.FromSeconds(acc));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool IsMin(DateTime d, TimeSpan acc)
        {
            return (d - DateTime.MinValue).CompareTo(acc) <= 0;
        }

        public static bool IsMin(DateTime? d, TimeSpan acc)
        {
            if (d != null)
            {
                return ((DateTime)d - DateTime.MinValue).CompareTo(acc) <= 0;
            }
            else
            {
                return false;
            }
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

        public static bool IsUndefined(double? d)
        {
            return d == null || System.Double.IsNaN((double)d);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsInfinity(double d)
        {
            return double.IsInfinity(d);
        }

        public static bool IsInfinity(double? d)
        {
            return d == null || double.IsInfinity((double)d);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool IsInfinity(float f)
        {
            return float.IsInfinity(f);
        }

        public static bool IsInfinity(float? f)
        {
            return f == null || float.IsInfinity((float)f);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool IsUndefined(float f)
        {
            return System.Single.IsNaN(f);
        }

        public static bool IsUndefined(float? f)
        {
            return f == null || System.Single.IsNaN((float)f);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsUndefined(string s)
        {
            return (s == null || s == "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsUndefinedOrInfinity(double d)
        {
            return double.IsNaN(d) || double.IsInfinity(d);
        }

        public static bool IsUndefinedOrInfinity(double? d)
        {
            return d == null || double.IsNaN((double)d) || double.IsInfinity((double)d);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool IsUndefinedOrInfinity(float f)
        {
            return float.IsNaN(f) || float.IsInfinity(f);
        }

        public static bool IsUndefinedOrInfinity(float? f)
        {
            return f == null || float.IsNaN((float)f) || float.IsInfinity((float)f);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsDefined(double d)
        {
            return !IsUndefined(d);
        }

        public static bool IsDefined(double? d)
        {
            return !IsUndefined(d);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool IsDefined(float f)
        {
            return !IsUndefined(f);
        }

        public static bool IsDefined(float? f)
        {
            return !IsUndefined(f);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double Norm1(IList<double> l)
        {
            double value = 0;
            foreach (double x in l)
            {
                value += System.Math.Abs(x);
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double Norm2(IList<double> l)
        {
            double value = 0;
            foreach (double x in l)
            {
                value += x * x;
            }
            return SqrtEqual(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static double NormInfinity(IList<double> l)
        {
            double value = 0;
            foreach (double x in l)
            {
                if (System.Math.Abs(x) > value)
                {
                    value = System.Math.Abs(x);
                }
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(DateTime a, DateTime min, DateTime max)
        {
            if (min > max)
            {
                return Numeric.GE(a, max) && Numeric.LE(a, min);
            }
            else
            {
                return Numeric.GE(a, min) && Numeric.LE(a, max);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(short a, short min, short max)
        {
            if (min > max)
            {
                return (a >= max && a <= min);
            }
            else
            {
                return (a >= min && a <= max);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(int a, int min, int max)
        {
            if (min > max)
            {
                return (a >= max && a <= min);
            }
            else
            {
                return (a >= min && a <= max);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(long a, long min, long max)
        {
            if (min > max)
            {
                return (a >= max && a <= min);
            }
            else
            {
                return (a >= min && a <= max);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(float a, float min, float max)
        {
            if (min > max)
            {
                return (a >= max && a <= min);
            }
            else
            {
                return (a >= min && a <= max);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(double a, double min, double max)
        {
            if (min > max)
            {
                return (a >= max && a <= min);
            }
            else
            {
                return (a >= min && a <= max);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Interpolate(float x1, float x2, float y1, float y2, float x)
        {
            if (Numeric.EQ(x1, x2))
            {
                if (Numeric.EQ(y1, y2))
                {
                    return y1;
                }
                else
                {
                    return Numeric.UNDEF_FLOAT;
                }
            }
            else
            {
                return (y2 - ((x2 - x) * ((y2 - y1) / (x2 - x1))));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Interpolate(double x1, double x2, double y1, double y2, double x)
        {
            if (Numeric.EQ(x2, x1))
            {
                if (Numeric.EQ(y1, y2))
                {
                    return y1;
                }
                else
                {
                    return Numeric.UNDEF_DOUBLE;
                }
            }
            else
            {
                return (y2 - ((x2 - x) * ((y2 - y1) / (x2 - x1))));
            }
        }
        /// <summary>
        /// Given four points (x1,y1), (x2,y1), (x2,y2), (x1, y2) surrounding the point (x,y), 
        /// and their corresponding values z1, z2, z3, z4, the method returns the value z interpolated at point (x,y).
        ///     |
        ///     |
        /// y2  |       z4                  z3
        ///     | 
        ///     | 
        /// y   |              z
        ///     | 
        ///     |
        /// y1  |       z1                  z2
        ///     |
        ///     |
        ///    ------------------------------------------
        ///             x1     x            x2
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="z1"></param>
        /// <param name="z2"></param>
        /// <param name="z3"></param>
        /// <param name="z4"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double BiLinearInterpolation(double x1, double x2, double y1, double y2, double z1, double z2, double z3, double z4, double x, double y)
        {
            if (!Numeric.EQ(x1, x2) && !Numeric.EQ(y1, y2))
            {
                double t = (x - x1) / (x2 - x1);
                double u = (y - y1) / (y2 - y1);
                return (1 - t) * (1 - u) * z1 + t * (1 - u) * z2 + t * u * z3 + (1 - t) * u * z4;
            }
            else return Numeric.UNDEF_DOUBLE;
        }

        /// <summary>
        /// Interpolates from a function R^3->R, where the interpolation points are in a parallelipiped (i.e. regular grid). 
        /// 
        /// The values val111 -> val212 correspond to: 
        /// 
        /// val111 = f(x1, y1, z1)
        /// val121 = f(x1, y2, z1)
        /// val221 = f(x2, y2, z1)
        /// val211 = f(x2, y1, z1)
        /// val112 = f(x1, y1, z2)
        /// val122 = f(x1, y2, z2)
        /// val222 = f(x2, y2, z2)
        /// val212 = f(x2, y1, z2)
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="z1"></param>
        /// <param name="z2"></param>
        /// <param name="val111">f(x1, y1, z1)</param>
        /// <param name="val121">f(x1, y2, z1)</param>
        /// <param name="val221">f(x2, y2, z1)</param>
        /// <param name="val211">f(x2, y1, z1)</param>
        /// <param name="val112">f(x1, y1, z2)</param>
        /// <param name="val122">f(x1, y2, z2)</param>
        /// <param name="val222">f(x2, y2, z2)</param>
        /// <param name="val212">f(x2, y1, z2)</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double TriLinearInterpolation(double x1, double x2, double y1, double y2, double z1, double z2, double val111, double val121, double val221, double val211,
            double val112, double val122, double val222, double val212, double x, double y, double z)
        {
            double result = Numeric.UNDEF_DOUBLE;

            double val0 = Numeric.BiLinearInterpolation(x1, x2, y1, y2, val111, val121, val221, val211, x, y);
            double val1 = Numeric.BiLinearInterpolation(x1, x2, y1, y2, val112, val122, val222, val212, x, y);

            result = Numeric.Interpolate(z1, z2, val0, val1, z);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Log10Interpolate(double x1, double x2, double y1, double y2, double x)
        {
            if (Numeric.EQ(x2, x1))
            {
                return 0;
            }
            else
            {
                return y1 + (y2 - y1) * (System.Math.Pow(10.0, (x - x1) / (x2 - x1)) / 9.0 - 1.0 / 9.0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Exp10Interpolate(double x1, double x2, double y1, double y2, double x)
        {
            if (Numeric.EQ(x2, x1))
            {
                return 0;
            }
            else
            {
                return y1 + (y2 - y1) * System.Math.Log10(1 + 9 * (x - x1) / (x2 - x1));
            }
        }

        /// <summary>
        /// Linear extrapolation/interpolation given two points of a straight line
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Extrapolate(double x1, double x2, double y1, double y2, double x)
        {
            return y1 + ((x - x1) / (x2 - x1)) * (y2 - y1);
        }

        /// <summary>
        /// 
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

        public static double? Pow(double? a, int n)
        {
            if (a != null)
            {
                double da = (double)a;
                double value = 1.0;
                int n1 = (n < 0) ? -n : n;
                for (int i = 0; i < n1; i++)
                {
                    value *= da;
                }
                if (n < 0)
                {
                    value = 1 / value;
                }
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double SqrtEqual(double a)
        {
            if (a < 0 && Numeric.EQ(a, 0))
            {
                return 0;
            }
            else
            {
                return System.Math.Sqrt(a);
            }
        }

        public static double? SqrtEqual(double? a)
        {
            if (a != null)
            {
                if (a < 0 && Numeric.EQ(a, 0))
                {
                    return 0;
                }
                else
                {
                    return System.Math.Sqrt((double)a);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double AcosEqual(double a)
        {
            if (Numeric.EQ(a, 1))
            {
                return 0;
            }
            else if (Numeric.EQ(a, -1))
            {
                return Numeric.PI;
            }
            else
            {
                return System.Math.Acos(a);
            }
        }

        public static double? AcosEqual(double? a)
        {
            if (a != null)
            {
                if (Numeric.EQ(a, 1))
                {
                    return 0;
                }
                else if (Numeric.EQ(a, -1))
                {
                    return Numeric.PI;
                }
                else
                {
                    return System.Math.Acos((double)a);
                }
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double AsinEqual(double a)
        {
            if (Numeric.EQ(a, 1))
            {
                return Numeric.PI / 2.0;
            }
            else if (Numeric.EQ(a, -1))
            {
                return 3.0 * Numeric.PI / 2.0;
            }
            else
            {
                return System.Math.Asin(a);
            }
        }

        public static double? AsinEqual(double? a)
        {
            if (a != null)
            {
                if (Numeric.EQ(a, 1))
                {
                    return Numeric.PI / 2.0;
                }
                else if (Numeric.EQ(a, -1))
                {
                    return 3.0 * Numeric.PI / 2.0;
                }
                else
                {
                    return System.Math.Asin((double)a);
                }
            } else
            {
                return null;
            }
        }

        public static double AngleNormalized(double a)
        {
            a %= 2.0 * Numeric.PI;
            if (a < 0)
            {
                return a + 2.0 * Numeric.PI;
            }
            else
            {
                return a;
            }
        }

        public static double? AngleNormalized(double? a)
        {
            if (a != null)
            {
                a %= 2.0 * Numeric.PI;
                if (a < 0)
                {
                    return a + 2.0 * Numeric.PI;
                }
                else
                {
                    return a;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// convert degrees to radians
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double ToRadians(double a)
        {
            return a * PI / 180.0;
        }

        /// <summary>
        /// Heaviside function
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static double Heaviside(double d)
        {
            if (d >= 0)
            {
                return d;
            }
            else
            {
                return 0.0;
            }
        }
        /// <summary>
        /// Computes the gamma function, defined by 
        /// 
        /// \Gamma(z)  := \int_0^\infty t^{z-1} \exp^{-t} dt
        /// 
        /// the following routine only computes gamma for real positives values
        /// It is based on Lanczos approximation, as desrcibed in Numerical Recipes in C, p213, on-line edition
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double LogGamma(double z)
        {
            if (Numeric.GT(z, 0, 1e-8))
            {
                double x, y, temp, ser;
                double[] cof = {76.18009172947146,
                            -86.50532032941677,
                            24.01409824083091,
                            -1.23173957245055,
                            0.1208650973866179e-2,
                            -0.5395239384953e-5};
                y = x = z;
                temp = x + 5.5;
                temp -= (x + 0.5) * System.Math.Log(temp);
                ser = 1.000000000190015;
                for (int i = 0; i < 6; i++)
                {
                    ser += cof[i] / ++y;
                }
                return -temp + System.Math.Log(2.5066282746310005 * ser / x);
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }

        /// <summary>
        /// Computes the incomplete gamma function, by its series representations
        /// 
        /// The continued fraction approach is supposed to converge faster for x>a+1, 
        /// to do...
        /// Numerical Recipes in C, p217 o fthe on-line edition
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double IncompleteGamma(double a, double x)
        {
            double gln = LogGamma(a);
            return IncompleteGamma(a, x, gln);
        }

        /// <summary>
        /// Computes the incomplete gamma function, by its series representations
        /// 
        /// The continued fraction approach is supposed to converge faster for x>a+1, 
        /// to do...
        /// Numerical Recipes in C, p217 o fthe on-line edition
        /// </summary>
        /// <param name="a"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double IncompleteGamma(double a, double x, double logGammaa)
        {
            if (Numeric.EQ(x, 0))
            {
                return 0;
            }
            int maxIterations = 100;
            double epsilon = 3.0e-7;
            double gln = logGammaa;
            double sum, del, ap;
            double result = Numeric.UNDEF_DOUBLE;
            if (Numeric.GE(x, 0, 1e-8) && !Numeric.EQ(a, 0, 1e-8))
            {
                ap = a;
                del = sum = 1.0 / a;
                int i = 0;
                while (i < maxIterations)
                {
                    ++ap;
                    del *= x / ap;
                    sum += del;
                    if (Numeric.LE(System.Math.Abs(del), System.Math.Abs(sum) * epsilon))
                    {
                        result = sum * System.Math.Exp(-x + a * System.Math.Log(x) - gln);
                        return result;
                    }
                    i++;
                }
                return result;
            }
            else
            {
                return Numeric.UNDEF_DOUBLE;
            }
        }

        /// <summary>
        /// Complementary error function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double ErFC(double x)
        {
            double t, z, ans;
            z = System.Math.Abs(x);
            t = 1 / (1 + 0.5 * z);
            ans = t * System.Math.Exp(-z * z - 1.26551223 + t * (1.00002368 + t * (0.37409196 + t * (0.09678418 +
                t * (-0.18628806 + t * (0.27886807 + t * (-1.13520398 + t * (1.48851587 + t * (-0.82215223 + t * 0.17087277)))))))));
            return Numeric.GE(x, 0) ? ans : 2 - ans;
        }

        public static double ErrorFunction(double x)
        {
            if (Numeric.GT(System.Math.Abs(x), 7.696))
            {
                return 1.0 - ErFC(x);
            }
            return x < 0 ? -IncompleteGamma(0.5, x * x) : IncompleteGamma(0.5, x * x);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Swap(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byte tmp = bytes[0];
            bytes[0] = bytes[3];
            bytes[3] = tmp;
            tmp = bytes[1];
            bytes[1] = bytes[2];
            bytes[2] = tmp;
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short Swap(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byte tmp = bytes[0];
            bytes[0] = bytes[1];
            bytes[1] = tmp;
            return BitConverter.ToInt16(bytes, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static void Swap(ref double a, ref double b)
        {
            double temp = b;
            b = a;
            a = temp;
        }

        /// <summary>
        /// Solve the system of 2 equations of 2 variables (x and y)
        /// a1*x+b1*y+c1*x*y+d1=0
        /// a2*x+b2*y+c2*x*y+d2=0
        /// 
        /// There are two solutions (x1, y1) and (x2, y2)
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="b1"></param>
        /// <param name="c1"></param>
        /// <param name="d1"></param>
        /// <param name="a2"></param>
        /// <param name="b2"></param>
        /// <param name="c2"></param>
        /// <param name="d2"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static void SolveSystemOf2Equations(double a1, double b1, double c1, double d1, double a2, double b2, double c2, double d2, out double x1, out double y1, out double x2, out double y2)
        {
            x1 = UNDEF_DOUBLE;
            y1 = UNDEF_DOUBLE;
            x2 = UNDEF_DOUBLE;
            y2 = UNDEF_DOUBLE;
            double A = c1 * c1 * d2 * d2 - 2 * c1 * c2 * d1 * d2 + 4 * a1 * b1 * c2 * d2 - 2 * a1 * b2 * c1 * d2 - 2 * a2 * b1 * c1 * d2 + c2 * c2 * d1 * d1 - 2 * a1 * b2 * c2 * d1 - 2 * a2 * b1 * c2 * d1 + 4 * a2 * b2 * c1 * d1 + a1 * a1 * b2 * b2 - 2 * a1 * a2 * b1 * b2 + a2 * a2 * b1 * b1;
            if (GE(A, 0))
            {
                double sqrtA = SqrtEqual(A);
                double denom1 = c2 * (sqrtA + a1 * b2 + a2 * b1) + c1 * c2 * d2 - c2 * c2 * d1 - 2 * a2 * b2 * c1;
                double denom2 = 2 * b1 * c2 - 2 * b2 * c1;
                double denom3 = c2 * (-sqrtA + a1 * b2 + a2 * b1) + c1 * c2 * d2 - c2 * c2 * d1 - 2 * a2 * b2 * c1;
                if (!EQ(denom1, 0) && !EQ(denom2, 0) && !EQ(denom3, 0))
                {
                    x1 = -(b2 * (sqrtA - a2 * b1) + (2 * b1 * c2 - b2 * c1) * d2 - b2 * c2 * d1 + a1 * b2 * b2) / denom1;
                    y1 = (sqrtA + c1 * d2 - c2 * d1 + a1 * b2 - a2 * b1) / denom2;
                    x2 = -(b2 * (-sqrtA - a2 * b1) + (2 * b1 * c2 - b2 * c1) * d2 - b2 * c2 * d1 + a1 * b2 * b2) / denom3;
                    y2 = -(sqrtA - c1 * d2 + c2 * d1 - a1 * b2 + a2 * b1) / denom2;
                }
            }
        }

        /// <summary>
        /// Attempts to parse a string using different number formats until successful or no more number formats. Formats are attempted in this order:
        /// US, NO, FR, IT, SP, PT, UK.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">Output value</param>
        /// <returns>True if successful, else false</returns>
        public static bool TryParse(string value, out double result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = Numeric.UNDEF_DOUBLE;
                return true;
            }
            else
            {
                return double.TryParse(value, out result) ||
                       double.TryParse(value, NumberStyles.Float, US_NUMBER_FORMAT, out result) ||
                       double.TryParse(value, NumberStyles.Float, NO_NUMBER_FORMAT, out result) ||
                       double.TryParse(value, NumberStyles.Float, FR_NUMBER_FORMAT, out result) ||
                       double.TryParse(value, NumberStyles.Float, IT_NUMBER_FORMAT, out result) ||
                       double.TryParse(value, NumberStyles.Float, SP_NUMBER_FORMAT, out result) ||
                       double.TryParse(value, NumberStyles.Float, PT_NUMBER_FORMAT, out result) ||
                       double.TryParse(value, NumberStyles.Float, UK_NUMBER_FORMAT, out result);
            }
        }

        /// <summary>
        /// Attempts to parse a string using different number formats until successful or no more number formats. Formats are attempted in this order:
        /// US, NO, FR, IT, SP, PT, UK.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">Output value</param>
        /// <returns>True if successful, else false</returns>
        public static bool TryParse(string value, out float result)
        {
            if (string.IsNullOrEmpty(value))
            {
                result = Numeric.UNDEF_FLOAT;
                return true;
            }
            else
            {
                return float.TryParse(value, out result) ||
                   float.TryParse(value, NumberStyles.Float, US_NUMBER_FORMAT, out result) ||
                   float.TryParse(value, NumberStyles.Float, NO_NUMBER_FORMAT, out result) ||
                   float.TryParse(value, NumberStyles.Float, FR_NUMBER_FORMAT, out result) ||
                   float.TryParse(value, NumberStyles.Float, IT_NUMBER_FORMAT, out result) ||
                   float.TryParse(value, NumberStyles.Float, SP_NUMBER_FORMAT, out result) ||
                   float.TryParse(value, NumberStyles.Float, UK_NUMBER_FORMAT, out result) ||
                   float.TryParse(value, NumberStyles.Float, PT_NUMBER_FORMAT, out result);
            }
        }

        /// <summary>
        /// Attempts to parse a string using different number formats until successful or no more number formats. Formats are attempted in this order:
        /// US, NO, FR, IT, SP, PT, UK.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">Output value</param>
        /// <returns>True if successful, else false</returns>
        public static bool TryParse(string value, out long result)
        {
            return long.TryParse(value, out result) ||
                   long.TryParse(value, NumberStyles.Float, US_NUMBER_FORMAT, out result) ||
                   long.TryParse(value, NumberStyles.Float, NO_NUMBER_FORMAT, out result) ||
                   long.TryParse(value, NumberStyles.Float, FR_NUMBER_FORMAT, out result) ||
                   long.TryParse(value, NumberStyles.Float, IT_NUMBER_FORMAT, out result) ||
                   long.TryParse(value, NumberStyles.Float, SP_NUMBER_FORMAT, out result) ||
                   long.TryParse(value, NumberStyles.Float, UK_NUMBER_FORMAT, out result) ||
                   long.TryParse(value, NumberStyles.Float, PT_NUMBER_FORMAT, out result);
        }

        /// <summary>
        /// Attempts to parse a string using different number formats until successful or no more number formats. Formats are attempted in this order:
        /// US, NO, FR, IT, SP, PT, UK.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">Output value</param>
        /// <returns>True if successful, else false</returns>
        public static bool TryParse(string value, out int result)
        {
            return int.TryParse(value, out result) ||
                   int.TryParse(value, NumberStyles.Float, US_NUMBER_FORMAT, out result) ||
                   int.TryParse(value, NumberStyles.Float, NO_NUMBER_FORMAT, out result) ||
                   int.TryParse(value, NumberStyles.Float, FR_NUMBER_FORMAT, out result) ||
                   int.TryParse(value, NumberStyles.Float, IT_NUMBER_FORMAT, out result) ||
                   int.TryParse(value, NumberStyles.Float, SP_NUMBER_FORMAT, out result) ||
                   int.TryParse(value, NumberStyles.Float, UK_NUMBER_FORMAT, out result) ||
                   int.TryParse(value, NumberStyles.Float, PT_NUMBER_FORMAT, out result);
        }

        /// <summary>
        /// Attempts to parse a string using different number formats until successful or no more number formats. Formats are attempted in this order:
        /// US, NO, FR, IT, SP, PT, UK.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">Output value</param>
        /// <returns>True if successful, else false</returns>
        public static bool TryParse(string value, out short result)
        {
            return short.TryParse(value, out result) ||
                   short.TryParse(value, NumberStyles.Float, US_NUMBER_FORMAT, out result) ||
                   short.TryParse(value, NumberStyles.Float, NO_NUMBER_FORMAT, out result) ||
                   short.TryParse(value, NumberStyles.Float, FR_NUMBER_FORMAT, out result) ||
                   short.TryParse(value, NumberStyles.Float, IT_NUMBER_FORMAT, out result) ||
                   short.TryParse(value, NumberStyles.Float, SP_NUMBER_FORMAT, out result) ||
                   short.TryParse(value, NumberStyles.Float, UK_NUMBER_FORMAT, out result) ||
                   short.TryParse(value, NumberStyles.Float, PT_NUMBER_FORMAT, out result);
        }

        /// <summary>
        /// Attempts to parse a string using different number formats until successful or no more number formats. Formats are attempted in this order:
        /// US, NO, FR, IT, SP, PT, UK.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">Output value</param>
        /// <returns>True if successful, else false</returns>
        public static bool TryParse(string value, out byte result)
        {
            return byte.TryParse(value, out result) ||
                   byte.TryParse(value, NumberStyles.Float, US_NUMBER_FORMAT, out result) ||
                   byte.TryParse(value, NumberStyles.Float, NO_NUMBER_FORMAT, out result) ||
                   byte.TryParse(value, NumberStyles.Float, FR_NUMBER_FORMAT, out result) ||
                   byte.TryParse(value, NumberStyles.Float, IT_NUMBER_FORMAT, out result) ||
                   byte.TryParse(value, NumberStyles.Float, SP_NUMBER_FORMAT, out result) ||
                   byte.TryParse(value, NumberStyles.Float, UK_NUMBER_FORMAT, out result) ||
                   byte.TryParse(value, NumberStyles.Float, PT_NUMBER_FORMAT, out result);
        }

        /// <summary>
        /// Returns true if value matches a case-insensitive 'true', else false.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">Output value</param>
        /// <returns>Always true</returns>
        public static bool TryParse(string value, out bool result)
        {
            result = !string.IsNullOrEmpty(value) && value.ToUpper().Equals("TRUE");
            return true;
        }

        /// <summary>
        /// Attempts to parse a string using different DateTime formats until successful or no more number formats. Formats are attempted in this order:
        /// US, NO, FR, IT, SP, PT, UK.
        /// </summary>
        /// <param name="value">Input string</param>
        /// <param name="result">Output value</param>
        /// <returns>True if successful, else false</returns>
        public static bool TryParse(string value, out DateTime result)
        {
            return DateTime.TryParse(value, out result) ||
                   DateTime.TryParse(value, US_DATETIME_FORMAT, DateTimeStyles.AssumeLocal, out result) ||
                   DateTime.TryParse(value, NO_DATETIME_FORMAT, DateTimeStyles.AssumeLocal, out result) ||
                   DateTime.TryParse(value, FR_DATETIME_FORMAT, DateTimeStyles.AssumeLocal, out result) ||
                   DateTime.TryParse(value, IT_DATETIME_FORMAT, DateTimeStyles.AssumeLocal, out result) ||
                   DateTime.TryParse(value, SP_DATETIME_FORMAT, DateTimeStyles.AssumeLocal, out result) ||
                   DateTime.TryParse(value, UK_DATETIME_FORMAT, DateTimeStyles.AssumeLocal, out result) ||
                   DateTime.TryParse(value, PT_DATETIME_FORMAT, DateTimeStyles.AssumeLocal, out result);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="ok"></param>
        public static void TryParseBoolean(string value, ref bool ok)
        {
            if (string.IsNullOrEmpty(value))
            {
                // Do nothing
            }
            else if (value.ToLower().Equals("yes") || value.ToLower().Equals("true"))
            {
                ok = true;
            }
            else
            {
                ok = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="val"></param>
        public static void TryParseDouble(string value, ref double val)
        {
            if (string.IsNullOrEmpty(value))
            {
                // Do nothing
            }
            else
            {
                Numeric.TryParse(value, out val);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="val"></param>
        public static void TryParseFloat(string value, ref float val)
        {
            if (string.IsNullOrEmpty(value))
            {
                // Do nothing
            }
            else
            {
                Numeric.TryParse(value, out val);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="val"></param>
        public static void TryParseInt(string value, ref int val)
        {
            if (string.IsNullOrEmpty(value))
            {
                // Do nothing
            }
            else
            {
                Numeric.TryParse(value, out val);
            }
        }

        /// <summary>
        /// Computes the decimal day of year from month, day, year. Accounts for leap years
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static double ToDecimalDate(int year, int month, int day)
        {
            return ToDecimalDate(new DateTime(year, month, day));
        }

        /// <summary>
        /// Computes the decimal day of year from month, day, year. Accounts for leap years
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static double ToDecimalDate(int year, int month, int day, int hours, int minutes, int seconds)
        {
            return ToDecimalDate(new DateTime(year, month, day, hours, minutes, seconds));
        }

        /// <summary>
        /// Computes the decimal day of year from month, day, year. Accounts for leap years
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static double ToDecimalDate(int year, int month, int day, int hours, int minutes, int seconds, int milliseconds)
        {
            return ToDecimalDate(new DateTime(year, month, day, hours, minutes, seconds, milliseconds));
        }

        /// <summary>
        /// Computes the decimal day of year from month, day, year. Accounts for leap years
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static double ToDecimalDate(DateTime date)
        {
            bool isLeapYear = DateTime.DaysInMonth(date.Year, 2) == 29;
            double dayOfYear = (double)date.DayOfYear;

            double timeOfDay = ((double)date.Hour * 3600.0 + (double)date.Minute * 60.0 + (double)date.Second + (double)date.Millisecond * 0.001) / 86400.0;
            dayOfYear += timeOfDay;
            if (isLeapYear)
            {
                return (double)date.Year + dayOfYear / 366.0;
            }
            else
            {
                return (double)date.Year + dayOfYear / 365.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime FromDecimalDate(double date)
        {
            if (IsDefined(date))
            {
                int year = (int)date;
                int daysInYear = 365;
                bool isLeapYear = DateTime.DaysInMonth(year, 2) == 29;
                if (isLeapYear) { daysInYear++; }
                Int64 ticksSinceFirstJanuary = (Int64)(((date - year) * daysInYear - 1) * 24 * 60 * 60 * 1e7);
                return new DateTime(year, 1, 1).AddTicks(ticksSinceFirstJanuary);
            }
            else
            {
                return DateTime.MaxValue;
            }
        }

        public static int SolveRealQuadraticEquation(double a, double b, double c, out double x1, out double x2)
        {
            if (Numeric.EQ(a, 0))
            {
                // degenerated case b.x+c=0
                x1 = -c / b;
                x2 = x1;
                return 1;
            }
            else
            {
                double delta = b * b - 4.0 * a * c;
                if (Numeric.EQ(delta, 0))
                {
                    x1 = -b / (2.0 * a);
                    x2 = x1;
                    return 1;
                }
                else if (Numeric.GT(delta, 0))
                {
                    x1 = (-b + System.Math.Sqrt(delta)) / (2.0 * a);
                    x2 = (-b - System.Math.Sqrt(delta)) / (2.0 * a);
                    return 2;
                }
                else
                {
                    x1 = Numeric.UNDEF_DOUBLE;
                    x2 = x1;
                    return 0;
                }
            }
        }

        public static double Erf(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
                sign = -1;
            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return sign * y;
        }

        public static double GetAcceleration(double dt, double u2, double u1, double u0)
        {
            return (u2 - 2 * u1 + u0) / (dt * dt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deg"></param>
        /// <param name="min"></param>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static double FromSexagecimal(int deg, int min, double sec)
        {
            double dec = deg;
            dec += min / 60.0;
            dec += sec / 3600.0;
            return dec;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dec"></param>
        /// <param name="deg"></param>
        /// <param name="min"></param>
        /// <param name="sec"></param>
        public static void ToSexagecimal(double dec, out int deg, out int min, out double sec)
        {
            deg = (int)(dec);
            double rem = dec - deg;
            min = (int)(rem * 60);
            sec = 60.0 * (rem * 60 - min);
        }

        /// <summary>
        /// This static method returns a string where the decimal part is displayed
        /// as a fraction. The default accurary is 1/32. For instance 6.5 -> 6 1/2
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToFractionalValue(double value)
        {
            return ToFractionalValue(value, 1.0 / 32.0);
        }

        /// <summary>
        /// This static method returns a string where the decimal part is displayed
        /// as a fraction (accounting for the given accuracy. For instance 6.5 -> 6 1/2
        /// </summary>
        /// <param name="value"></param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static string ToFractionalValue(double value, double accuracy)
        {
            bool negative = false;
            if (Double.IsInfinity(value))
            {
                return "Infinity";
            }
            if (value < 0)
            {
                value = -1 * value;
                negative = true;
            }
            int intVal = (int)value;
            double dec = value - (double)intVal;
            double n = dec / accuracy;
            double N = System.Math.Round(n, 0);
            if (N == 0 && intVal > 0)
            {
                if (negative)
                {
                    return ("-" + intVal.ToString());
                }
                else
                {
                    return intVal.ToString();
                }

            }
            else if (N == 0 && intVal == 0)
            {
                return "0";
            }
            else
            {
                int counter = 2;
                double D = 1 / accuracy;
                double Tn = N;
                double Td = D;
                while (counter <= N)
                {
                    if (N % counter == 0 && D % counter == 0)
                    {
                        Tn = N / counter;
                        Td = D / counter;
                    }
                    counter++;
                }
                if (intVal > 0)
                {
                    if (negative)
                    {
                        return ("-" + intVal + " " + Tn.ToString() + "/" + Td.ToString());
                    }
                    else
                    {
                        return (intVal + " " + Tn.ToString() + "/" + Td.ToString());
                    }
                }
                else
                {
                    if (negative)
                    {
                        return ("-" + Tn.ToString() + "/" + Td.ToString());
                    }
                    else
                    {
                        return (Tn.ToString() + "/" + Td.ToString());
                    }
                }
            }

            // throw Exception("Soon implemented");
        }

        /// <summary>
        /// Returns a decimal value based on a string where the decimal part can be
        /// expressed as a fractional value. For instance 6 1/2 is converted into 6.5
        /// But non fractional value are also accepted by this method. For instance
        /// 6,5 is read as 6.5
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double FromFractionalValue(string value)
        {
            double result = UNDEF_DOUBLE;
            if (string.IsNullOrEmpty(value))
            {
                return result;
            }
            else
            {
                if (value.Contains("/"))
                {
                    int index = value.IndexOf("/");
                    string str1 = value.Substring(0, index);
                    string str2 = value.Substring(index + 1);
                    double denom;
                    TryParse(str2, out denom);

                    if (denom <= 0)
                    {
                        return UNDEF_DOUBLE;
                    }
                    else
                    {
                        string[] words;
                        char[] separator = { ' ', '\t' };
                        words = str1.Split(separator);
                        int length = words.Length;
                        List<string> numStr = new List<string>();

                        for (int i = 0; i < length; i++)
                        {
                            if (words[i] != "")
                            {
                                numStr.Add(words[i]);
                            }
                        }
                        if (numStr.Count == 1)
                        {

                            TryParse(numStr[0], out result);
                            result = result / denom;
                            return result;
                        }
                        else if (numStr.Count == 2)
                        {
                            if (numStr[0] == "-")
                            {
                                TryParse(numStr[1], out result);
                                return (-1 * result / denom);
                            }
                            else
                            {
                                double n1, n2;
                                TryParse(numStr[0], out n1);
                                TryParse(numStr[1], out n2);

                                if (n1 > 0)
                                {
                                    result = n1 + (n2 / denom);
                                    return result;
                                }
                                else
                                {
                                    result = (-1 * n1) + (n2 / denom);
                                    return (-1 * result);
                                }

                            }
                        }
                        else if (numStr.Count == 3 && numStr[0] == "-")
                        {
                            double n1, n2;

                            TryParse(numStr[1], out n1);
                            TryParse(numStr[2], out n2);
                            if (n1 > 0)
                            {
                                result = n1 + n2 / denom;
                                return (-1 * result);
                            }

                            else
                            {
                                return UNDEF_DOUBLE;
                            }
                        }

                        else
                        {
                            return UNDEF_DOUBLE;
                        }
                    }
                }
                else
                {
                    TryParse(value, out result);
                    return result;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToTimeValue(double value)
        {
            TimeSpan time = TimeSpan.FromSeconds(value);
            return time.Hours.ToString("00") + ":" + time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double FromTimeValue(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length == 8)
            {
                int hours, minutes, seconds;
                if (TryParse(str.Substring(0, 2), out hours) &&
                    TryParse(str.Substring(3, 2), out minutes) &&
                    TryParse(str.Substring(6, 2), out seconds))
                {
                    return (double)(hours * 3600 + minutes * 60 + seconds);
                }
                else
                {
                    return UNDEF_DOUBLE;
                }
            }
            else
            {
                return UNDEF_DOUBLE;
            }
        }

        public static string GetScientificNumberFormat(int accuracy)
        {
            string s = "0";
            for (int i = 0; i < accuracy; i++)
            {
                if (i == 0) s += ".";
                s += "0";
            }
            s += "e+00";
            return s;
        }

        /// <summary>
        /// Convert a double to scientific notation, with the specified number of decimals
        /// </summary>
        /// <param name="value"></param>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static string ToScientificNotation(double value, int accuracy)
        {
            return value.ToString(GetScientificNumberFormat(accuracy), CultureInfo.CurrentCulture.NumberFormat);
        }

    }
}
