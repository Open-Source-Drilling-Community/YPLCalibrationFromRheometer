using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace YPLCalibrationFromRheometer.Test
{
    public partial class Rheogram
    {
        private static Random rnd_ = null;

        /// <summary>
        /// default constructor
        /// </summary>
        public Rheogram()
        {
            if (rnd_ == null)
            {
                InitializeRandomGenerator();
            }
            ID = rnd_.Next();
            if (Measurements == null)
            {
                Measurements = new List<RheometerMeasurement>();
            }
        }
        /// <summary>
        /// copy constructor
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
                    if (target.Measurements == null)
                    {
                        target.Measurements = new List<RheometerMeasurement>();
                    }
                    target.Measurements.Clear();
                    foreach (RheometerMeasurement measurement in Measurements)
                    {
                        target.Measurements.Add(measurement);
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return values;
        }

        /// <summary>
        /// initialization of a random number generator using a seed calculated from a global unique identifier (GUID)
        /// </summary>
        private void InitializeRandomGenerator()
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            int sum = 0;
            foreach (byte b in bytes)
            {
                if (sum < int.MaxValue - 256)
                {
                    sum += (int)b;
                }
            }
            rnd_ = new Random(sum);
        }
    }
}
