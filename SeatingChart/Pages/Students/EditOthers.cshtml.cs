using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeatingChart.Data;
using SeatingChart.Models;

namespace SeatingChart.Pages.Students
{
    public class EditOthersModel : PageModel
    {
        private readonly SeatingChart.Data.ChartContext _context;

        public EditOthersModel(SeatingChart.Data.ChartContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int? ChartNum {get;set;}

        [BindProperty]
        public String namesInput { get; set; } = default!;

        [BindProperty]
        public Student Student { get; set; } = default!;

        public IActionResult OnGet(int? chartNum)
        {
            ChartNum = chartNum;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Start by deleting the Others; to be replaced by the new others
            _context.Others.RemoveRange(_context.Others);
            // Check if namesInput is not empty
            if (!string.IsNullOrWhiteSpace(namesInput))
            {
                // Split the input into an array of names 
                string[] namesArray = namesInput.Split('\n');

                int ord = 0;
                foreach (var name in namesArray)
                {
                    var nameParts = name.Trim().Split(' ');

                    var newOther = new Other
                    {
                        FirstName = nameParts[0],

                        MiddleName = nameParts.Length >= 3
                            ? nameParts.Skip(1).Take(nameParts.Length - 2).Aggregate((x, y) => x + " " + y)
                            : "",

                        LastName = nameParts.Length >= 2
                            ? nameParts.Last()
                            : "",

                        Order = ord
                    };                    
                    _context.Others.Add(newOther);
                    ord++;
                }

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index", new {chartNum = ChartNum.ToString()});
            }

            // Handle case where namesInput is empty
            ModelState.AddModelError("namesInput", "Please provide a list of names.");
            return Page();
        }

        public string getDisplayString(){
            string display = "";
            foreach(Other o in _context.Others){
                display += o.FirstName;
                if(o.MiddleName != null && o.MiddleName.Length > 0){
                    display += " " + o.MiddleName;
                }
                display += " " + o.LastName + "\n";
            }
            return display;
        }

    }
}
