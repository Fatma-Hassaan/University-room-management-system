using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Data;

namespace Project.Pages.CleaningStaff
{
    public class Cleaning_RequestsModel : PageModel
    {
        public DB db { get; set; }
        public DataTable CleaningRequestsTable { get; set; }

        public Cleaning_RequestsModel()
        {
            db = new DB(); // Manually create instance since not using DI
        }

        public IActionResult OnGet()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                // If not logged in, redirect to login page
                return RedirectToPage("/Login");
            }

            // Otherwise, load the cleaning requests (real or mock)
            CleaningRequestsTable = db.LoadCleaningRequests();

            return Page();
        }


        public IActionResult OnPostMarkAsDone(int requestId)
        {
            db.MarkCleaningRequestAsHandled(requestId);
            return RedirectToPage(); // Refresh the list after marking
        }

        
        
        
    }
}

