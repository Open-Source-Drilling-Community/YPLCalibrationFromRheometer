using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OSDC.YPL.ModelCalibration.FromRheometer.Data;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;
using System.ComponentModel.DataAnnotations;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service.Pages.Rheograms
{
    public class EditModel : PageModel
    {
        private readonly OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext _context;

        /// <summary>
        /// an example rheometer measurement used to pick up the names of the RheometerMeasurement properties
        /// </summary>
        public RheometerMeasurement ExampleRheometerMeasurement { get; } = new RheometerMeasurement();

        public EditModel(OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext context)
        {
            _context = context;
        }

        public Rheogram Rheogram { get; set; }

        [BindProperty]
        public Rheogram WorkingRheogram { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Rheogram = RheogramManager.Instance.Get((int)id);
            if (Rheogram == null)
            {
                return NotFound();
            }
            if (Rheogram.Measurements != null)
            {
                foreach (RheometerMeasurement measurement in Rheogram.Measurements)
                {
                    if (measurement.ID < 0)
                    {
                        measurement.ParentID = Rheogram.ID;
                        IdentifiedObjectManager<RheometerMeasurement>.Instance.Add(measurement);
                    }
                    else
                    {
                        IdentifiedObjectManager<RheometerMeasurement>.Instance.Update(measurement.ID, measurement);
                    }
                }
            }
            WorkingRheogram = new Rheogram(Rheogram);
             return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (WorkingRheogram != null && WorkingRheogram.ID >= 0)
            {
                RheogramManager.Instance.Update(WorkingRheogram.ID, WorkingRheogram);
                List<int> measurementIDs = IdentifiedObjectManager<RheometerMeasurement>.Instance.GetIDs(WorkingRheogram.ID);
                if (measurementIDs != null)
                {
                    // remove the measurments from the RheometerMeasurementManager as they were posted there only while editing the rheogram
                    foreach (int id in measurementIDs)
                    {
                        IdentifiedObjectManager<RheometerMeasurement>.Instance.Remove(id);
                    }
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RheogramExists(int id)
        {
            return RheogramManager.Instance.Contains(id);
        }

    }
}
