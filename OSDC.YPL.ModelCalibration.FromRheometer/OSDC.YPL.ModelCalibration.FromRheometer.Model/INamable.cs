using System;
using System.Collections.Generic;
using System.Text;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    /// <summary>
    /// an interface for things that can be named
    /// </summary>
    public interface INamable
    {
        string Name { get; set; }
    }
}
