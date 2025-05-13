
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;

namespace Project.Pages.TA
{
    public class Room_BookingModel : PageModel
    {
        private readonly DB _db;

        public Room_BookingModel(DB db)
        {
            _db = db;
        }

        public int RemainingQuota { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Room code is required")]
        public string RoomCode { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Time slot is required")]
        public string TimeSlot { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Tutorial selection is required")]
        public string SelectedTutorial { get; set; }

        public List<Course> AvailableTutorials { get; set; } = new List<Course>();

        public class Course
        {
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public string DefaultRoom { get; set; }
        }

        public void OnGet()
        {
            var taId = GetTAId();
            if (taId == 0) return;

            // Get remaining quota
            RemainingQuota = _db.GetTARemainingQuota(taId);

            // Get assigned courses
            DataTable coursesDt = _db.GetTAAssignedCourses(taId);
            foreach (DataRow row in coursesDt.Rows)
            {
                AvailableTutorials.Add(new Course
                {
                    CourseCode = row["CourseCode"].ToString(),
                    CourseName = row["CourseName"].ToString(),
                    DefaultRoom = row["DefaultRoom"].ToString()
                });
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var taId = GetTAId();
            if (taId == 0)
            {
                ModelState.AddModelError("", "TA not found");
                return Page();
            }

            try
            {
                // Parse time slot input
                var timeSlotParts = TimeSlot.Split(' ');
                var day = timeSlotParts[0];
                var time = TimeSpan.Parse(timeSlotParts[1]);

                _db.SubmitTABooking(taId, RoomCode, day, time, SelectedTutorial);
                TempData["SuccessMessage"] = "Room booking requested successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error submitting booking: {ex.Message}");
                return Page();
            }
        }

        private int GetTAId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return _db.GetUserID(User.Identity.Name);
            }
            return 0;
        }
    }
}