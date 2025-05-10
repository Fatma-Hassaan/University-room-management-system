using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Data;

namespace Project.Pages.RoomServicesTeam
{
    public class ReportsModel : PageModel
    {
        public DB db { get; set; }
        public DataTable ReportsTable { get; set; } = new DataTable();

        public ReportsModel(DB db)
        {
            this.db = db;
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }

            ReportsTable = db.LoadAllReports();
            return Page();
        }

        public IActionResult OnPostMarkAs(string status, int reportId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }

            db.UpdateReportCondition(reportId, status);
            return RedirectToPage(); // refresh after update
        }
    }
}