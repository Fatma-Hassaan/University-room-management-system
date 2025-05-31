using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Data;

namespace Project.Pages.CleaningStaff
{
    public class CleaningRequestsModel : PageModel
    {
        public DB db { get; set; }
        public DataTable CleaningRequestsTable { get; set; }

        [BindProperty]
        public int SelectedRequestId { get; set; }

        [BindProperty]
        public string SelectedStatus { get; set; }

        public CleaningRequestsModel(DB db)
        {
            this.db = db;
        }
        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "CleaningStaffMember")
            {
                return RedirectToPage("/Home");
            }
            else
            {
                CleaningRequestsTable = db.LoadCleaningRequestsForStaff();
                return Page();
            }
        }
        public IActionResult OnPostUpdateRequestStatus()
        {
            if (SelectedRequestId != 0 && !string.IsNullOrEmpty(SelectedStatus))
            {
                db.UpdateCleaningRequestStatus(SelectedRequestId, SelectedStatus);
            }

            return RedirectToPage();
        }
    }
}
