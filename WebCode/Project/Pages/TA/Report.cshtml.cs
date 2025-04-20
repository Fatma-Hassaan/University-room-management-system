using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Pages.TA
{
    
        public class ReportModel : PageModel
        {
            [Required]
            [Display(Name = "Room Code")]
            public string RoomCode { get; set; }

            [Required]
            [StringLength(500)]
            [Display(Name = "Issue Description")]
            public string Description { get; set; }

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
