using System;
using NORCE.General.Std;

namespace NORCE.General.Math
{
    public class LinearPolynom : IPolynom
    {
        private double a_;
        private double b_;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public LinearPolynom(double a, double b)
        {
            a_ = a;
            b_ = b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        public LinearPolynom(double[] a)
        {
            if (a != null && a.Length >= 2)
            {
                a_ = a[1];
                b_ = a[0];
            }
            else
            {
                a_ = Numeric.UNDEF_DOUBLE;
                b_ = Numeric.UNDEF_DOUBLE;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public LinearPolynom(IPolynom p)
        {
            if (p != null && p.Degree >= 1)
            {
                a_ = p[1];
                b_ = p[0];
            }
            else
            {
                a_ = Numeric.UNDEF_DOUBLE;
                b_ = Numeric.UNDEF_DOUBLE;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public LinearPolynom(LinearPolynom p)
        {
            a_ = p.a_;
            b_ = p.b_;
        }

        /// <summary>
        /// 
        /// </summary>
        public double A
        {
            get { return a_; }
            set { a_ = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public double B
        {
            get { return b_; }
            set { b_ = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "y=" + a_ + "*x+" + b_;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void Set(LinearPolynom p)
        {
            a_ = p.a_;
            b_ = p.b_;
        }

        #region ILinearPolynom<T> Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void Set(double a, double b)
        {
            a_ = a;
            b_ = b;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        public void Set(double[] a)
        {
            if (a != null && a.Length >= 2)
            {
                a_ = a[1];
                b_ = a[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void Set(IPolynom p)
        {
            if (p != null && p.Degree >= 2)
            {
                a_ = p[1];
                b_ = p[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public int FindRoots(ref double root)
        {
            if (Numeric.EQ(a_, 0))
            {
                root = Numeric.UNDEF_DOUBLE;
                if (Numeric.EQ(b_, 0))
                {
                    //infinite number of solutions
                    return -1;
                }
                else
                {
                    //no solutions
                    return 0;
                }
            }
            else
            {
                root = -b_ / a_;
                return 1;
            }
        }

        #endregion

        #region IPolynom Members
        /// <summary>
        /// 
        /// </summary>
        public int Degree
        {
            get { return 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return b_;
                }
                else
                {
                    return a_;
                }
            }
            set
            {
                if (index == 0)
                {
                    b_ = value;
                }
                else
                {
                    a_ = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Eval(double x)
        {
            return a_ * x + b_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double FindRoot(double min, double max)
        {
            double root = Numeric.UNDEF_DOUBLE;
            int result = FindRoots(ref root);
            if (result == 0)
            {
                return Numeric.UNDEF_DOUBLE;
            }
            else if (result == -1)
            {
                return min;
            }
            else
            {
                if (Numeric.GE(root, min) && Numeric.LE(root, max))
                {
                    return root;
                }
                else
                {
                    return Numeric.UNDEF_DOUBLE;
                }
            }
        }

        #endregion

        #region IUndefinable Members
        /// <summary>
        /// 
        /// </summary>
        public void SetUndefined()
        {
            a_ = Numeric.UNDEF_DOUBLE;
            b_ = Numeric.UNDEF_DOUBLE;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsUndefined()
        {
            return Numeric.IsUndefined(a_) || Numeric.IsUndefined(b_);
        }

        #endregion

        #region ICloneable Members
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new LinearPolynom(this);
        }

        #endregion

        #region ICopyable<IPolynom> Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Copy(ref IPolynom item)
        {
            if (item != null && item.Degree == 1)
            {
                item[0] = b_;
                item[1] = a_;
            }
        }

        #endregion

        #region IEquatable<IPolynom> Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IPolynom other)
        {
            if (other != null && other.Degree == 1)
            {
                return Numeric.EQ(a_, other[1]) && Numeric.EQ(b_, other[0]);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ICopyable<LinearPolynom<T,Calculator>> Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Copy(ref LinearPolynom item)
        {
            item.a_ = a_;
            item.b_ = b_;
        }

        #endregion

        #region IEquatable<LinearPolynom<T,Calculator>> Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(LinearPolynom other)
        {
            return Numeric.EQ(a_, other.a_) && Numeric.EQ(b_, other.b_);
        }

        #endregion

        #region IZero Members
        /// <summary>
        /// 
        /// </summary>
        public void SetZero()
        {
            a_ = 0;
            b_ = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsZero()
        {
            return Numeric.EQ(a_, 0) && Numeric.EQ(b_, 0);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Derive(double x)
        {
            return a_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DeriveSecond(double x)
        {
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public double Integrate(double a, double b)
        {
            return 0.5 * a_ * b * b + b_ * b - (0.5 * a_ * a * a + b_ * a);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void Derivate(ref IPolynom p)
        {
            if (p != null && p.Degree == 0)
            {
                p[0] = a_;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void Primitive(ref IPolynom p)
        {
            if (p != null && p.Degree == 2)
            {
                p[0] = 0;
                p[1] = b_;
                p[2] = a_ / 2.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public void Derivate(ref double p)
        {
            p = a_;
        }

    }
}
