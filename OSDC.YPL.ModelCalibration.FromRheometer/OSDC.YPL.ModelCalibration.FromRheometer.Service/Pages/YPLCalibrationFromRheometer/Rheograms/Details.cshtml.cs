using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OSDC.YPL.ModelCalibration.FromRheometer.Data;
using OSDC.YPL.ModelCalibration.FromRheometer.Model;
using System.ComponentModel.DataAnnotations;

namespace OSDC.YPL.ModelCalibration.FromRheometer.Service.Pages.Rheograms
{
    public class Chart
    {
        public object[] cols { get; set; }
        public object[] rows { get; set; }
    }

    public class DetailsTableModel
    {
        [Display(Name = "Shear rate (1/s)")]
        [DisplayFormat(
               ApplyFormatInEditMode = false,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double ShearRate { get; set; }
        [Display(Name = "Measured shear stress (Pa)")]
        [DisplayFormat(
               ApplyFormatInEditMode = false,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double MeasuredShearStress { get; set; }
        [Display(Name = "Estimated shear stress Zamora (Pa)")]
        [DisplayFormat(
               ApplyFormatInEditMode = false,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double EstimatedShearStressZamora { get; set; }
        [Display(Name = "Estimated shear stress Mullineux (Pa)")]
        [DisplayFormat(
               ApplyFormatInEditMode = false,
               DataFormatString = "{0:0.000}",
               NullDisplayText = "")]
        public double EstimatedShearStressMullineux { get; set; }
    }
    public class DetailsModel : PageModel
    {
        private readonly OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext _context;

        private static int lastID_ = -1;
        public DetailsModel(OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext context)
        {
            _context = context;
        }
        /// <summary>
        /// the referenced rheogram
        /// </summary>
        public Rheogram Rheogram { get; set; }
        /// <summary>
        /// an example rheometer measurement used to pick up the names of the RheometerMeasurement properties
        /// </summary>
        public DetailsTableModel ExampleRheometerMeasurement { get; } = new DetailsTableModel();
        public YPLModel YPLModelCalibratedWithZamora { get; } = new YPLModel();
        public YPLModel YPLModelCalibratedWithMullineux { get; } = new YPLModel();
        public List<DetailsTableModel> Measurements { get; } = new List<DetailsTableModel>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                lastID_ = -1;
                return NotFound();
            }
            lastID_ = (int)id;
            Rheogram = RheogramManager.Instance.Get((int)id);
            if (Rheogram == null)
            {
                return NotFound();
            }
            TransferData();
            return Page();
        }
        public ActionResult OnGetChartData()
        {
            Chart chart = null;
            if (Measurements == null || Measurements.Count == 0)
            {
                TransferData();
            }
            object[] rs = new object[Measurements.Count];
            for (int i = 0; i < Measurements.Count; i++)
            {
                rs[i] = new {c = new object[] {new {v = Measurements[i].ShearRate},
                                               new {v = Measurements[i].MeasuredShearStress},
                                               new {v = Measurements[i].EstimatedShearStressZamora},
                                               new {v = Measurements[i].EstimatedShearStressMullineux}} };
            }
            chart = new Chart
            {
                cols = new object[]
                {
                   new { id = "ShearRate", type = "number", label = "Shear-rate" },
                   new { id = "Measurements", type = "number", label = "Measurements" },
                   new { id = "YPLZamora", type = "number", label = "YPL Zamora/Kelessidis" },
                   new { id = "YPLMullineux", type = "number", label = "YPL Mullineux" }
                },
                rows = rs
            };
            var result = new JsonResult(chart);
            return result;
        }

        private void TransferData()
        {
            if (Rheogram == null && lastID_ >= 0)
            {
                Rheogram = RheogramManager.Instance.Get(lastID_);
            }
            if (Rheogram != null) 
            { 
                YPLModelCalibratedWithZamora.FitToKelessidis(Rheogram);
                YPLModelCalibratedWithMullineux.FitToMullineux(Rheogram);
                Measurements.Clear();
                foreach (var measurement in Rheogram.Measurements)
                {
                    DetailsTableModel values = new DetailsTableModel();
                    values.ShearRate = measurement.ShearRate;
                    values.MeasuredShearStress = measurement.ShearStress;
                    values.EstimatedShearStressZamora = YPLModelCalibratedWithZamora.Eval(measurement.ShearRate);
                    values.EstimatedShearStressMullineux = YPLModelCalibratedWithMullineux.Eval(measurement.ShearRate);
                    Measurements.Add(values);
                }
            }
        }
    }
}
