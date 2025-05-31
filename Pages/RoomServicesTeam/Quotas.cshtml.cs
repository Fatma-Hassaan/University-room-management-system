using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Data;

namespace Project.Pages.RoomServicesTeam
{
    public class QuotaModel : PageModel
    {
        private readonly DB db;
        public DataTable QuotaRequestsTable { get; set; } = new DataTable();

        [BindProperty]
        public string SelectedStatus { get; set; }

        [BindProperty]
        public int SelectedRequestId { get; set; }

        public QuotaModel(DB db)
        {
            this.db = db;
        }

        // OnGet loads all the quota requests
        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "RoomServicesMember")
            {
                return RedirectToPage("/Home");
            }
            QuotaRequestsTable = db.LoadQuotaRequests();
            return Page();
        }

        // OnPost handles the update of the quota request status
        public IActionResult OnPostUpdateStatus()
        {
            if (SelectedRequestId != 0 && !string.IsNullOrEmpty(SelectedStatus))
            {
                db.UpdateQuotaStatus(SelectedRequestId, SelectedStatus); // Update status
            }

            // Reload the quota requests after update
            QuotaRequestsTable = db.LoadQuotaRequests();
            return RedirectToPage(); // Refresh the page to show updated data
        }
    }
}