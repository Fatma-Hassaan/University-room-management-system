using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.Data;
using Project.Models;

namespace Project.Pages.RoomServicesTeam
{
    public class CleaningRequestsModel : PageModel
    {
        public DB db { get; set; }
        public DataTable RequestsTable { get; set; }
        public List<string> AvailableRooms { get; set; }

        public CleaningRequestsModel(DB db)
        {
            this.db = db;
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }

            LoadPageData();
            return Page();
        }

        public IActionResult OnPostNewRequest(string RoomID)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }

            int userId = db.GetUserID(HttpContext.Session.GetString("Email")); // Assuming Email is stored
            db.InsertCleaningRequest(userId, RoomID);

            return RedirectToPage(); // Refresh the page
        }

        private void LoadPageData()
        {
            RequestsTable = db.LoadAllCleaningRequests(); 
            AvailableRooms = db.GetAvailableRoomIDs();    
        }
    }
}
