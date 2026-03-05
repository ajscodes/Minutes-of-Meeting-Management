using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public IActionResult MeetingTypeList()
        {
            List<MeetingType> meetingTypeslist = GetMeetingTypes(null);
            return View(meetingTypeslist);
        }

        [HttpPost]
        public IActionResult MeetingTypeList(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<MeetingType> meetingTypeslist = GetMeetingTypes(searchText);
            return View(meetingTypeslist);
        }


        [HttpGet]
        public IActionResult MeetingTypeAddEdit(int? id)
        {
            if (id > 0)
            {
                MeetingType meetingType = GetMeetingTypeById(id.Value);
                return View(meetingType);
            }
            else
            {
                return View(new MeetingType());
            }
        }

        [HttpPost]
        public IActionResult MeetingTypeAddEdit(MeetingType model)
        {
            if (ModelState.IsValid)
            {
                AddEditMeetingType(model);
                TempData["Message"] = "Meeting type saved successfully!";
                return RedirectToAction("MeetingTypeList");
            }

            return View(model);
        }

        public List<MeetingType> GetMeetingTypes(string searchText)
        {
            List<MeetingType> meetingTypeslist = new List<MeetingType>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

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

            return meetingTypeslist;
        }

        public MeetingType GetMeetingTypeById(int id)
        {
            MeetingType meetingType = new MeetingType();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SelectByPK";
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter p = new SqlParameter();
            p.ParameterName = "@MeetingTypeID";
            p.SqlDbType = SqlDbType.Int;
            p.Value = id;

            cmd.Parameters.Add(p);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                meetingType.MeetingTypeID = Convert.ToInt32(reader["MeetingTypeID"]);
                meetingType.MeetingTypeName = reader["MeetingTypeName"].ToString() ?? string.Empty;
                meetingType.Remarks = reader["Remarks"]?.ToString();
                meetingType.Created = Convert.ToDateTime(reader["Created"]);
                meetingType.Modified = Convert.ToDateTime(reader["Modified"]);
            }

            reader.Close();
            con.Close();

            return meetingType;
        }

        public void AddEditMeetingType(MeetingType meetingType)
        {
            bool isEditing = false;

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (meetingType.MeetingTypeID > 0)
            {
                isEditing = true;
                cmd.CommandText = "PR_MOM_MeetingType_UpdateByPK";
            }
            else
            {
                cmd.CommandText = "PR_MOM_MeetingType_Insert";
            }

            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter pName = new SqlParameter();
            pName.ParameterName = "@MeetingTypeName";
            pName.SqlDbType = SqlDbType.VarChar;
            pName.Value = meetingType.MeetingTypeName;

            SqlParameter pRemarks = new SqlParameter();
            pRemarks.ParameterName = "@Remarks";
            pRemarks.SqlDbType = SqlDbType.VarChar;
            pRemarks.Value = (object?)meetingType.Remarks ?? DBNull.Value;

            SqlParameter pId = new SqlParameter();
            pId.ParameterName = "@MeetingTypeID";
            pId.SqlDbType = SqlDbType.Int;
            pId.Value = meetingType.MeetingTypeID;

            cmd.Parameters.Add(pName);
            cmd.Parameters.Add(pRemarks);

            if (isEditing)
            {
                cmd.Parameters.Add(pId);
            }

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public IActionResult MeetingTypeDelete(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_MeetingType_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@MeetingTypeID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Meeting type deleted successfully.";
                return RedirectToAction("MeetingTypeList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to delete meeting type due to related records.";
                return RedirectToAction("MeetingTypeList");
            }
        }

    }
}
