using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OSDC.YPL.ModelCalibration.FromRheometer.Data;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service.Pages.Rheograms
{
    public class CreateModel : PageModel
    {
        private readonly OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext _context;
        /// <summary>
        /// an example rheometer measurement used to pick up the names of the RheometerMeasurement properties
        /// </summary>
        public RheometerMeasurement ExampleRheometerMeasurement { get; } = new RheometerMeasurement();
        [BindProperty]
        public Rheogram WorkingRheogram { get; set; }

        public CreateModel(OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            WorkingRheogram = new Rheogram();
            WorkingRheogram.ID = RheogramManager.Instance.GetNextID();
            return Page();
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (WorkingRheogram != null && WorkingRheogram.ID >= 0)
            {
                // retrieve the measurements that have been posted on the RheometerMeasurementManager
                if (WorkingRheogram.Measurements == null)
                {
                    WorkingRheogram.Measurements = new List<RheometerMeasurement>();
                }
                WorkingRheogram.Measurements.Clear();
                List<int> measurementIDs = IdentifiedObjectManager<RheometerMeasurement>.Instance.GetIDs(WorkingRheogram.ID);
                if (measurementIDs != null)
                {
                    foreach (int id in measurementIDs)
                    {
                        RheometerMeasurement measurement = IdentifiedObjectManager<RheometerMeasurement>.Instance.Get(id);
                        WorkingRheogram.Measurements.Add(measurement);
                    }
                    // sort
                    WorkingRheogram.Measurements.Sort(ExampleRheometerMeasurement);
                    // remove the measurments from the RheometerMeasurementManager as they were posted there only while editing the rheogram
                    foreach (int id in measurementIDs)
                    {
                        IdentifiedObjectManager<RheometerMeasurement>.Instance.Remove(id);
                    }
                }
                RheogramManager.Instance.Add(WorkingRheogram);
            }
            return RedirectToPage("./Index");
        }
    }
}
