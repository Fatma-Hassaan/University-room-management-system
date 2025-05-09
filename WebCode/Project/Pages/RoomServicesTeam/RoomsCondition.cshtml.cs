using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;

namespace Project.Pages.RoomServicesTeam
{
    public class Rooms_ConditionModel : PageModel
    {
        public DB db { get; set; }
        public Rooms_ConditionModel(DB db)
        {
            this.db = db;
        }
        public List<(string RoomId, string Condition)> RoomCondition { get; set; }

        [BindProperty]
        public string RoomId { get; set; }

        [BindProperty]
        public string NewCondition { get; set; }

        public List<string> AvailableRooms { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else
            {
                RoomCondition = new List<(string, string)>
                {
                ("F004", "Open"),
                ("S004", "Close"),
                ("S013 d", "Open"),
                ("S010 e", "Open"),
                };

                AvailableRooms = RoomCondition.Select(r => r.RoomId).ToList();

                return Page();
            }
        }


        public IActionResult OnPost()
        {
            // Handle logic to update condition here
            // e.g., Update DB: UpdateRoomCondition(RoomId, NewCondition);
            return RedirectToPage();
        }
    }
}