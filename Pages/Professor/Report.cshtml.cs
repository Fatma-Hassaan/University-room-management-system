using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Project.Models;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.Professor
{
    public class ReportModel : PageModel
    {
        private readonly DB _db;

        public ReportModel()
        {
            _db = new DB();
        }

        [BindProperty]
        [Required(ErrorMessage = "Room code is required")]
        public string RoomCode { get; set; }

        [BindProperty]
        public ReportCategory Category { get; set; }

        [BindProperty]
        public UrgencyLevel Urgency { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public enum ReportCategory
        {
            EquipmentFailure,
            Maintenance,
            SafetyHazard,
            Other
        }

        public enum UrgencyLevel
        {
            Low,
            Medium,
            High
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
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = _db.GetUserID(User.Identity.Name);
            if (userId == 0)
            {
                ModelState.AddModelError("", "User not found");
                return Page();
            }

            try
            {
                // Start transaction
                using (var connection = new SqlConnection(_db.ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            //  RequestOrReport
                            var requestQuery = @"
                                INSERT INTO RequestOrReport (UserID, Condition, DayofR, HourofR, RType)
                                OUTPUT INSERTED.RID
                                VALUES (@UserID, 'Pending', GETDATE(), CONVERT(TIME, GETDATE()), 'Report')";

                            var requestCmd = new SqlCommand(requestQuery, connection, transaction);
                            requestCmd.Parameters.AddWithValue("@UserID", userId);
                            int newRid = (int)requestCmd.ExecuteScalar();


                            var reportQuery = @"
                                INSERT INTO Report (ID, RoomID, Complaint, IssueCategory, UrgencyLevel)
                                VALUES (@ID, @RoomID, @Complaint, @IssueCategory, @UrgencyLevel)";

                            var reportCmd = new SqlCommand(reportQuery, connection, transaction);
                            reportCmd.Parameters.AddWithValue("@ID", newRid);
                            reportCmd.Parameters.AddWithValue("@RoomID", RoomCode);
                            reportCmd.Parameters.AddWithValue("@Complaint", Description);
                            reportCmd.Parameters.AddWithValue("@IssueCategory", Category.ToString());
                            reportCmd.Parameters.AddWithValue("@UrgencyLevel", Urgency.ToString());
                            reportCmd.ExecuteNonQuery();

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                TempData["ReportSuccess"] = "Report submitted successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error submitting report. Please try again.");
                return Page();
            }
        }
    }
}