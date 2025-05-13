using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Project.Pages.Admin
{
    public class UsersModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public SearchFilters Input { get; set; } = new SearchFilters();

        public List<UserResult> SearchResults { get; set; }
        public bool Searched { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            if (ModelState.IsValid && AnyFilterApplied())
            {
                SearchResults = GetMockData()
                    .Where(u => (string.IsNullOrEmpty(Input.UserType) || u.UserType == Input.UserType))
                    .Where(u => (string.IsNullOrEmpty(Input.Name) || u.Name.Contains(Input.Name)))
                    .ToList();

                Searched = true;
            }
            return Page();
            }


            private bool AnyFilterApplied()
            {
                return !string.IsNullOrEmpty(Input.UserType) ||
                       !string.IsNullOrEmpty(Input.Name);
            }

            private List<UserResult> GetMockData()
            {
                return new List<UserResult>
                {
                new UserResult { Id = 1, UserType = "Student", Name = "Ahmed Mohamed", Email = "ahmed@example.com" },
                new UserResult { Id = 2, UserType = "TA", Name = "Mariam Ali", Email = "mariam@example.com" },
                new UserResult { Id = 3, UserType = "Student", Name = "Omar Hassan", Email = "omar@example.com" },
                new UserResult { Id = 4, UserType = "Professor", Name = "Dr. Samir", Email = "samir@example.com" },
                new UserResult { Id = 5, UserType = "Admin", Name = "Admin User", Email = "admin@example.com" }
                };
            }


            public class SearchFilters
        {
            public string UserType { get; set; }

            [StringLength(100)]
            public string Name { get; set; }
        }

        public class UserResult
        {
            public int Id { get; set; }
            public string UserType { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
        
