using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages
{
    [BindProperties (SupportsGet=true)]

    public class LoginModel : PageModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Home");
            }
            return Page();
        }

        public IActionResult OnPost()
        {   
            if (Email.Contains("admin-"))
            {
                HttpContext.Session.SetString("UserType", "Admin");
            }
            else if (Email.Contains("ta-"))
            {
                HttpContext.Session.SetString("UserType", "TA");
            }
            else if (Email.Contains("re-"))
            {
                HttpContext.Session.SetString("UserType", "Registrar");
            }
            else if (Email.Contains("cs-"))
            {
                HttpContext.Session.SetString("UserType", "CleaningStaff");
            }
            else if (Email.Contains("prof-"))
            {
                HttpContext.Session.SetString("UserType", "Professor");
            }
            else if (Email.Contains("rst-"))
            {
                HttpContext.Session.SetString("UserType", "RoomSevicesTeam");
            }
            else
            {
                HttpContext.Session.SetString("UserType", "Student");
            }

            return RedirectToPage("/Home");
            
        }
    }
}