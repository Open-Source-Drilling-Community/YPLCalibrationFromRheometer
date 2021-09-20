using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service
{
    /// <summary>
    /// A manager for a Rheograms. The manager implements the singleton pattern as defined by 
    /// Gamma, Erich, et al. "Design patterns: Abstraction and reuse of object-oriented design." 
    /// European Conference on Object-Oriented Programming. Springer, Berlin, Heidelberg, 1993.
    /// </summary>
    public class RheogramManager
    {
        private static RheogramManager instance_ = null;

        private Dictionary<int, Rheogram> data_ = new Dictionary<int, Rheogram>();
        private object lock_ = new object();

        /// <summary>
        /// default constructor is private when implementing a singleton pattern
        /// </summary>
        private RheogramManager()
        {

        }

        public static RheogramManager Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new RheogramManager();
                    instance_.FillDefault();
                }
                return instance_;

            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                lock (lock_)
                {
                    count = data_.Count;
                }
                return count;
            }
        }

        public bool Clear()
        {
            lock (lock_)
            {
                data_.Clear();
            }
            return true;
        }

        public bool Contains(int id)
        {
            bool contains = false;
            lock (lock_)
            {
                contains = data_.ContainsKey(id);
            }
            return contains;
        }
        public List<int> GetIDs()
        {
            List<int> ids = new List<int>();
            lock (lock_)
            {
                foreach (int key in data_.Keys)
                {
                    ids.Add(key);
                }
            }
            return ids;
        }

        public Rheogram Get(int rheogramID)
        {
            if (rheogramID > 0)
            {
                Rheogram rheogram = null;
                lock (lock_)
                {
                    data_.TryGetValue(rheogramID, out rheogram);
                }
                return rheogram;
            }
            else
            {
                return null;
            }
        }

        public bool Add(Rheogram rheogram)
        {
            bool result = false;
            if (rheogram != null)
            {
                lock (lock_)
                {
                    data_.Add(rheogram.ID, rheogram);
                    result = true;
                }
            }
            return result;
        }

        public bool Remove(Rheogram rheogram)
        {
            bool result = false;
            if (rheogram != null) 
            {
                result = Remove(rheogram.ID);
            }
            return result;
        }

        public bool Remove(int rheogramID)
        {
            bool result = false;
            if (data_.ContainsKey(rheogramID))
            {
                lock (lock_)
                {
                    data_.Remove(rheogramID);
                    result = true;
                }
            }
            return result;
        }

        public bool Update(int rheogramID, Rheogram updatedRheogram)
        {
            bool result = false;
            if (rheogramID > 0 && updatedRheogram != null)
            {
                lock (lock_)
                {
                    Rheogram rheogram = Get(rheogramID);
                    if (rheogram == null)
                    {
                        result = Add(updatedRheogram);
                    }
                    else
                    {
                        result = updatedRheogram.Copy(rheogram);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// populate with a few default rheograms
        /// </summary>
        private void FillDefault()
        {
            // a Newtonian rheological behavior
            Rheogram rheogram1 = new Rheogram();
            rheogram1.ID = 1;
            rheogram1.Name = "Newtonian fluid";
            double newtonianViscosity = 0.75; // Pa.s
            rheogram1.Description = "Newtonian viscosity " + newtonianViscosity.ToString("F3") + "Pa.s";
            rheogram1.ShearStressStandardDeviation = 0.01;
            int size = 0;
            for (double gammaDot = 1.0; gammaDot <= 1000.0; gammaDot *= 2.0)
            {
                size++;
            }
            rheogram1.Measurements = new List<RheometerMeasurement>();
            int idx = 0;
            for (double shearRate = 1.0; shearRate <= 1000.0; shearRate *= 2.0)
            {
                rheogram1.Measurements.Add(new RheometerMeasurement(shearRate, newtonianViscosity * shearRate));
                idx++;
            }
            Add(rheogram1);
            // a power law rheological behavior
            Rheogram rheogram2 = new Rheogram();
            rheogram2.ID = 2;
            rheogram2.Name = "Power law fluid";
            double consistencyIndex = 0.75;
            double flowBehaviorIndex = 0.5;
            rheogram2.Description = "Consistency index " + consistencyIndex.ToString("F3") + "Pa.s^n and flow behavior index " + flowBehaviorIndex.ToString("F3");
            rheogram2.ShearStressStandardDeviation = 0.01;
            rheogram2.Measurements = new List<RheometerMeasurement>();
            idx = 0;
            for (double shearRate = 1.0; shearRate <= 1000.0; shearRate *= 2.0)
            {
                rheogram2.Measurements.Add(new RheometerMeasurement(shearRate, consistencyIndex * System.Math.Pow(shearRate, flowBehaviorIndex)));
                idx++;
            }
            Add(rheogram2);
            // a Bingham plastic rheological behavior
            Rheogram rheogram3 = new Rheogram();
            rheogram3.ID = 3;
            rheogram3.Name = "Bingham plastic fluid";
            double yieldStress = 2.0;
            double plasticViscosity = 0.75;
            rheogram3.Description = "Yield stress " + yieldStress.ToString("F3") + "Pa and plastic viscosity " + plasticViscosity.ToString("F3") + "Pa.s";
            rheogram3.ShearStressStandardDeviation = 0.01;
            rheogram3.Measurements = new List<RheometerMeasurement>();
            idx = 0;
            for (double shearRate = 1.0; shearRate <= 1000.0; shearRate *= 2.0)
            {
                rheogram3.Measurements.Add(new RheometerMeasurement(shearRate, yieldStress + plasticViscosity * shearRate));
                idx++;
            }
            Add(rheogram3);
            // a perfect Herschel-Bulkley rheological behavior
            Rheogram rheogram4 = new Rheogram();
            rheogram4.ID = 4;
            rheogram4.Name = "Herschel-bulkley fluid";
            rheogram4.Description = "Yield stress " + yieldStress.ToString("F3") + "Pa, consistency index " + consistencyIndex.ToString("F3") + "Pa.s^n and flow behavior index " + flowBehaviorIndex.ToString("F3");
            rheogram4.ShearStressStandardDeviation = 0.01;
            rheogram4.Measurements = new List<RheometerMeasurement>();
            idx = 0;
            for (double shearRate = 1.0; shearRate <= 1000.0; shearRate *= 2.0)
            {
                rheogram4.Measurements.Add(new RheometerMeasurement(shearRate, yieldStress + consistencyIndex * System.Math.Pow(shearRate, flowBehaviorIndex)));
                idx++;
            }
            Add(rheogram4);
            // a perfect Quemada rheological behavior
            Rheogram rheogram5 = new Rheogram();
            rheogram5.ID = 5;
            rheogram5.Name = "Quemada fluid";
            double infiniteViscosity = 0.025; // Pa.s
            double gammaDotC = 300.0; // 1/s
            double p = 0.4; // dimensionless
            rheogram5.Description = "Zero visosity ∞, infinite viscosity " + infiniteViscosity.ToString("F3") + "Pa.s, reference shear rate " + gammaDotC.ToString("F3") + " 1/s and flow behavior index " + p.ToString("F3");
            rheogram5.ShearStressStandardDeviation = 0.01;
            rheogram5.Measurements = new List<RheometerMeasurement>();
            idx = 0;
            for (double shearRate = 1.0; shearRate <= 1000.0; shearRate *= 2.0)
            {
                rheogram5.Measurements.Add(new RheometerMeasurement(shearRate, infiniteViscosity * shearRate * System.Math.Pow((System.Math.Pow(gammaDotC, p) + System.Math.Pow(shearRate, p)) / System.Math.Pow(shearRate, p), 2.0)));
                idx++;
            }
            Add(rheogram5);
            // a real rheogram
            Rheogram rheogram6 = new Rheogram();
            rheogram6.ID = 6;
            rheogram6.Name = "Unweighted KCl/polymer fluid at 20°C";
            rheogram6.Description = "Measured with an Anton Paar rheometer Physica MCR301. 300s shearing at 100 1/s, 30s per measurements, from high to low shear rates.";
            rheogram6.ShearStressStandardDeviation = 0.01;
            rheogram6.Measurements = new List<RheometerMeasurement>() {
            new RheometerMeasurement(1, 3.2),
            new RheometerMeasurement(1.26, 3.32),
            new RheometerMeasurement(1.58, 3.44),
            new RheometerMeasurement(2, 3.57),
            new RheometerMeasurement(2.51, 3.7),
            new RheometerMeasurement(3.16, 3.84),
            new RheometerMeasurement(3.98, 3.99),
            new RheometerMeasurement(5.01, 4.14),
            new RheometerMeasurement(6.31, 4.31),
            new RheometerMeasurement(7.94, 4.49),
            new RheometerMeasurement(10, 4.69),
            new RheometerMeasurement(12.6, 4.9),
            new RheometerMeasurement(15.8, 5.13),
            new RheometerMeasurement(20, 5.38),
            new RheometerMeasurement(25.1, 5.66),
            new RheometerMeasurement(31.6, 5.96),
            new RheometerMeasurement(39.8, 6.3),
            new RheometerMeasurement(50.1, 6.67),
            new RheometerMeasurement(63.1, 7.09),
            new RheometerMeasurement(79.4, 7.57),
            new RheometerMeasurement(100, 8.1)
            };
            Add(rheogram6);
        }
    }
}
