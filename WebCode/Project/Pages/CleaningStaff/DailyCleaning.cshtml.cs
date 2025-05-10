using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Data;

namespace Project.Pages.CleaningStaff
{
    public class Daily_CleaningModel : PageModel
    {
        public DB db { get; set; }

        public DataTable DailyStatuses { get; set; } = new DataTable();

        [BindProperty]
        public string SelectedRoom { get; set; }

        [BindProperty]
        public string SelectedCondition { get; set; }
        
      
        public DataTable RoomCleaningTable { get; set; }

        

        public Daily_CleaningModel()
        {
            db = new DB(); // Manual instance for now
        }

        public IActionResult OnGet()
        {
            // Check if user is logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                // If not logged in, redirect to login page
                return RedirectToPage("/Login");
            }
            
            DailyStatuses = db.LoadDailyCleaningStatuses();
            return Page();
            
        }

        public IActionResult OnPost()
        {   
            
            if (!string.IsNullOrEmpty(SelectedRoom) && !string.IsNullOrEmpty(SelectedCondition))
            {
                db.UpdateRoomCleaningStatus(SelectedRoom, SelectedCondition);
            }

            DailyStatuses = db.LoadDailyCleaningStatuses();
            return Page();
        }
    }
}