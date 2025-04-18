using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Pages.RoomServicesTeam
{
    public class Rooms_ConditionModel : PageModel
    {
        public List<(string RoomId, string Condition)> RoomCondition { get; set; }

        [BindProperty]
        public string RoomId { get; set; }

        [BindProperty]
        public string NewCondition { get; set; }

        public List<string> AvailableRooms { get; set; }

        public void OnGet()
        {
            // Sample data
            RoomCondition = new List<(string, string)>
            {
                ("F004", "Open"),
                ("S004", "Close"),
                ("S013 d", "Open"),
                ("S010 e", "Open"),
            };

            AvailableRooms = RoomCondition.Select(r => r.RoomId).ToList();
        }

        public IActionResult OnPost()
        {
            // Handle logic to update condition here
            // e.g., Update DB: UpdateRoomCondition(RoomId, NewCondition);
            return RedirectToPage();
        }
    }
}