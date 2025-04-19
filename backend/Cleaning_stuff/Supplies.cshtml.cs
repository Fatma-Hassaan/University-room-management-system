using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Project.Pages.CleaningStaff
{
    public class SuppliesModel : PageModel
    {
        [BindProperty, Required(ErrorMessage = "Please specify what supplies you need.")]
        public string Supplies { get; set; }

        [BindProperty, Required(ErrorMessage = "Please choose an expected delivery time.")]
        [DataType(DataType.DateTime)]
        public DateTime? ExpectedDeliveryTime { get; set; }

        /// <summary>
        /// Only enable the button if both fields have values
        /// </summary>
        public bool CanSubmit =>
            !string.IsNullOrWhiteSpace(Supplies)
            && ExpectedDeliveryTime.HasValue;

        public void OnGet()
        {
            // nothing to seed on GET
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            TempData["SuccessMessage"] = "Your supplies request has been submitted.";
            return RedirectToPage();
        }
    }
}
