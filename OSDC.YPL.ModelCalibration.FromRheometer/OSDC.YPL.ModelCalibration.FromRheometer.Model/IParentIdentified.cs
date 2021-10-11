using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    public interface IParentIdentified
    {
        /// <summary>
        /// the ID of a parent
        /// </summary>
        int ParentID { get; set; }
    }
}
