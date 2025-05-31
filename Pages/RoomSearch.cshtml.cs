using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Project.Models;
using System.Data;

namespace Project.Pages
{
    [BindProperties(SupportsGet = true)]
    public class RoomSearchModel : PageModel
    {
        public string SearchType { get; set; }
        public string CourseCode { get; set; }
        public string StaffName { get; set; }
        public string RoomType { get; set; }
        public string Building { get; set; }
        public int? Floor { get; set; }
        public string Zone { get; set; }
        public DB db { get; set; }
        public RoomSearchModel(DB db)
        {
            this.db = db;
        }
        public DataTable DT { get; set; } = new DataTable();
        public bool HasSearched { get; set; }

        public void OnGet()
        {
            if (!string.IsNullOrEmpty(SearchType))
            {
                HasSearched = true;

                switch (SearchType)
                {
                    case "ByCourse":
                        DT = db.RoomSearchByCourse(CourseCode ?? "", RoomType ?? "");
                        break;

                    case "ByStaff":
                        DT = db.RoomSearchForOffice(StaffName ?? "");
                        break;

                    case "ByLocation":
                        DT = db.RoomSearchByLocation(Building ?? "", Floor , Zone ?? "");
                        break;

                    default:
                        DT = db.RetrieveAllRooms();
                        break;
                }
            }
            else
            {
                DT = db.RetrieveAllRooms();
            }
        }
    }
}