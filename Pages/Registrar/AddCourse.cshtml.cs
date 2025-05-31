using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Models;

namespace Project.Pages.Registrar
{
    public class AddCourseModel : PageModel
    {
        public string TA_IDs_CSV { get; set; }
        public string JTA_IDs_CSV { get; set; }
        public string Student_IDs_CSV { get; set; }
        public DataTable DT { get; set; } = new DataTable();
        public DB db { get; set; }
        public AddCourseModel(DB db)
        {
            this.db = db;
        }
        [BindProperty]
        public CourseInputModel Input { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "Registrar")
            {
                return RedirectToPage("/Home");
            }
            DT = db.AllCoursesBriefed();

            if (Input == null)
                Input = new CourseInputModel();

            return Page();
        }
        public IActionResult OnPostDeleteCourse(string Course_Code)
        {
            if (string.IsNullOrWhiteSpace(Course_Code))
            {
                ModelState.AddModelError(string.Empty, "Invalid course code.");
                DT = db.AllCoursesBriefed();
                return Page();
            }

            db.DeleteCourse(Course_Code);
            DT = db.AllCoursesBriefed();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (!db.DoesProfessorExist(Input.Professor))
            {
                ModelState.AddModelError("Input.Professor", "Professor ID not found.");
            }
            foreach (var taId in ParseIds(Input.TAsIDs))
            {
                if (!db.DoesTAExist(taId))
                {
                    ModelState.AddModelError("Input.TAsIDs", $"TA ID {taId} does not exist.");
                }
            }
            foreach (var jtaId in ParseIds(Input.JTAsIDs))
            {
                if (!db.DoesJTAExist(jtaId))
                {
                    ModelState.AddModelError("Input.JTAsIDs", $"JTA ID {jtaId} does not exist.");
                }
            }
            foreach (var studentId in ParseIds(Input.StudentsIDs))
            {
                if (!db.DoesStudentExist(studentId))
                {
                    ModelState.AddModelError("Input.StudentsIDs", $"Student ID {studentId} does not exist.");
                }
            }
            if (!string.IsNullOrWhiteSpace(Input.LectureRoom) && !db.DoesRoomExist(Input.LectureRoom))
            {
                ModelState.AddModelError("Input.LectureRoom", "Lecture room ID does not exist.");
            }
            if (!string.IsNullOrWhiteSpace(Input.TutorialRoom) && !db.DoesRoomExist(Input.TutorialRoom))
            {
                ModelState.AddModelError("Input.TutorialRoom", "Tutorial room ID does not exist.");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            db.AddNewCourse(Input.CourseCode, Input.CourseName, Input.Professor, Input.LectureRoom, Input.LectureDay, TimeSpan.Parse(Input.LectureHour), Input.LectureDuration ?? 0, Input.TutorialRoom, Input.TutorialDay, TimeSpan.Parse(Input.TutorialHour), Input.TutorialDuration ?? 0);
            db.ADD_TA_JTA_Students(Input.CourseCode, ParseIds(Input.TAsIDs), ParseIds(Input.JTAsIDs), ParseIds(Input.StudentsIDs));

            return RedirectToPage();
        }
        

        public class CourseInputModel
        {
            [Required]
            [MaxLength(15, ErrorMessage = "Course code cannot exceed 15 characters.")]
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            [Required(ErrorMessage = "Professor ID is required.")]
            public int Professor { get; set; }
            public string TAsIDs { get; set; }
            public string JTAsIDs { get; set; }
            public string StudentsIDs { get; set; }
            public string LectureRoom { get; set; }
            public string LectureDay { get; set; }
            public string LectureHour { get; set; }
            public int? LectureDuration { get; set; }
            public string TutorialRoom { get; set; }
            public string TutorialDay { get; set; }
            public string TutorialHour { get; set; }
            public int? TutorialDuration { get; set; }
        }
        private List<int> ParseIds(string idString)
        {
            if (string.IsNullOrWhiteSpace(idString))
                return new List<int>();

            return idString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(id => id.Trim())
                          .Where(id => !string.IsNullOrEmpty(id))
                          .Select(id => int.TryParse(id, out var num) ? num : (int?)null)
                          .Where(id => id.HasValue)
                          .Select(id => id.Value)
                          .ToList();
        }
    }
}