using System;
using System.Collections.Generic;
using System.Text;

namespace NORCE.General.Std
{
    /// <summary>
    /// Strict implementation of IPair interface. Contains a pair of objects, not necessarily of the same type. See also Couple.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    [Serializable]
    public struct Pair<T1, T2> : IPair<T1, T2>, ICopyable<IPair<T1, T2>>, ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public Pair(T1 left, T2 right)
        {
            left_ = left;
            right_ = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair"></param>
        public Pair(Pair<T1, T2> pair)
        {
            left_ = pair.left_;
            right_ = pair.right_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair"></param>
        public Pair(IPair<T1, T2> pair)
        {
            left_ = pair.Left;
            right_ = pair.Right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<" + left_ + ", " + right_ + ">";
        }

        /// <summary>
        /// 
        /// </summary>
        public T1 Left
        {
            get { return left_; }
            set { left_ = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public T2 Right
        {
            get { return right_; }
            set { right_ = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void Set(T1 left, T2 right)
        {
            left_ = left;
            right_ = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair"></param>
        public void Set(IPair<T1, T2> pair)
        {
            left_ = pair.Left;
            right_ = pair.Right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair"></param>
        public void Copy(ref IPair<T1, T2> pair)
        {
            pair.Left = left_;
            pair.Right = right_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Pair<T1, T2>(this);
        }

        private T1 left_;
        private T2 right_;
    }

    /// <summary>
    /// A comparer for struct Pair.  Requires that IComparable is implemented for each type.  We don't want this restriction
    /// in the general implementation of the class, therefore the comparer is placed in a separate class.
    /// Example of usage: IDictionary(IPair(int, string), string) sorted =
    ///     new SortedList(IPair(int, string), string>(new PairComparer(int, string)());
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class PairComparer<T1, T2> : Comparer<IPair<T1, T2>>
        where T1 : IComparable
        where T2 : IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(IPair<T1, T2> x, IPair<T1, T2> y)
        {
            if (x.Left.CompareTo(y.Left) != 0)
            {
                return x.Left.CompareTo(y.Left);
            }
            else if (x.Right.CompareTo(y.Right) != 0)
            {
                return x.Right.CompareTo(y.Right);
            }
            else
            {
                return 0;
            }
        }
    }
}
