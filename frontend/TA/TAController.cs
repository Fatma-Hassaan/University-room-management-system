[Authorize(Roles = "TA")]
public class TAController : Controller
{
    private readonly ApplicationDbContext _context;

    public TAController(ApplicationDbContext context)
    {
        _context = context;
    }

    // My Rooms (TA Version)
    public IActionResult MyRooms()
    {
        var taId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var model = new TARoomsViewModel
        {
            TutorialRooms = _context.Tutorials
                .Where(t => t.TAId == taId)
                .Select(t => new TutorialRoomInfo
                {
                    CourseCode = t.CourseCode,
                    RoomNumber = t.Room.Number,
                    TimeSlot = t.TimeSlot,
                    Status = t.Status
                }).ToList(),
            
            PendingRequests = _context.RoomChangeRequests
                .Where(r => r.TAId == taId)
                .ToList()
        };

        return View(model);
    }

    // Room Booking (TA Version)
    [HttpGet]
    public IActionResult RoomBooking()
    {
        var model = new TARoomBookingViewModel
        {
            AvailableTutorials = _context.Tutorials
                .Where(t => t.TAId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .ToList(),
            RemainingQuota = CalculateRemainingQuota()
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult RoomBooking(TARoomBookingViewModel model)
    {
        // TA-specific booking logic
        if (model.BookingHours > CalculateRemainingQuota())
        {
            ModelState.AddModelError("", "Exceeds monthly quota");
        }
        
        if (ModelState.IsValid)
        {
            // Save booking
            return RedirectToAction("MyRooms");
        }
        
        model.AvailableTutorials = _context.Tutorials
            .Where(t => t.TAId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            .ToList();
        return View(model);
    }

    // Report (TA Version)
    [HttpGet]
    public IActionResult Report()
    {
        return View(new TAReportViewModel());
    }

    [HttpPost]
    public IActionResult Report(TAReportViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Handle TA-specific reporting
            return RedirectToAction("Report");
        }
        return View(model);
    }

    private decimal CalculateRemainingQuota()
    {
        // TA quota logic (20h base + adjustments)
        return 20 - _context.TABookings
            .Where(b => b.TAId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            .Sum(b => b.Duration);
    }
}