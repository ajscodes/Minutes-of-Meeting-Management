using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Collections.Generic;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {

        private IConfiguration _configuration;

        public MeetingTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public IActionResult MeetingTypeList()
        {
            List<MeetingType> meetingTypeslist = new List<MeetingType>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingType m = new MeetingType();

                m.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                m.MeetingTypeName = reader["MeetingTypeName"].ToString() ?? string.Empty;
                m.Remarks = reader["Remarks"]?.ToString();
                m.Created = Convert.ToDateTime(reader["Created"]);
                m.Modified = Convert.ToDateTime(reader["Modified"]);

                meetingTypeslist.Add(m);
            }


            reader.Close();
            con.Close();

            return View(meetingTypeslist);
        }


        [HttpGet]
        public IActionResult MeetingTypeAddEdit(int? id)
        {
            if (id == null)
            {
                // Add mode
                return View(new MeetingType());
            }

            // Edit mode - Fetch all and find the specific one
            // Ideally we should use a SelectByPK stored procedure, but reusing SelectAll for safety
            List<MeetingType> meetingTypeslist = new List<MeetingType>();

            SqlConnection con = new SqlConnection("Server=AYUSH\\SQLEXPRESS;Database=MOM_DB;Trusted_Connection=True;TrustServerCertificate=True;");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                MeetingType m = new MeetingType();

                m.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                m.MeetingTypeName = reader["MeetingTypeName"].ToString() ?? string.Empty;
                m.Remarks = reader["Remarks"]?.ToString();
                m.Created = Convert.ToDateTime(reader["Created"]);
                m.Modified = Convert.ToDateTime(reader["Modified"]);

                meetingTypeslist.Add(m);
            }

            reader.Close();
            con.Close();

            var model = meetingTypeslist.FirstOrDefault(m => m.MeetingTypeID == id);

            if (model == null)
            {
                return RedirectToAction("MeetingTypeList");
            }

            return View(model);
        }

    }
}
