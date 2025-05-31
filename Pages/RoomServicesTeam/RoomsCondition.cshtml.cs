using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Data;
using static Project.Pages.Registrar.AddCourseModel;

namespace Project.Pages.RoomServicesTeam
{
    public class RoomConditionModel : PageModel
    {
        private readonly DB db;

        public DataTable RoomConditionTable { get; set; }
        public List<string> RoomList { get; set; }

        [BindProperty]
        public string SelectedRoomId { get; set; }

        public RoomConditionModel(DB db)
        {
            this.db = db;
        }
        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "RoomServicesMember")
            {
                return RedirectToPage("/Home");
            }

            RoomList = db.GetAvailableRoomIDs();
            RoomConditionTable = db.LoadRoomAvailabilityStatuses(); return Page();
        }
       

        public IActionResult OnPostToggleCondition()
        {
            if (!string.IsNullOrEmpty(SelectedRoomId))
            {
                db.ToggleRoomAvailability(SelectedRoomId);
            }

            return RedirectToPage();
        }
    }
}