using System;
using System.Collections.Generic;
using System.Text;

namespace NORCE.General.Std
{
    /// <summary>
    /// Copy from current object to another object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICopyable<T>
    {
        /// <summary>
        /// Copy from 'this' to 'item'.
        /// </summary>
        /// <param name="item"></param>
        void Copy(ref T item);
    }
}
