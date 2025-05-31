using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using Project.Models;
using Microsoft.Data.SqlClient;
using static Project.Pages.Registrar.AddCourseModel;


namespace Project.Pages.Student
{
    public class Room_BookingModel : PageModel
    {
        private readonly DB _db;

        public Room_BookingModel(DB db)
        {
            _db = db;
        }

        [BindProperty]
        [Required(ErrorMessage = "Room code is required")]
        public string RoomCode { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Reason is required")]
        public string Reason { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Time slot is required")]
        public string TimeSlot { get; set; }

        [BindProperty]
        public bool IsClinic { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Course selection is required")]
        public string SelectedCourse { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "Student")
            {
                return RedirectToPage("/Home");
            }

            return Page();
        }
    

        public IActionResult OnPostRoomBooking()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var studentId = GetStudentId();
            if (studentId == 0)
            {
                ModelState.AddModelError("", "Student not found");
                return Page();
            }

            try
            {
                var (startTime, duration) = ParseTimeSlot(TimeSlot);

                using (var connection = new SqlConnection(_db.ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            
                            var requestQuery = @"
                                INSERT INTO RequestOrReport 
                                    (UserID, Condition, DayofR, HourofR, RType)
                                OUTPUT INSERTED.RID
                                VALUES
                                    (@UserID, 'Pending', @Day, @Hour, 'RoomBooking')";

                            using (var cmd = new SqlCommand(requestQuery, connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserID", studentId);
                                cmd.Parameters.AddWithValue("@Day", startTime.Date);
                                cmd.Parameters.AddWithValue("@Hour", startTime.TimeOfDay);
                                var newRid = (int)cmd.ExecuteScalar();

                                // Insert into RoomBooking
                                var bookingQuery = @"
                                    INSERT INTO RoomBooking 
                                        (ID, RoomID, Reason, TimeSlotDay, TimeSlotHour, Duration)
                                    VALUES
                                        (@ID, @RoomID, @Reason, @DayName, @Hour, @Duration)";

                                using (var cmd2 = new SqlCommand(bookingQuery, connection, transaction))
                                {
                                    cmd2.Parameters.AddWithValue("@ID", newRid);
                                    cmd2.Parameters.AddWithValue("@RoomID", RoomCode);
                                    cmd2.Parameters.AddWithValue("@Reason", Reason);
                                    cmd2.Parameters.AddWithValue("@DayName", startTime.DayOfWeek.ToString());
                                    cmd2.Parameters.AddWithValue("@Hour", startTime.TimeOfDay);
                                    cmd2.Parameters.AddWithValue("@Duration", duration);
                                    cmd2.ExecuteNonQuery();
                                }

                                // If clinic booking
                                if (IsClinic)
                                {
                                    var clinicQuery = @"
                                        INSERT INTO ClinicBookingRequest 
                                            (ID, CourseCode)
                                        VALUES
                                            (@ID, @CourseCode)";

                                    using (var cmd3 = new SqlCommand(clinicQuery, connection, transaction))
                                    {
                                        cmd3.Parameters.AddWithValue("@ID", newRid);
                                        cmd3.Parameters.AddWithValue("@CourseCode", SelectedCourse);
                                        cmd3.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                            TempData["SuccessMessage"] = "Booking request submitted successfully!";
                            return RedirectToPage();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error submitting booking request. Please try again.");
                return Page();
            }
        }

        private (DateTime startTime, int duration) ParseTimeSlot(string timeSlot)
        {
            try
            {
                var parts = timeSlot.Split(new[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                var startTime = DateTime.ParseExact(parts[0], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                var endTime = DateTime.ParseExact(parts[1], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                
                if (startTime >= endTime)
                {
                    ModelState.AddModelError("TimeSlot", "End time must be after start time");
                    throw new ArgumentException("Invalid time slot");
                }

                return (startTime, (int)(endTime - startTime).TotalMinutes);
            }
            catch
            {
                ModelState.AddModelError("TimeSlot", "Invalid format. Use DD/MM/YYYY HH:MM - HH:MM");
                throw;
            }
        }

        private int GetStudentId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return _db.GetUserID(User.Identity.Name);
            }
            return 0;
        }
    }
}