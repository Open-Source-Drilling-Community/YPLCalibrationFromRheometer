using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Model
{
    public interface IUndefinable
    {
        bool IsUndefined();

        void SetUndefined();
    }
}
