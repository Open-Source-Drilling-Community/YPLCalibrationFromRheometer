using System;

namespace NORCE.General.Math
{
    public interface IPolynom : IValuable
    {
        /// <summary>
        /// 
        /// </summary>
        int Degree { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        double this[int index] { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        void Set(double[] a);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        void Set(IPolynom a);


        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        double FindRoot(double min, double max);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        double Derive(double x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        double DeriveSecond(double x);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        double Integrate(double a, double b);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        void Derivate(ref IPolynom d);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        void Primitive(ref IPolynom p);

    }
}
