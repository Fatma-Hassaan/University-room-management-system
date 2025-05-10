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
            db = new DB(); 
        }

        public IActionResult OnGet()
        {
        
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
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
