using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Admin
{
    public class Create_UsersModel : PageModel
    {
        [BindProperty, Required]
        public string UserName { get; set; }

        [BindProperty, Required]
        public string UserId { get; set; }

        [BindProperty, Required, EmailAddress]
        public string Email { get; set; }

        [BindProperty, Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [BindProperty, Required]
        public string Type { get; set; }

        public List<SelectListItem> UserTypes { get; set; }

        public void OnGet()
        {
            PopulateUserTypes();
        }

        public IActionResult OnPost()
        {
            PopulateUserTypes();

            if (!ModelState.IsValid)
            {
                return Page();
            }



            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToPage();
        }

        private void PopulateUserTypes()
        {
            UserTypes = new List<SelectListItem>
            {
                new SelectListItem("Professor", "Professor"),
                new SelectListItem("Student",   "Student"),
                new SelectListItem("TA",        "TA")
            };
        }
    }
}
