using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using Project.Models;

namespace Project.Pages.Student
{
    public class ReportModel : PageModel
    {
        private readonly DB _db;

        public ReportModel(DB db)
        {
            _db = db;
        }

        [BindProperty]
        [Required(ErrorMessage = "Room code is required")]
        public string RoomCode { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Complaint details are required")]
        public string Complaint { get; set; }

        public List<ReportHistory> ReportHistory { get; set; } = new List<ReportHistory>();

        public class ReportHistory
        {
            public string RoomCode { get; set; }
            public string ComplaintDetails { get; set; }
            public string SubmissionDateTime { get; set; }
            public string Status { get; set; }
        }

        public void OnGet()
        {
            var studentId = GetStudentId();
            if (studentId > 0)
            {
                LoadReportHistory(studentId);
            }
        }

        public IActionResult OnPost()
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
                using (var connection = new SqlConnection(_db.ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert into RequestOrReport
                            var requestQuery = @"
                                INSERT INTO RequestOrReport (UserID, Condition, DayofR, HourofR, RType)
                                OUTPUT INSERTED.RID
                                VALUES (@UserID, 'Pending', GETDATE(), CONVERT(TIME, GETDATE()), 'Report')";

                            var requestCmd = new SqlCommand(requestQuery, connection, transaction);
                            requestCmd.Parameters.AddWithValue("@UserID", studentId);
                            int newRid = (int)requestCmd.ExecuteScalar();

                            // Insert into Report
                            var reportQuery = @"
                                INSERT INTO Report (ID, RoomID, Complaint)
                                VALUES (@ReportId, @RoomCode, @Complaint)";

                            var reportCmd = new SqlCommand(reportQuery, connection, transaction);
                            reportCmd.Parameters.AddWithValue("@ReportId", newRid);
                            reportCmd.Parameters.AddWithValue("@RoomCode", RoomCode);
                            reportCmd.Parameters.AddWithValue("@Complaint", Complaint);
                            reportCmd.ExecuteNonQuery();

                            transaction.Commit();
                            TempData["SuccessMessage"] = "Report submitted successfully!";
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                return RedirectToPage();
            }
            catch
            {
                ModelState.AddModelError("", "Error submitting report. Please try again.");
                return Page();
            }
        }

        private void LoadReportHistory(int studentId)
        {
            DataTable dt = _db.GetStudentReports(studentId);
            foreach (DataRow row in dt.Rows)
            {
                ReportHistory.Add(new ReportHistory
                {
                    RoomCode = row["RoomCode"].ToString(),
                    ComplaintDetails = row["ComplaintDetails"].ToString(),
                    SubmissionDateTime = row["SubmissionDateTime"].ToString(),
                    Status = row["Status"].ToString()
                });
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