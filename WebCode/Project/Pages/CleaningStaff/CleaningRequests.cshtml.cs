using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Pages.CleaningStaff
{
    public class Cleaning_RequestsModel : PageModel
    {
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
