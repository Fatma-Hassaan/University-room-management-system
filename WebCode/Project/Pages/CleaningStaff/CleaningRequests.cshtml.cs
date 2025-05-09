using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;

namespace Project.Pages.CleaningStaff
{
    public class Cleaning_RequestsModel : PageModel
    {
        public DB db { get; set; }
        public Cleaning_RequestsModel(DB db)
        {
            this.db = db;
        }
        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else
            {
                return Page();
            }
        }
    }
}
