using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MOM.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MOM.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();

            Dictionary<string, int> meetingTypeCounts = new Dictionary<string, int>();
            Dictionary<string, int> departmentCounts = new Dictionary<string, int>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Meetings_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            DateTime now = DateTime.Now;

            while (reader.Read())
            {
                model.TotalMeetings++;

                DateTime meetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                bool isCancelled = Convert.ToBoolean(reader["IsCancelled"]);

                if (isCancelled)
                {
                    model.CancelledMeetings++;
                }
                else if (meetingDate >= now)
                {
                    model.UpcomingMeetings++;
                }
                else
                {
                    model.CompletedMeetings++;
                }

                string meetingTypeName = reader["MeetingTypeName"]?.ToString() ?? "Unknown";
                if (!meetingTypeCounts.ContainsKey(meetingTypeName))
                {
                    meetingTypeCounts.Add(meetingTypeName, 0);
                }
                meetingTypeCounts[meetingTypeName]++;

                string departmentName = reader["DepartmentName"]?.ToString() ?? "Unknown";
                if (!departmentCounts.ContainsKey(departmentName))
                {
                    departmentCounts.Add(departmentName, 0);
                }
                departmentCounts[departmentName]++;
            }

            reader.Close();
            con.Close();

            model.MeetingsByTypeLabels = meetingTypeCounts.Keys.ToList();
            model.MeetingsByTypeSeries = meetingTypeCounts.Values.ToList();

            model.MeetingsByDepartmentLabels = departmentCounts.Keys.ToList();
            model.MeetingsByDepartmentSeries = departmentCounts.Values.ToList();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
