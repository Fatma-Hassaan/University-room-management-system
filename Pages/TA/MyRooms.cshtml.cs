// MyRooms.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
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

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "TA")
            {
                return RedirectToPage("/Home");
            }
            else
            {
                var taId = GetTAId();
                if (taId == 0) return Page();
               

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
                return Page();

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