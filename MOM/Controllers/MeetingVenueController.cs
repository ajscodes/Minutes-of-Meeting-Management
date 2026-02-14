using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        public IActionResult MeetingVenueList()
        {
            List<MeetingVenue> meetingVenueList = new List<MeetingVenue>();

            SqlConnection con = new SqlConnection("Server=AYUSH\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_SelectAll";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingVenue v = new MeetingVenue();

                v.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                v.MeetingVenueName = reader["MeetingVenueName"].ToString() ?? string.Empty;
                v.Created = Convert.ToDateTime(reader["Created"]);
                v.Modified = Convert.ToDateTime(reader["Modified"]);

                meetingVenueList.Add(v);
            }


            reader.Close();
            con.Close();

            return View(meetingVenueList);
        }

        public IActionResult MeetingVenueAddEdit(int? id)
        {
            MeetingVenue model;

            if (id.HasValue)
            {
                model = new MeetingVenue
                {
                    MeetingVenueID = id.Value,
                    MeetingVenueName = "Conference Room A",
                    Created = DateTime.Now.AddDays(-5),
                    Modified = DateTime.Now
                };
            }
            else
            {
                model = new MeetingVenue();
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult MeetingVenueAddEdit(MeetingVenue model)
        {
            if (ModelState.IsValid)
            {
                // Save logic here
                TempData["Message"] = "Venue saved successfully!";
                return RedirectToAction("MeetingVenueList");
            }
            return View(model);
        }
    }
}
