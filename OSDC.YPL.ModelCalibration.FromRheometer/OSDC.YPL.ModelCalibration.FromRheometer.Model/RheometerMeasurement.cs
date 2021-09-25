using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    public class RheometerMeasurement : IIdentifiable, IParentIdentified, ICopyable<RheometerMeasurement>, IUndefinable, IComparer<RheometerMeasurement>
    {
        public int ID { get; set; } = -1;

        public int ParentID { get; set; } = -1;

        /// <summary>
        /// The shear rate is expected in SI unit, i.e. dimension [T^-1](1/s)
        /// </summary>
        [Display(Name = "Shear rate (1/s)")]
        [DisplayFormat(
               ApplyFormatInEditMode = true,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double ShearRate { 
            get; 
            set; 
        }
        /// <summary>
        /// The shear stress is expected in SI unit, i.e., dimension [ML^-1T^-2](Pa)
        /// </summary>
        [Display(Name = "Shear stress (Pa)")]
        [DisplayFormat(
               ApplyFormatInEditMode = true,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double ShearStress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RheometerMeasurement()
        {
            SetUndefined();
        }
        /// <summary>
        /// initialization constructor
        /// </summary>
        /// <param name="shearRate"></param>
        /// <param name="shearStress"></param>
        public RheometerMeasurement(double shearRate, double shearStress)
        {
            ShearRate = shearRate;
            ShearStress = shearStress;
            ID = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool Copy(RheometerMeasurement target)
        {
            target.ShearRate = ShearRate;
            target.ShearStress = ShearStress;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsUndefined()
        {
            return ID == 0 && ShearRate == 0 && ShearStress == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        public void SetUndefined()
        {
            ID = 0;
            ShearRate = 0;
            ShearStress = 0;
        }

        public int Compare(RheometerMeasurement x, RheometerMeasurement y)
        {
            if (x == null || y == null)
            {
                return 0;
            }
            else
            {
                if (Numeric.EQ(x.ShearRate, y.ShearRate, 1e-6))
                {
                    return 0;
                }
                else if (Numeric.GT(x.ShearRate, y.ShearRate, 1e-6))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
