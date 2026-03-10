using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingController : Controller
    {
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public MeetingController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public IActionResult MeetingList()
        {
            List<Meeting> MeetingList = GetMeetings(null);
            return View(MeetingList);
        }

        [HttpPost]
        public IActionResult MeetingList(IFormCollection formData)
        {
            string? searchText = formData["SearchText"].ToString();

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;

            List<Meeting> MeetingList = GetMeetings(searchText);
            return View(MeetingList);
        }

        private List<Meeting> GetMeetings(string? searchText)
        {
            List<Meeting> MeetingList = new List<Meeting>();

            SqlConnection con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection")
            ); 

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Meetings_SelectAll";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

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

            return MeetingList;
        }


        [HttpGet]
        public IActionResult MeetingDetail(int id)
        {
            Meeting meeting = new Meeting();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = @"SELECT m.*, 
                                        mt.MeetingTypeName, 
                                        d.DepartmentName, 
                                        mv.MeetingVenueName
                                 FROM MOM_Meetings m
                                 LEFT JOIN MOM_MeetingType mt ON m.MeetingTypeID = mt.MeetingTypeID
                                 LEFT JOIN MOM_Department d ON m.DepartmentID = d.DepartmentID
                                 LEFT JOIN MOM_MeetingVenue mv ON m.MeetingVenueID = mv.MeetingVenueID
                                 WHERE m.MeetingID = @MeetingID";

                SqlCommand cmd = new SqlCommand(query, con);
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

                    if (reader["CancellationDateTime"] != DBNull.Value)
                        meeting.CancellationDateTime = Convert.ToDateTime(reader["CancellationDateTime"]);

                    if (reader["CancellationReason"] != DBNull.Value)
                        meeting.CancellationReason = reader["CancellationReason"]?.ToString();

                    if (reader["Created"] != DBNull.Value)
                        meeting.Created = Convert.ToDateTime(reader["Created"]);

                    if (reader["Modified"] != DBNull.Value)
                        meeting.Modified = Convert.ToDateTime(reader["Modified"]);

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
                }

                reader.Close();
                con.Close();
            }

            return View(meeting);
        }


        [HttpGet]
        public IActionResult MeetingAddEdit(int? id)
        {
            LoadDropdowns();

            if (id == null)
                return View(new Meeting { MeetingDate = DateTime.Now });

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
            {
                LoadDropdowns();
                return View(meeting);
            }

            if (meeting.DocumentFile != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(meeting.DocumentFile.FileName);
                string fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    meeting.DocumentFile.CopyTo(stream);
                }

                meeting.DocumentPath = "/uploads/" + fileName;
            }

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

        [HttpGet]
        public IActionResult MeetingAttendance(int id)
        {
            ViewBag.MeetingID = id;

            List<Staff> staffList = new List<Staff>();
            List<MeetingMember> existingMembers = new List<MeetingMember>();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand cmdStaff = new SqlCommand("PR_MOM_Staff_SelectAll", con);
                cmdStaff.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataReader reader = cmdStaff.ExecuteReader();
                while (reader.Read())
                {
                    Staff staff = new Staff();
                    staff.StaffID = Convert.ToInt32(reader["StaffID"]);
                    staff.StaffName = reader["StaffName"]?.ToString() ?? string.Empty;
                    staffList.Add(staff);
                }
                reader.Close();

                SqlCommand cmdMember = new SqlCommand("PR_MOM_MeetingMember_SelectAll", con);
                cmdMember.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader2 = cmdMember.ExecuteReader();
                while (reader2.Read())
                {
                    if (Convert.ToInt32(reader2["MeetingID"]) == id)
                    {
                        MeetingMember member = new MeetingMember();
                        member.MeetingMemberID = Convert.ToInt32(reader2["MeetingMemberID"]);
                        member.MeetingID = Convert.ToInt32(reader2["MeetingID"]);
                        member.StaffID = Convert.ToInt32(reader2["StaffID"]);
                        member.IsPresent = Convert.ToBoolean(reader2["IsPresent"]);
                        existingMembers.Add(member);
                    }
                }
                reader2.Close();
                con.Close();
            }

            List<MeetingMember> model = new List<MeetingMember>();
            foreach (var staff in staffList)
            {
                var existing = existingMembers.FirstOrDefault(m => m.StaffID == staff.StaffID);
                if (existing != null)
                {
                    existing.Staff = staff;
                    model.Add(existing);
                }
                else
                {
                    model.Add(new MeetingMember
                    {
                        MeetingMemberID = 0,
                        MeetingID = id,
                        StaffID = staff.StaffID,
                        IsPresent = true,
                        Staff = staff
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult MeetingAttendance(List<MeetingMember> members)
        {
            if (members == null || !members.Any())
            {
                return RedirectToAction("MeetingList");
            }

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                foreach (var item in members)
                {
                    SqlCommand cmd;
                    if (item.MeetingMemberID == 0)
                    {
                        cmd = new SqlCommand("PR_MOM_MeetingMember_Insert", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MeetingID", item.MeetingID);
                        cmd.Parameters.AddWithValue("@StaffID", item.StaffID);
                        cmd.Parameters.AddWithValue("@IsPresent", item.IsPresent);
                        cmd.Parameters.AddWithValue("@Remarks", DBNull.Value);
                    }
                    else
                    {
                        cmd = new SqlCommand("PR_MOM_MeetingMember_UpdateByPK", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MeetingMemberID", item.MeetingMemberID);
                        cmd.Parameters.AddWithValue("@MeetingID", item.MeetingID);
                        cmd.Parameters.AddWithValue("@StaffID", item.StaffID);
                        cmd.Parameters.AddWithValue("@IsPresent", item.IsPresent);
                        cmd.Parameters.AddWithValue("@Remarks", DBNull.Value);
                    }
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }

            TempData["Message"] = "Attendance saved successfully!";
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

        private void LoadDropdowns()
        {
            LoadMeetingTypesDropdown();
            LoadDepartmentsDropdown();
            LoadMeetingVenuesDropdown();
        }

        private void LoadMeetingTypesDropdown()
        {
            List<SelectListItem> meetingTypes = new List<SelectListItem>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingType_SelectAll";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                SelectListItem item = new SelectListItem();
                item.Value = reader["MeetingTypeID"].ToString();
                item.Text = reader["MeetingTypeName"].ToString();
                meetingTypes.Add(item);
            }

            reader.Close();
            con.Close();

            ViewBag.MeetingTypes = meetingTypes;
        }

        private void LoadDepartmentsDropdown()
        {
            List<SelectListItem> departments = new List<SelectListItem>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectAll";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                SelectListItem item = new SelectListItem();
                item.Value = reader["DepartmentID"].ToString();
                item.Text = reader["DepartmentName"].ToString();
                departments.Add(item);
            }

            reader.Close();
            con.Close();

            ViewBag.Departments = departments;
        }

        private void LoadMeetingVenuesDropdown()
        {
            List<SelectListItem> meetingVenues = new List<SelectListItem>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_MeetingVenue_SelectAll";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                SelectListItem item = new SelectListItem();
                item.Value = reader["MeetingVenueID"].ToString();
                item.Text = reader["MeetingVenueName"].ToString();
                meetingVenues.Add(item);
            }

            reader.Close();
            con.Close();

            ViewBag.MeetingVenues = meetingVenues;
        }

    }
}
