using OSDC.DotnetLibraries.General.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YPLCalibrationFromRheometer.Model
{
    public class CouetteRheometer : INamable, IIdentifiable, ICloneable
    {
        /// <summary>
        /// the different types of Couette Rheometers: Rotor-Bob or Rotating Bob
        /// </summary>
        public enum RheometerTypeEnum { RotorBob, RotatingBob }
        /// <summary>
        /// a Human readable name for the rheometer
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// a description of the rheometer
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// a global unique identifier for the rheometer
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// The type of the rheometer
        /// </summary>
        public RheometerTypeEnum RheometerType { get; set; }
        /// <summary>
        /// the radius of the bob
        /// </summary>
        public double BobRadius { get; set; }
        /// <summary>
        /// the gap between the bob and the cup or rotor
        /// </summary>
        public double Gap { get; set; }
        /// <summary>
        /// the end effect correction of the torque conversion utilizing the Newtonian fluid hypothesis
        /// </summary>
        public double NewtonianEndEffectCorrection { get; set; }
        /// <summary>
        /// the length of the bob
        /// </summary>
        public double BobLength { get; set; }
        /// <summary>
        /// the angle compared to horizontal of the conical part of the bob
        /// </summary>
        public double ConicalAngle { get; set; }
        /// <summary>
        /// The standard deviation of of the degree to which repeated measurements under unchanged conditions show the same results.
        /// Typically 0.25Pa for a Fann35 rheometer, 0.02Pa for an Anton Paar rheometer
        /// </summary>
        public double MeasurementPrecision { get; set; }
        /// <summary>
        /// when true then the shear rate and stress are given in middle of the gap. Otherwise, they are at the surface of the bob.
        /// </summary>
        public bool UseISOConvention { get; set; }
        /// <summary>
        /// for mechanical rheometers that have a fixed list of speeds. The values are in rev per second.
        /// </summary>
        public List<double> FixedSpeedList { get; set; }

        /// <summary>
        /// default constructor
        /// </summary>
        public CouetteRheometer() : base()
        {

        }
        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public CouetteRheometer(CouetteRheometer src) : base()
        {
            if (src != null)
            {
                Name = src.Name;
                Description = src.Description;
                ID = src.ID;
                RheometerType= src.RheometerType;
                BobRadius = src.BobRadius;
                Gap = src.Gap;
                NewtonianEndEffectCorrection = src.NewtonianEndEffectCorrection;
                BobLength = src.BobLength;
                ConicalAngle = src.ConicalAngle;
                MeasurementPrecision = src.MeasurementPrecision;
                UseISOConvention = src.UseISOConvention;
                if (src.FixedSpeedList != null)
                {
                    FixedSpeedList = new List<double>();
                    foreach (double speed in src.FixedSpeedList)
                    {
                        FixedSpeedList.Add(speed);
                    }
                }
                
            }
        }
        /// <summary>
        /// cloning
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new CouetteRheometer(this);
        }
    }
}
