using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Collections.Generic;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        public IActionResult MeetingTypeList()
        {
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

            return View(meetingTypeslist);
        }



    }
}
