using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SeatingChart.Data;
using SeatingChart.Models;

namespace SeatingChart.Pages.Students
{
    public class EditModel : PageModel
    {
        private readonly SeatingChart.Data.ChartContext _context;

        public EditModel(SeatingChart.Data.ChartContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Student Student { get; set; } = default!;

        [BindProperty]
        public int testing { get; set; } = default!;

        [BindProperty]
        public int? ChartNum {get;set;}
        [BindProperty]
        public int IsGrad { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? chartNum)
        {
            ChartNum = chartNum;
            if (id == null)
            {
                return NotFound();
            
            }

            var student = await _context.Students.FirstOrDefaultAsync(m => m.ID == (int)id);
            if (student == null)
            {
                return NotFound();
            }
            Student = student;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            Student.isGrad = IsGrad == 1 ? true : false;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(Student.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index",new{chartNum = ChartNum.ToString()});
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
