using System;
using System.Collections.Generic;

namespace NORCE.General.Math
{
    public interface IValuable
    {
        /// <summary>
        /// Interface for objects which can behave like a function f: R --> R
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        double Eval(double x);
    }


    /// <summary>
    /// Interface for objects which can behave like a function f: R -> R or f: Z --> Z
    /// </summary>
    public interface IValuable<T>
    {
        /// <summary>
        /// Interface for objects which can behave like a function f: R -> R or f: Z --> Z
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        T Eval(T x);
    }


    /// <summary>
    /// Interface for objects which can behave like a function f: R -> R or f: Z --> Z
    /// Evaluate at a set of parameter values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValuableList<T>
    {
        /// <summary>
        /// Interface for objects which can behave like a function f: R -> R or f: Z --> Z
        /// Evaluate at a set of parameter values
        /// </summary>
        /// <param name="list"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        void Eval(IList<T> list, ref IList<T> results);
    }
}
