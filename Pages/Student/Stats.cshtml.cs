using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Student
{
    public class StatsModel : PageModel
    {
        public DB db { get; set; }
        public StatsModel(DB db)
        {
            this.db = db;
        }
        public int RemainingMonthlyQuota { get; set; }
        [BindProperty,
           Required(ErrorMessage = "Please enter how many hours you need.")]
        [Range(1, 100, ErrorMessage = "Value must be at least {1}.")]
        public int AdditionalHours { get; set; }

        [BindProperty,
         Required(ErrorMessage = "Please provide a reason.")]
        public string Reason { get; set; }


        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToPage("/Home");
            }
            else
            {
                RemainingMonthlyQuota = 10;

                return Page();
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            TempData["SuccessMessage"] = "Your request has been submitted.";
            return RedirectToPage();
        }
    }
}