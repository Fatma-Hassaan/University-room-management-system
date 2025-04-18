using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // My Rooms Page
    public IActionResult MyRooms()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var model = new StudentRoomsViewModel
        {
            JTACourses = _context.Courses
                .Where(c => c.IsJTA && c.StudentId == studentId)
                .Select(c => new CourseInfo
                {
                    CourseCode = c.Code,
                    CourseName = c.Name,
                    RoomNumber = c.Room.Number,
                    RoomLocation = c.Room.Building + " " + c.Room.Floor,
                    TimeSlot = c.TimeSlot
                }).ToList(),

            EnrolledCourses = _context.Courses
                .Where(c => !c.IsJTA && c.StudentId == studentId)
                .ToList()
        };

        return View(model);
    }

    // Room Booking Page (GET)
    [HttpGet]
    public IActionResult RoomBooking()
    {
        var courses = _context.Courses
            .Where(c => c.StudentId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            .ToList();
        
        var model = new RoomBookingViewModel
        {
            AvailableCourses = courses
        };

        return View(model);
    }

    // Room Booking Page (POST)
    [HttpPost]
    public IActionResult RoomBooking(RoomBookingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableCourses = _context.Courses
                .Where(c => c.StudentId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .ToList();
            return View(model);
        }

        var booking = new Booking
        {
            RoomCode = model.RoomCode,
            Reason = model.Reason,
            TimeSlot = model.TimeSlot,
            IsClinic = model.IsClinic,
            CourseCode = model.SelectedCourse,
            StudentId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Status = BookingStatus.Pending
        };

        _context.Bookings.Add(booking);
        _context.SaveChanges();

        return RedirectToAction("MyRooms");
    }
}

// GET: Report Page
[HttpGet]
public IActionResult Report()
{
    return View();
}

// POST: Handle Report Submission
[HttpPost]
public IActionResult Report(ReportViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    var report = new Report
    {
        RoomCode = model.RoomCode,
        Description = model.Complaint,
        Status = ReportStatus.Submitted,
        ReportedBy = User.FindFirstValue(ClaimTypes.NameIdentifier),
        ReportedAt = DateTime.Now
    };

    _context.Reports.Add(report);
    _context.SaveChanges();

    TempData["SuccessMessage"] = "Report submitted successfully!";
    return RedirectToAction("Report");
}