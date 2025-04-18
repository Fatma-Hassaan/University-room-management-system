[Authorize(Roles = "Professor")]
public class ProfessorController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProfessorController(ApplicationDbContext context)
    {
        _context = context;
    }

    // My Rooms Action
    public IActionResult MyRooms()
    {
        var model = new ProfessorMyRoomsViewModel
        {
            Courses = _context.Courses
                .Where(c => c.ProfessorId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .ToList(),
            ChangeRequests = _context.RoomChangeRequests
                .Where(r => r.ProfessorId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .ToList()
        };
        return View(model);
    }

    // Room Booking
    public IActionResult RoomBooking()
    {
        var model = new ProfessorRoomBookingViewModel
        {
            TimeSlots = GetAvailableTimeSlots(),
            RecurringOptions = new List<string> { "Weekly", "Daily", "Monthly" }
        };
        return View(model);
    }

    // Handle TA Requests
    [HttpPost]
    public IActionResult HandleRequest(int requestId, bool approve)
    {
        var request = _context.ApprovalRequests.Find(requestId);
        request.Status = approve ? RequestStatus.Approved : RequestStatus.Rejected;
        _context.SaveChanges();
        return RedirectToAction("RoomBooking");
    }
    
    private List<DateTime> GetAvailableTimeSlots() { ... }
}

[HttpGet]
public IActionResult Report()
{
    return View(new ProfessorReportViewModel());
}

[HttpPost]
public IActionResult Report(ProfessorReportViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    var report = new Report
    {
        RoomCode = model.RoomCode,
        Description = $"[PROFESSOR] {model.Description}",
        Category = model.Category.ToString(),
        Urgency = model.Urgency.ToString(),
        ReportedBy = User.Identity.Name,
        Role = "Professor",
        ReportDate = DateTime.Now,
        Status = ReportStatus.Submitted
    };

    _context.Reports.Add(report);
    _context.SaveChanges();

    TempData["ReportSuccess"] = "Report submitted successfully!";
    return RedirectToAction("Report");
}