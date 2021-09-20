using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    public class Rheogram : INamable, IIdentifiable, ICopyable<Rheogram>
    {
        /// <summary>
        /// An identifier to further reference the rheometer measurements set
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// A name for this rheogram
        /// </summary>
        [DataType(DataType.Text)]
        [StringLength(128,MinimumLength =3)]
        public string Name { get; set; }
        /// <summary>
        /// A description of this rheogram
        /// </summary>
        [DataType(DataType.Text)]
        public string Description { get; set; }
        /// <summary>
        /// The standard deviation of the shear stress measurement
        /// Typically 0.25Pa for a Fann35 rheometer, 0.02Pa for an Anton Paar rheometer
        /// </summary>
        [Display(Name = "Shear stress measurement error (Pa)")]
        [DisplayFormat(
               ApplyFormatInEditMode = true,
               DataFormatString = "{0:0.00}",
               NullDisplayText = "")]
        [Range(0,100)]
        public double ShearStressStandardDeviation { 
            get; 
            set; 
        } = 0.01;
        /// <summary>
        ///  The list of measurements
        /// </summary>
        [NotMapped]
        public List<RheometerMeasurement> Measurements { get; set; } = new List<RheometerMeasurement>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public Rheogram()
        {
        }
        /// <summary>
        /// Copy constructor
        /// Also copy the ID from the source
        /// </summary>
        /// <param name="source"></param>
        public Rheogram(Rheogram source) : base()
        {
            if (source != null)
            {
                ID = source.ID;
                source.Copy(this);
            }
        }
        /// <summary>
        /// Copy this into the target but does not change the ID of target
        /// </summary>
        /// <param name="target"></param>
        public bool Copy(Rheogram target)
        {
            if (target != null)
            {
                if (Measurements == null)
                {
                    target.Measurements = null;
                }
                else
                {
                    target.Name = Name;
                    target.Description = Description;
                    target.ShearStressStandardDeviation = ShearStressStandardDeviation;
                    if (target.Measurements == null)
                    {
                        target.Measurements = new List<RheometerMeasurement>();
                    }
                    for (int i = 0; i < Measurements.Count; i++)
                    {
                        target.Measurements.Add(Measurements[i]);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Serialize this instance into a Json string
        /// </summary>
        /// <returns></returns>
        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// deserialize a string that is expected to be in Json into an instance of RheometerValues
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Rheogram FromJson(string str)
        {
            Rheogram values = null;
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    values = JsonConvert.DeserializeObject<Rheogram>(str);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            return values;
        }
    }
}

