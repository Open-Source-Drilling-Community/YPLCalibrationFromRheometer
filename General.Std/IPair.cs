using System;
using System.Collections.Generic;
using System.Text;

namespace NORCE.General.Std
{
    /// <summary>
    /// A pair of objects, not necessarily of the same type. See also ICouple.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IPair<T1, T2>
    {
        /// <summary>
        /// 
        /// </summary>
        T1 Left
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        T2 Right
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lefth"></param>
        /// <param name="right"></param>
        void Set(T1 lefth, T2 right);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair"></param>
        void Set(IPair<T1, T2> pair);
    }
}
