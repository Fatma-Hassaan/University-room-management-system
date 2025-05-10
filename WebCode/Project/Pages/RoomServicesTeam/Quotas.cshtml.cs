using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using System.Data;

namespace Project.Pages.RoomServicesTeam
{
    public class QuotasModel : PageModel
    {
        public DB db { get; set; }
        public DataTable QuotaRequests { get; set; } = new DataTable();

        public QuotasModel(DB db)
        {
            this.db = db;
        }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }

            QuotaRequests = db.LoadQuotaRequests(); 
            return Page();
        }

        public IActionResult OnPostSetStatus(int requestId, string status)
        {
           if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }

            db.UpdateQuotaStatus(requestId, status); 
            return RedirectToPage();
        }
    }
}
