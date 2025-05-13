// Report.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace Project.Pages.TA
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
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public List<ReportRecord> Reports { get; set; } = new List<ReportRecord>();

        public class ReportRecord
        {
            public string RoomCode { get; set; }
            public string IssueDescription { get; set; }
            public string SubmissionDateTime { get; set; }
            public string Status { get; set; }
        }

        public void OnGet()
        {
            var taId = GetTAId();
            if(taId > 0)
            {
                LoadReports(taId);
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
                _db.SubmitTAReport(taId, RoomCode, Description);
                TempData["SuccessMessage"] = "Report submitted successfully!";
                return RedirectToPage();
            }
            catch
            {
                ModelState.AddModelError("", "Error submitting report");
                return Page();
            }
        }

        private void LoadReports(int taId)
        {
            DataTable dt = _db.GetTAReports(taId);
            foreach (DataRow row in dt.Rows)
            {
                Reports.Add(new ReportRecord
                {
                    RoomCode = row["RoomCode"].ToString(),
                    IssueDescription = row["IssueDescription"].ToString(),
                    SubmissionDateTime = row["SubmissionDateTime"].ToString(),
                    Status = row["Status"].ToString()
                });
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