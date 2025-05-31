using ChartExample.Models.Chart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Project.Models;
namespace Project.Pages.Admin
{
    public class StatsModel : PageModel
    {
        public DB db;
        public int ProfessorsCount { get; private set; }
        public int TAsCount { get; private set; }
        public int StudentsCount { get; private set; }
        public int HandledBookingRequests { get; private set; }
        public int HandledReports { get; private set; }

        public ChartJs UserDistributionChart { get; set; }
        public string UserDistributionChartJson { get; set; }

        public ChartJs RequestStatusChart { get; set; }
        public string RequestStatusChartJson { get; set; }

        public StatsModel(DB database)
        {
            db = database;
            UserDistributionChart = new ChartJs();
            RequestStatusChart = new ChartJs();
        }
        public IActionResult OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToPage("/Login");
            }
            else if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToPage("/Home");
            }

            var (profs, tas, students, handledBookings, handledReports) = db.Admin_stats();
            ProfessorsCount = profs;
            TAsCount = tas;
            StudentsCount = students;
            HandledBookingRequests = handledBookings;
            HandledReports = handledReports;

            // Fetch all data needed for charts
            Dictionary<string, int> chartData = db.getFavouriteCodeEditors(); // This method returns all needed counts

            SetUpUserDistributionChart(chartData);
            SetUpRequestStatusChart(chartData);

            return Page();
        }

        private void SetUpUserDistributionChart(Dictionary<string, int> allData)
        {
            try
            {
                UserDistributionChart.type = "doughnut";
                UserDistributionChart.options.responsive = true;

                var labelsArray = new List<string> { "Professors", "TAs", "Students" };
                var dataArray = new List<double>
                {
                    allData.ContainsKey("Professors") ? allData["Professors"] : 0,
                    allData.ContainsKey("TAs") ? allData["TAs"] : 0,
                    allData.ContainsKey("Students") ? allData["Students"] : 0
                };

                UserDistributionChart.data.labels = labelsArray;

                var dataset = new Dataset
                {
                    label = "User Distribution",
                    data = dataArray.ToArray(),
                    // You can add specific colors here if needed
                    // backgroundColor = new List<string> { "#FF6384", "#36A2EB", "#FFCE56" } 
                };
                UserDistributionChart.data.datasets.Add(dataset);

                UserDistributionChartJson = JsonConvert.SerializeObject(UserDistributionChart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initialising the user distribution chart: " + e.Message);
                // Consider logging the full exception e
            }
        }

        private void SetUpRequestStatusChart(Dictionary<string, int> allData)
        {
            try
            {
                RequestStatusChart.type = "doughnut";
                RequestStatusChart.options.responsive = true;

                var labelsArray = new List<string> { "Handled Booking Requests", "Handled Reports" };
                var dataArray = new List<double>
                {
                    allData.ContainsKey("HandledBookingRequests") ? allData["HandledBookingRequests"] : 0,
                    allData.ContainsKey("HandledReports") ? allData["HandledReports"] : 0
                };

                RequestStatusChart.data.labels = labelsArray;

                var dataset = new Dataset
                {
                    label = "Request Statuses",
                    data = dataArray.ToArray(),
                    // You can add specific colors here if needed
                    // backgroundColor = new List<string> { "#4BC0C0", "#E7E9ED" }
                };
                RequestStatusChart.data.datasets.Add(dataset);

                RequestStatusChartJson = JsonConvert.SerializeObject(RequestStatusChart, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initialising the request status chart: " + e.Message);
                // Consider logging the full exception e
            }
        }
    }
}