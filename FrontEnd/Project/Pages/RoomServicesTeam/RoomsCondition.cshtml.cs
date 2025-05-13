using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Data;

namespace Project.Pages.RoomServicesTeam
{
    public class Rooms_ConditionModel : PageModel
    {
        public DB db { get; set; }
        public Rooms_ConditionModel(DB db)
        {
            this.db = db;
        }

        public DataTable RoomsTable { get; set; }

        [BindProperty]
        public string RoomId { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }

            RoomsTable = db.LoadRoomConditions(); // This method should return a DataTable with RoomID & Condition columns
            return Page();
        }

        public IActionResult OnPostToggle()
        {
            db.ToggleRoomCondition(RoomId); // This method toggles Open/Close for the selected room
            return RedirectToPage();
        }
    }
}