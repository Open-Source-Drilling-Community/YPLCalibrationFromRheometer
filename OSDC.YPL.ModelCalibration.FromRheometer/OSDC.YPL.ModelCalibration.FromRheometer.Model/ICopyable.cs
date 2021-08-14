using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    /// <summary>
    /// an interface for things that can be copied into another object of the same class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    interface ICopyable<T>
    {
        /// <summary>
        /// copy from this to target
        /// </summary>
        /// <param name="target"></param>
        bool Copy(T target);
    }
}
