using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;

namespace Project.Pages.CleaningStaff
{
    public class Daily_CleaningModel : PageModel
    {
        public DB db { get; set; }
        public Daily_CleaningModel(DB db)
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
