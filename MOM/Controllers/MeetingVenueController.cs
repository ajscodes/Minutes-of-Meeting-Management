using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        private IConfiguration _configuration;

        public MeetingVenueController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult MeetingVenueList()
        {
            List<MeetingVenue> meetingVenueList = new List<MeetingVenue>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

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

        [HttpGet]
        public IActionResult MeetingVenueAddEdit(int? id)
        {
            if (id > 0)
            {
                MeetingVenue meetingVenue = GetMeetingVenueById(id.Value);
                return View(meetingVenue);
            }
            else
            {
                return View(new MeetingVenue());
            }
        }

        [HttpPost]
        public IActionResult MeetingVenueAddEdit(MeetingVenue model)
        {
            if (ModelState.IsValid)
            {
                AddEditMeetingVenue(model);
                TempData["Message"] = "Venue saved successfully!";
                return RedirectToAction("MeetingVenueList");
            }
            return View(model);
        }

        public MeetingVenue GetMeetingVenueById(int id)
        {
            MeetingVenue meetingVenue = new MeetingVenue();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_SelectByPK";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter p = new SqlParameter();
            p.ParameterName = "@MeetingVenueID";
            p.SqlDbType = System.Data.SqlDbType.Int;
            p.Value = id;

            cmd.Parameters.Add(p);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                meetingVenue.MeetingVenueID = Convert.ToInt32(reader["MeetingVenueID"]);
                meetingVenue.MeetingVenueName = reader["MeetingVenueName"].ToString() ?? string.Empty;
                meetingVenue.Created = Convert.ToDateTime(reader["Created"]);
                meetingVenue.Modified = Convert.ToDateTime(reader["Modified"]);
            }

            reader.Close();
            con.Close();

            return meetingVenue;
        }

        public void AddEditMeetingVenue(MeetingVenue meetingVenue)
        {
            bool isEditing = false;

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (meetingVenue.MeetingVenueID > 0)
            {
                isEditing = true;
                cmd.CommandText = "PR_MOM_MeetingVenue_UpdateByPK";
            }
            else
            {
                cmd.CommandText = "PR_MOM_MeetingVenue_Insert";
            }

            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter pName = new SqlParameter();
            pName.ParameterName = "@MeetingVenueName";
            pName.SqlDbType = System.Data.SqlDbType.VarChar;
            pName.Value = meetingVenue.MeetingVenueName;

            SqlParameter pId = new SqlParameter();
            pId.ParameterName = "@MeetingVenueID";
            pId.SqlDbType = System.Data.SqlDbType.Int;
            pId.Value = meetingVenue.MeetingVenueID;

            cmd.Parameters.Add(pName);

            if (isEditing)
            {
                cmd.Parameters.Add(pId);
            }

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public IActionResult MeetingVenueDelete(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingVenue_DeleteByPK";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingVenueID";
                p.SqlDbType = System.Data.SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Venue deleted successfully.";
                return RedirectToAction("MeetingVenueList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to delete venue due to related records.";
                return RedirectToAction("MeetingVenueList");
            }
        }

    }
}
