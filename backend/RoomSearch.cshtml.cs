using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.Pages
{
    public class RoomSearchModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public SearchFilters Input { get; set; } = new SearchFilters();

        public List<RoomResult> SearchResults { get; set; }
        public bool Searched { get; set; }

        public void OnGet()
        {
            if (ModelState.IsValid && AnyFilterApplied())
            {
                // Replace with your actual data access
                SearchResults = GetMockData()
                    .Where(r => (string.IsNullOrEmpty(Input.CourseCode) || r.CourseCode.Contains(Input.CourseCode)))
                    .Where(r => (string.IsNullOrEmpty(Input.StaffOffice) || r.StaffOffice.Contains(Input.StaffOffice)))
                    .Where(r => (string.IsNullOrEmpty(Input.Building) || r.Building == Input.Building))
                    .Where(r => (string.IsNullOrEmpty(Input.Floor) || r.Floor == Input.Floor))
                    .Where(r => (string.IsNullOrEmpty(Input.Zone) || r.Zone == Input.Zone))
                    .Where(r => (string.IsNullOrEmpty(Input.RoomType) || r.RoomType == Input.RoomType))
                    .ToList();

                Searched = true;
            }
        }

        private bool AnyFilterApplied()
        {
            return !string.IsNullOrEmpty(Input.CourseCode) ||
                   !string.IsNullOrEmpty(Input.StaffOffice) ||
                   !string.IsNullOrEmpty(Input.Building) ||
                   !string.IsNullOrEmpty(Input.Floor) ||
                   !string.IsNullOrEmpty(Input.Zone) ||
                   !string.IsNullOrEmpty(Input.RoomType);
        }

        private List<RoomResult> GetMockData()
        {
            return new List<RoomResult>
            {
                new RoomResult { RoomNumber = "101", CourseCode = "SPC101", StaffOffice = "Dr. Ahmed", Building = "Nano", Floor = "Ground", Zone = "A", RoomType = "Lab", IsAvailable = true },
                new RoomResult { RoomNumber = "201", CourseCode = "REE101", StaffOffice = "TA. Amir", Building = "Helmy", Floor = "First", Zone = "B", RoomType = "Office", IsAvailable = false },
                new RoomResult { RoomNumber = "102", CourseCode = "CSE301", StaffOffice = "Prof. Samir", Building = "Nano", Floor = "Ground", Zone = "C", RoomType = "Lab", IsAvailable = true }
            };
        }

        public class SearchFilters
        {
            public string CourseCode { get; set; }
            public string StaffOffice { get; set; }
            public string Building { get; set; }
            public string Floor { get; set; }
            public string Zone { get; set; }
            public string RoomType { get; set; }
        }

        public class RoomResult
        {
            public string RoomNumber { get; set; }
            public string CourseCode { get; set; }
            public string StaffOffice { get; set; }
            public string Building { get; set; }
            public string Floor { get; set; }
            public string Zone { get; set; }
            public string RoomType { get; set; }
            public bool IsAvailable { get; set; }
        }
    }
}