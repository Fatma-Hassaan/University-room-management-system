using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Security.Claims;

namespace Project.Pages.Student
{
    public class My_RoomsModel : PageModel
    {
        private readonly DB _db;
        
        public List<Course> JTACourses { get; set; } = new List<Course>();
        public List<Course> EnrolledCourses { get; set; } = new List<Course>();

        public My_RoomsModel(DB db)
        {
            _db = db;
        }

        public void OnGet()
        {
            var studentId = GetStudentId();
            if (studentId == 0) return;

            DataTable coursesDt = _db.GetStudentCourses(studentId);
            
            foreach (DataRow row in coursesDt.Rows)
            {
                var course = new Course
                {
                    CourseCode = row["CourseCode"].ToString(),
                    CourseName = row["CourseName"].ToString(),
                    RoomNumber = $"{row["Building"]} {row["Room"]}",
                    RoomLocation = GetRoomLocation(row["Building"].ToString()),
                    TimeSlot = FormatTimeSlot(row["TimeSlot"].ToString())
                };

                if (row["CourseType"].ToString() == "JTA Course")
                    JTACourses.Add(course);
                else
                    EnrolledCourses.Add(course);
            }
        }

        private int GetStudentId()
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        private string GetRoomLocation(string building)
        {
            
            return building switch
            {
                "G" => "Main Campus - Building G",
                "B" => "Science Complex - Building B",
                _ => "University Campus"
            };
        }

        private string FormatTimeSlot(string rawSlot)
        {
           
            return DateTime.TryParse(rawSlot.Split(' ')[1], out var startTime)
                ? $"{rawSlot.Split(' ')[0]} {startTime:hh\\:mm tt}"
                : rawSlot;
        }

        public class Course
        {
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public string RoomNumber { get; set; }
            public string RoomLocation { get; set; }
            public string TimeSlot { get; set; }
        }
    }
}