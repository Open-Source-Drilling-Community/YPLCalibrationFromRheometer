using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    /// <summary>
    /// an interface for things that can be identified
    /// </summary>
    public interface IIdentifiable
    {
        int ID { get; set; }
    }
}
