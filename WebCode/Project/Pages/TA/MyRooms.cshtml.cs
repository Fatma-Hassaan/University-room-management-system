// MyRooms.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;

namespace Project.Pages.TA
{
    public class MyRoomsModel : PageModel
    {
        private readonly DB _db;
        
        public List<TutorialRoom> TutorialRooms { get; set; } = new List<TutorialRoom>();
        public List<PendingRequest> PendingRequests { get; set; } = new List<PendingRequest>();

        public MyRoomsModel(DB db)
        {
            _db = db;
        }

        public void OnGet()
        {
            var taId = GetTAId();
            if (taId == 0) return;

            // Load approved tutorial rooms
            DataTable roomsDt = _db.GetTARooms(taId);
            foreach (DataRow row in roomsDt.Rows)
            {
                TutorialRooms.Add(new TutorialRoom
                {
                    CourseCode = row["CourseCode"].ToString(),
                    CourseName = row["CourseName"].ToString(),
                    RoomNumber = $"{row["Building"]} {row["Room"]}",
                    TimeSlot = row["TimeSlot"].ToString(),
                    Status = "Approved"
                });
            }

            // Load pending requests
            DataTable requestsDt = _db.GetTAPendingRequests(taId);
            foreach (DataRow row in requestsDt.Rows)
            {
                PendingRequests.Add(new PendingRequest
                {
                    CourseCode = row["CourseCode"].ToString(),
                    Reason = row["Reason"].ToString(),
                    Status = row["Status"].ToString()
                });
            }
        }

        private int GetTAId()
        {
            // Get TA ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        public class TutorialRoom
        {
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public string RoomNumber { get; set; }
            public string TimeSlot { get; set; }
            public string Status { get; set; }
        }

        public class PendingRequest
        {
            public string CourseCode { get; set; }
            public string Reason { get; set; }
            public string Status { get; set; }
        }
    }
}