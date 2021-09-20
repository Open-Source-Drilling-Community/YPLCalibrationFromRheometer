using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OSDC.YPL.ModelCalibration.FromRheometer.Data;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service.Pages.Rheograms
{
    public class IndexModel : PageModel
    {
        private readonly OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext _context;

        public IndexModel(OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext context)
        {
            _context = context;
        }

        public IList<Rheogram> Rheograms { get;set; }

        public async Task OnGetAsync()
        {
            //Rheograms = await RheogramManager.Instance.Rheograms.ToListAsync(); // _context.Rheogram.ToListAsync();
            Rheograms = new List<Rheogram>();
            List<int> ids = RheogramManager.Instance.GetIDs();
            foreach (int id in ids)
            {
                Rheograms.Add(RheogramManager.Instance.Get(id));
            }
        }
    }
}
