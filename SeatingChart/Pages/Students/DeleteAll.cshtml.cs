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
    public class DeleteAllModel : PageModel
    {
        private readonly SeatingChart.Data.ChartContext _context;
        private readonly ILogger<DeleteModel> _logger;

        public DeleteAllModel(SeatingChart.Data.ChartContext context,
                           ILogger<DeleteModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        public string ErrorMessage { get; set; }

        public int? ChartNum {get;set;}

        public async Task<IActionResult> OnGetAsync(int? chartNum ,bool? saveChangesError = false)
        {
            ChartNum = chartNum;
            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete All failed. Try again";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _context.Students.RemoveRange(_context.Students);
                _context.Others.RemoveRange(_context.Others);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new {chartNum = ChartNum.ToString()});
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, ErrorMessage);

                return RedirectToAction("./DeleteAll",
                                     new {  saveChangesError = true });
            }
        }
    }
}