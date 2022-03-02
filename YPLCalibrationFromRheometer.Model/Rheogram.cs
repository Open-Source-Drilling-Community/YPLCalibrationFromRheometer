using System;
using System.Collections.Generic;

namespace YPLCalibrationFromRheometer.Model
{
    public class Rheogram : ICloneable
    {
        /// <summary>
        /// an ID for the Rheogram
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// a name for the Rheogram
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// a description for the Rheogram
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the standard deviation of the shear stress measurement
        /// typically 0.25Pa for a Fann35 rheometer, 0.02Pa for an Anton Paar rheometer
        /// </summary>
        public double ShearStressStandardDeviation
        {
            get;
            set;
        } = 0.01;

        /// <summary>
        ///  a list of RheometerMeasurement for Rheogram
        /// </summary>
        public List<RheometerMeasurement> RheometerMeasurementList { get; set; } = new List<RheometerMeasurement>();

        public int Count => RheometerMeasurementList.Count;

        public bool IsReadOnly => false;

        /// <summary>
        /// default constructor
        /// </summary>
        public Rheogram() : base()
        {

        }

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        public Rheogram(Rheogram src) : base()
        {
            if (src != null)
            {
                src.Copy(this);
            }
        }
        
        /// <summary>
        /// copy everything except the ID
        /// </summary>
        /// <param name="dest"></param>
        /// <returns></returns>
        public bool Copy(Rheogram dest)
        {
            if (dest != null)
            {
                dest.Name = Name;
                dest.Description = Description;
                dest.ShearStressStandardDeviation = ShearStressStandardDeviation;
                if (dest.RheometerMeasurementList == null)
                {
                    dest.RheometerMeasurementList = new List<RheometerMeasurement>();
                }
                dest.RheometerMeasurementList.Clear();
                if (RheometerMeasurementList != null)
                {
                    foreach (RheometerMeasurement itData in RheometerMeasurementList)
                    {
                        RheometerMeasurement iterData1 = new RheometerMeasurement();
                        itData.Copy(iterData1);
                        dest.RheometerMeasurementList.Add(iterData1);
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
        /// cloning function (including the ID)
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            Rheogram copy = new Rheogram(this);
            copy.ID = ID;
            return copy;
        }

        /// <summary>
        /// index accessor. It makes sure that the start of the section is actually the end of the previous section (if any).
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public RheometerMeasurement this[int index]
        {
            get => RheometerMeasurementList[index];
            set
            {
                RheometerMeasurementList[index] = value;
            }
        }

        /// <summary>
        /// ensure to link start to the end of the previous section
        /// </summary>
        /// <param name="item"></param>
        public void Add(RheometerMeasurement item)
        {
            RheometerMeasurementList.Add(item);
        }

        public void Clear()
        {
            RheometerMeasurementList.Clear();
        }

        public bool Contains(RheometerMeasurement item)
        {
            return RheometerMeasurementList.Contains(item);
        }

        public void CopyTo(RheometerMeasurement[] array, int arrayIndex)
        {
            RheometerMeasurementList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<RheometerMeasurement> GetEnumerator()
        {
            return RheometerMeasurementList.GetEnumerator();
        }

        public int IndexOf(RheometerMeasurement item)
        {
            return RheometerMeasurementList.IndexOf(item);
        }

        public void Insert(int index, RheometerMeasurement item)
        {
            RheometerMeasurementList.Insert(index, item);
        }

        public bool Remove(RheometerMeasurement item)
        {
            return RheometerMeasurementList.Remove(item);
        }

        public void RemoveAt(int index)
        {
            RheometerMeasurementList.RemoveAt(index);
        }
    }
}
