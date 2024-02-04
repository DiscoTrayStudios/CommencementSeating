using SeatingChart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; 
using SeatingChart.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SeatingChart.Pages.Students
{
    public class DeleteModel : PageModel
    {
        private readonly SeatingChart.Data.ChartContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteModel(SeatingChart.Data.ChartContext context,
                           ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Student Student { get; set; }
        public string ErrorMessage { get; set; }
        public int? ChartNum {get;set;}

        public async Task<IActionResult> OnGetAsync(int? id,int? chartNum, bool? saveChangesError = false)
        {
            ChartNum = chartNum;

            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == (int)id);

            if (Student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = String.Format("Delete {ID} failed. Try again", id);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            try
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new{chartNum = ChartNum.ToString()});
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ErrorMessage);

                return RedirectToAction("./Delete",
                                     new { id, saveChangesError = true });
            }
        }
    }
}