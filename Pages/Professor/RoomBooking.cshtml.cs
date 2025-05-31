using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using Project.Models;
using Microsoft.Data.SqlClient;


namespace Project.Pages.Professor
{
    public class Room_BookingModel : PageModel
    {
        private readonly DB _db;

        public Room_BookingModel()
        {
            _db = new DB();
        }

        [BindProperty]
        [Required(ErrorMessage = "Room code is required")]
        public string RoomCode { get; set; }


        [BindProperty]
        [Required]
        [Display(Name = "Time Slot")]
        public DateTime SelectedTime { get; set; }

        public List<DateTime> TimeSlots { get; set; } = new List<DateTime>();
        public List<JTARequest> PendingRequests { get; set; } = new List<JTARequest>();

        public class JTARequest
        {
            public int Id { get; set; }
            public string StudentName { get; set; }
            public string CourseCode { get; set; }
            public int RequestedHours { get; set; }
            public string RelatedRoom { get; set; }
            public string Reason { get; set; }
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "Professor")
            {
                return RedirectToPage("/Home");
            }
            LoadTimeSlots();
            LoadPendingRequests();
            return Page();
        }

        private void LoadTimeSlots()
        {
            // Implement time slot generation logic
            // Example: Generate slots for next 7 days
            var start = DateTime.Today.AddDays(1).AddHours(8);
            for (int i = 0; i < 28; i++) // 7 days * 4 slots per day
            {
                TimeSlots.Add(start.AddHours(i * 2));
            }
        }

        private void LoadPendingRequests()
        {
            var professorId = GetProfessorId();
            if (professorId == 0) return;

            DataTable dt = _db.GetPendingJTARequests(professorId);
            foreach (DataRow row in dt.Rows)
            {
                PendingRequests.Add(new JTARequest
                {
                    Id = Convert.ToInt32(row["RequestID"]),
                    StudentName = row["StudentName"].ToString(),
                    CourseCode = row["CourseCode"].ToString(),
                    RequestedHours = Convert.ToInt32(row["RequestedHours"]),
                    RelatedRoom = row["RelatedRoom"].ToString(),
                    Reason = row["Reason"].ToString()
                });
            }
        }

        public IActionResult OnPostRoomBooking()
        {
            if (!ModelState.IsValid)
            {
                LoadTimeSlots();
                LoadPendingRequests();
                return Page();
            }

            var professorId = GetProfessorId();
            if (professorId == 0)
            {
                ModelState.AddModelError("", "Professor not found");
                return Page();
            }

            try
            {
                using (var connection = new SqlConnection(_db.ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // RequestOrReport
                            var requestQuery = @"
                                INSERT INTO RequestOrReport (UserID, Condition, DayofR, HourofR, RType)
                                OUTPUT INSERTED.RID
                                VALUES (@UserID, 'Pending', @Day, @Hour, 'RoomBooking')";

                            var requestCmd = new SqlCommand(requestQuery, connection, transaction);
                            requestCmd.Parameters.AddWithValue("@UserID", professorId);
                            requestCmd.Parameters.AddWithValue("@Day", SelectedTime.Date);
                            requestCmd.Parameters.AddWithValue("@Hour", SelectedTime.TimeOfDay);

                            int newRid = (int)requestCmd.ExecuteScalar();

                            // RoomBooking
                            var bookingQuery = @"
                                INSERT INTO RoomBooking (ID, RoomID, TimeSlotDay, TimeSlotHour, Duration, CourseCode)
                                VALUES (@ID, @RoomID, @DayName, @Hour, @Duration, @CourseCode)";

                            var bookingCmd = new SqlCommand(bookingQuery, connection, transaction);
                            bookingCmd.Parameters.AddWithValue("@ID", newRid);
                            bookingCmd.Parameters.AddWithValue("@RoomID", RoomCode);
                            bookingCmd.Parameters.AddWithValue("@DayName", SelectedTime.DayOfWeek.ToString());
                            bookingCmd.Parameters.AddWithValue("@Hour", SelectedTime.TimeOfDay);
                            bookingCmd.Parameters.AddWithValue("@Duration", 120); // Fixed 2-hour duration
                            bookingCmd.Parameters.AddWithValue("@CourseCode", "CIE101"); // Should be dynamic

                            bookingCmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                TempData["SuccessMessage"] = "Room booking request submitted successfully!";
                return RedirectToPage();
            }
            catch
            {
                ModelState.AddModelError("", "Error submitting booking request");
                LoadTimeSlots();
                LoadPendingRequests();
                return Page();
            }
        }

        public IActionResult OnPostHandleRequest(int requestId, bool approve)
        {
            try
            {
                _db.UpdateRequestStatus(requestId, approve ? "Approved" : "Rejected");
                TempData["StatusMessage"] = $"Request {(approve ? "approved" : "rejected")} successfully";
            }
            catch
            {
                TempData["StatusMessage"] = "Error processing request";
            }
            return RedirectToPage();
        }

        private int GetProfessorId()
        {

            return 789;
        }
    }
}