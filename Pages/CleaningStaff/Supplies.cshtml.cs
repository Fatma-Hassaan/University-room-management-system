using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Project.Models;
namespace Project.Pages.CleaningStaff
{
    public class SuppliesModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "Please specify what supplies you need.")]
        public string Supplies { get; set; }

        [BindProperty, Required(ErrorMessage = "Please choose an expected delivery time.")]
        [DataType(DataType.DateTime)]
        public DateTime ExpectedDeliveryTime { get; set; }
        public DB db;




        public SuppliesModel(DB database)
        {
            db = database;
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
                return Page();
            }
        }

        public IActionResult OnPost()
        {
            string email = HttpContext.Session.GetString("Email");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Save your supplies-request (Supplies + ExpectedDeliveryTime)
            // e.g. _db.SuppliesRequests.Add(...); _db.SaveChanges();
            db.InsertSuppliesRequest(email, ExpectedDeliveryTime, Supplies);

            TempData["SuccessMessage"] = "Your supplies request has been submitted.";
            return RedirectToPage();
        }
    }
}