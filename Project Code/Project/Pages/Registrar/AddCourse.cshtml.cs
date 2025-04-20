using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project.Pages.Registrar
{
    public class AddCourseModel : PageModel
    {
        [BindProperty]
        public CourseInputModel Input { get; set; }

        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else
            {
                return Page();
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Process the form data (save to database, etc.)
            // Redirect to prevent duplicate submissions
            return RedirectToPage();
        }

        public class CourseInputModel
        {
            public string CourseCode { get; set; }
            public string CourseName { get; set; }
            public string Professor { get; set; }
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
    }
}