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
    public class DeleteModel : PageModel
    {
        private readonly OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public DeleteModel(OSDC.YPL.ModelCalibration.FromRheometer.Data.RheometerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rheogram Rheogram { get; set; }

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

            //Rheogram = await _context.Rheogram.FirstOrDefaultAsync(m => m.ID == id);
            Rheogram = RheogramManager.Instance.Get((int)id);

            if (Rheogram == null)
            {
                return NotFound();
            }
            return Page();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Rheogram = RheogramManager.Instance.Get((int)id);

            if (Rheogram != null)
            {
                RheogramManager.Instance.Remove(Rheogram);
            }

            return RedirectToPage("./Index");
        }
    }
}
