using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;

public class Daily_CleaningModel : PageModel
{
    private readonly DB db;
    [BindProperty]
    public DataTable RoomCleaningTable { get; set; }

    public Daily_CleaningModel(DB db)
    {
        this.db = db;
    }
    

    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
        {
           return RedirectToPage("/Login");
        }

        RoomCleaningTable = db.LoadRoomCleaningStatus();
        return Page();
    }
}