using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;
using Project.Pages.TA;
using System.Data;

namespace Project.Pages.Professor
{
    public class My_RoomsModel : PageModel
    {
        private readonly DB _db;

        public My_RoomsModel()
        {
            _db = new DB();
        }

        public List<Course> Courses { get; set; } = new List<Course>();
        public List<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();

        public void OnGet()
        {
            
            var professorId = int.Parse(User.FindFirst("ProfessorId").Value);

            
            DataTable coursesDt = _db.GetProfessorCourses(professorId);
            foreach (DataRow row in coursesDt.Rows)
            {
                Courses.Add(new Course
                {
                    CourseCode = row["CourseCode"].ToString(),
                    CourseName = row["CourseName"].ToString(),
                    CurrentRoom = row["CurrentRoom"].ToString(),
                    Building = row["Building"].ToString(),
                    Schedule = $"{row["LectureDay"]} {row["StartTime"]} - {row["EndTime"]}"
                });
            }

            // Load change requests
            DataTable requestsDt = _db.GetProfessorChangeRequests(professorId);
            foreach (DataRow row in requestsDt.Rows)
            {
                ChangeRequests.Add(new ChangeRequest
                {
                    CourseCode = row["CourseCode"].ToString(),
                    RequestedRoom = row["NewRoom"].ToString(),
                    Reason = row["Reason"].ToString(),
                    RequestID = (int)row["RequestId"]
                });
            }
        }

        
        public class Course
        {
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public string CurrentRoom { get; set; }
            public string Building { get; set; }
            public string Schedule { get; set; }
            public string RoomNumber => $"{Building} {CurrentRoom}";
        }

        public class ChangeRequest
        {
            public string CourseCode { get; set; }
            public string RequestedRoom { get; set; }
            public string Reason { get; set; }
            public int RequestID { get; set; }
        }
    }
}