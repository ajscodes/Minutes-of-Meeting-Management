using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class AttendenceController : Controller
    {
        private readonly IConfiguration _configuration;

        public AttendenceController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult AttendenceList()
        {
            List<MeetingMember> list = GetMeetingMembers();
            return View(list);
        }

        private List<MeetingMember> GetMeetingMembers()
        {
            List<MeetingMember> list = new List<MeetingMember>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("PR_MOM_MeetingMember_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    MeetingMember member = new MeetingMember();
                    member.MeetingMemberID = Convert.ToInt32(reader["MeetingMemberID"]);
                    member.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                    member.StaffID = Convert.ToInt32(reader["StaffID"]);
                    member.IsPresent = Convert.ToBoolean(reader["IsPresent"]);
                    member.Remarks = reader["Remarks"]?.ToString();
                    member.Created = Convert.ToDateTime(reader["Created"]);
                    member.Modified = Convert.ToDateTime(reader["Modified"]);

                    member.Staff = new Staff
                    {
                        StaffID = member.StaffID,
                        StaffName = reader["StaffName"]?.ToString() ?? string.Empty
                    };

                    member.Meeting = new Meeting
                    {
                        MeetingID = member.MeetingID,
                        MeetingDate = Convert.ToDateTime(reader["MeetingDate"])
                    };

                    list.Add(member);
                }
                reader.Close();
            }

            return list;
        }

        [HttpGet]
        public IActionResult AttendenceAddEdit(int? id)
        {
            LoadDropdowns();

            if (id == null)
                return View(new MeetingMember());

            MeetingMember member = new MeetingMember();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("PR_MOM_MeetingMember_SelectByPK", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingMemberID", id);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    member.MeetingMemberID = Convert.ToInt32(reader["MeetingMemberID"]);
                    member.MeetingID = Convert.ToInt32(reader["MeetingID"]);
                    member.StaffID = Convert.ToInt32(reader["StaffID"]);
                    member.IsPresent = Convert.ToBoolean(reader["IsPresent"]);
                    member.Remarks = reader["Remarks"]?.ToString();
                }

                reader.Close();
            }

            return View(member);
        }

        [HttpPost]
        public IActionResult AttendenceAddEdit(MeetingMember member)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(member);
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd;

                if (member.MeetingMemberID == 0)
                {
                    cmd = new SqlCommand("PR_MOM_MeetingMember_Insert", con);
                }
                else
                {
                    cmd = new SqlCommand("PR_MOM_MeetingMember_UpdateByPK", con);
                    cmd.Parameters.AddWithValue("@MeetingMemberID", member.MeetingMemberID);
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MeetingID", member.MeetingID);
                cmd.Parameters.AddWithValue("@StaffID", member.StaffID);
                cmd.Parameters.AddWithValue("@IsPresent", member.IsPresent);
                cmd.Parameters.AddWithValue("@Remarks", (object?)member.Remarks ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            TempData["Message"] = "Attendance record saved successfully!";
            return RedirectToAction("AttendenceList");
        }

        public IActionResult DeleteAttendence(int id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand cmd = new SqlCommand("PR_MOM_MeetingMember_DeleteByPK", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MeetingMemberID", id);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                return RedirectToAction("AttendenceList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Cannot delete because record is referenced elsewhere or an error occurred.";
                return RedirectToAction("AttendanceList");
            }
        }

        private void LoadDropdowns()
        {
            LoadMeetingsDropdown();
            LoadStaffDropdown();
        }

        private void LoadMeetingsDropdown()
        {
            List<SelectListItem> lists = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("PR_MOM_Meetings_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime md = Convert.ToDateTime(reader["MeetingDate"]);
                    string desc = reader["MeetingDescription"]?.ToString() ?? string.Empty;
                    string text = $"{md.ToString("dd-MMM-yyyy")} - {desc}";
                    lists.Add(new SelectListItem
                    {
                        Value = reader["MeetingID"].ToString(),
                        Text = text
                    });
                }
                reader.Close();
            }
            ViewBag.Meetings = lists;
        }

        private void LoadStaffDropdown()
        {
            List<SelectListItem> lists = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmd = new SqlCommand("PR_MOM_Staff_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lists.Add(new SelectListItem
                    {
                        Value = reader["StaffID"].ToString(),
                        Text = reader["StaffName"].ToString()
                    });
                }
                reader.Close();
            }
            ViewBag.Staffs = lists;
        }
    }
}
