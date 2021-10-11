using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Data
{
    public class RheometerContext : DbContext
    {
        public RheometerContext (DbContextOptions<RheometerContext> options)
            : base(options)
        {
        }

        public DbSet<OSDC.YPL.ModelCalibration.FromRheometer.Model.Rheogram> Rheogram { get; set; }
    }
}
