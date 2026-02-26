using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingController : Controller
    {
        private IConfiguration _configuration;

        public MeetingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult MeetingList()
        {
            List<Meeting> MeetingList = new List<Meeting>();

            SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            ); 

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Meetings_SelectAll";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Meeting meeting = new Meeting();

                meeting.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                meeting.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                meeting.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                meeting.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                meeting.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                meeting.MeetingDescription = reader["MeetingDescription"]?.ToString();
                meeting.DocumentPath = reader["DocumentPath"]?.ToString();
                meeting.IsCancelled = Convert.ToBoolean(reader["IsCancelled"]);

                meeting.MeetingType = new MeetingType()
                {
                    MeetingTypeID = meeting.MeetingTypeID,
                    MeetingTypeName = reader["MeetingTypeName"]?.ToString() ?? string.Empty
                };

                meeting.Department = new Department()
                {
                    DepartmentID = meeting.DepartmentID,
                    DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty
                };

                meeting.MeetingVenue = new MeetingVenue()
                {
                    MeetingVenueID = meeting.MeetingVenueID,
                    MeetingVenueName = reader["MeetingVenueName"]?.ToString() ?? string.Empty
                };

                MeetingList.Add(meeting);
            }

            reader.Close();
            con.Close();

            return View(MeetingList);
        }


        [HttpGet]
        public IActionResult MeetingAddEdit(int? id)
        {
            if (id == null)
                return View(new Meeting());

            Meeting meeting = new Meeting();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("PR_MOM_Meetings_SelectByPK", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingID", id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    meeting.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                    meeting.MeetingDate = Convert.ToDateTime(reader["MeetingDate"]);
                    meeting.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                    meeting.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                    meeting.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                    meeting.MeetingDescription = reader["MeetingDescription"]?.ToString();
                    meeting.DocumentPath = reader["DocumentPath"]?.ToString();
                    meeting.IsCancelled = Convert.ToBoolean(reader["IsCancelled"]);
                }

                reader.Close();
                con.Close();
            }

            return View(meeting);
        }


        [HttpPost]
        public IActionResult MeetingAddEdit(Meeting meeting)
        {
            if (!ModelState.IsValid)
                return View(meeting);

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd;

                if (meeting.MeetingID == 0)
                {
                    cmd = new SqlCommand("PR_MOM_Meetings_Insert", con);
                }
                else
                {
                    cmd = new SqlCommand("PR_MOM_Meetings_UpdateByPK", con);
                    cmd.Parameters.AddWithValue("@MeetingID", meeting.MeetingID);
                }

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MeetingDate", meeting.MeetingDate);
                cmd.Parameters.AddWithValue("@MeetingTypeID", meeting.MeetingTypeID);
                cmd.Parameters.AddWithValue("@DepartmentID", meeting.DepartmentID);
                cmd.Parameters.AddWithValue("@MeetingVenueID", meeting.MeetingVenueID);
                cmd.Parameters.AddWithValue("@MeetingDescription", (object?)meeting.MeetingDescription ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DocumentPath", (object?)meeting.DocumentPath ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            TempData["Message"] = "Meeting saved successfully!";
            return RedirectToAction("MeetingList");
        }


        public IActionResult CancelMeeting(int id)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE MOM_Meetings SET IsCancelled = 1, CancellationDateTime = GETDATE(), Modified = GETDATE() WHERE MeetingID = @MeetingID",
                    con);

                cmd.Parameters.AddWithValue("@MeetingID", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            return RedirectToAction("MeetingList");
        }


        public IActionResult DeleteMeeting(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection(
                    "Server=AYUSH\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;"
                );

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Meetings_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MeetingID", id);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToAction("MeetingList");
            }
            catch (SqlException ex)
            {
                return Content(ex.Message);
            }
        }

    }
}
