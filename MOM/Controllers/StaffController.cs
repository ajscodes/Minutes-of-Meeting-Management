using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using MOM.Models;
using System.Data;
using System.Linq;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {

        private IConfiguration _configuration;

        public StaffController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult StaffList(int departmentId = 0)
        {
            ViewBag.DepartmentId = departmentId;
            LoadDepartmentsDropdown();
            List<Staff> list = GetStaff(null, departmentId);
            return View(list);
        }

        [HttpPost]
        public IActionResult StaffList(IFormCollection formData)
        {
            string searchText = formData["SearchText"].ToString();
            int departmentId = 0;
            if (int.TryParse(formData["departmentId"], out int parsedId))
            {
                departmentId = parsedId;
            }

            if (string.IsNullOrWhiteSpace(searchText))
                searchText = null;

            ViewBag.SearchText = searchText;
            ViewBag.DepartmentId = departmentId;
            LoadDepartmentsDropdown();

            List<Staff> list = GetStaff(searchText, departmentId);
            return View(list);
        }

        public List<Staff> GetStaff(string searchText, int departmentId)
        {
            List<Staff> list = new List<Staff>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Staff_SelectAll";
            cmd.CommandType = CommandType.StoredProcedure;

            if (searchText != null)
                cmd.Parameters.AddWithValue("@SearchText", searchText);
            else
                cmd.Parameters.AddWithValue("@SearchText", DBNull.Value);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Staff s = new Staff();

                s.StaffID = Convert.ToInt32(reader["StaffID"]);
                s.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                s.StaffName = reader["StaffName"].ToString() ?? string.Empty;

                s.Mobile = reader["Mobile"] == DBNull.Value
                                ? null
                                : reader["Mobile"].ToString();

                s.Email = reader["Email"] == DBNull.Value
                                ? null
                                : reader["Email"].ToString();

                s.Remarks = reader["Remarks"] == DBNull.Value
                                ? null
                                : reader["Remarks"].ToString();

                s.Created = Convert.ToDateTime(reader["Created"]);
                s.Modified = Convert.ToDateTime(reader["Modified"]);

                s.Department = new Department
                {
                    DepartmentID = s.DepartmentID,
                    DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty
                };

                list.Add(s);
            }

            if(departmentId > 0)
            {
                list = list.Where(s => s.DepartmentID == departmentId).ToList();
            }

            reader.Close();
            con.Close();

            return list;
        }

        [HttpGet]
        public IActionResult StaffAddEdit(int departmentId, int? id)
        {
            ViewBag.DepartmentName = "Department";

            if (id > 0)
            {
                Staff staff = GetStaffById(id.Value);
                LoadDepartmentsDropdown();
                return View(staff);
            }
            else
            {
                var model = new Staff
                {
                    DepartmentID = departmentId
                };

                LoadDepartmentsDropdown();
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult StaffAddEdit(Staff model)
        {
            if (ModelState.IsValid)
            {
                AddEditStaff(model);
                TempData["Message"] = "Staff member saved successfully!";
                return RedirectToAction("StaffList", new { departmentId = model.DepartmentID });
            }
            
            ViewBag.DepartmentName = "Department";
            LoadDepartmentsDropdown();
            return View(model);
        }

        public void LoadDepartmentsDropdown()
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

        public Staff GetStaffById(int id)
        {
            Staff staff = new Staff();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Staff_SelectByPK";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter p = new SqlParameter();
            p.ParameterName = "@StaffID";
            p.SqlDbType = System.Data.SqlDbType.Int;
            p.Value = id;

            cmd.Parameters.Add(p);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                staff.StaffID = Convert.ToInt32(reader["StaffID"]);
                staff.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                staff.StaffName = reader["StaffName"].ToString() ?? string.Empty;

                staff.Mobile = reader["Mobile"] == DBNull.Value
                                    ? null
                                    : reader["Mobile"].ToString();

                staff.Email = reader["Email"] == DBNull.Value
                                    ? null
                                    : reader["Email"].ToString();

                staff.Remarks = reader["Remarks"] == DBNull.Value
                                    ? null
                                    : reader["Remarks"].ToString();

                staff.Created = Convert.ToDateTime(reader["Created"]);
                staff.Modified = Convert.ToDateTime(reader["Modified"]);

                string departmentName = string.Empty;
                try
                {
                    departmentName = reader["DepartmentName"]?.ToString() ?? string.Empty;
                }
                catch (Exception)
                {
                    departmentName = string.Empty;
                }

                if (string.IsNullOrEmpty(departmentName) && staff.DepartmentID > 0)
                {
                    departmentName = GetDepartmentNameById(staff.DepartmentID);
                }

                staff.Department = new Department
                {
                    DepartmentID = staff.DepartmentID,
                    DepartmentName = departmentName
                };
            }

            reader.Close();
            con.Close();

            return staff;
        }

        public string GetDepartmentNameById(int id)
        {
            string departmentName = string.Empty;

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectByPK";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter p = new SqlParameter();
            p.ParameterName = "@DepartmentID";
            p.SqlDbType = System.Data.SqlDbType.Int;
            p.Value = id;

            cmd.Parameters.Add(p);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                departmentName = reader["DepartmentName"]?.ToString() ?? string.Empty;
            }

            reader.Close();
            con.Close();

            return departmentName;
        }

        public void AddEditStaff(Staff staff)
        {
            bool isEditing = false;

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (staff.StaffID > 0)
            {
                isEditing = true;
                cmd.CommandText = "PR_MOM_Staff_UpdateByPK";
            }
            else
            {
                cmd.CommandText = "PR_MOM_Staff_Insert";
            }

            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter pDepartmentId = new SqlParameter();
            pDepartmentId.ParameterName = "@DepartmentID";
            pDepartmentId.SqlDbType = System.Data.SqlDbType.Int;
            pDepartmentId.Value = staff.DepartmentID;

            SqlParameter pName = new SqlParameter();
            pName.ParameterName = "@StaffName";
            pName.SqlDbType = System.Data.SqlDbType.VarChar;
            pName.Value = staff.StaffName;

            SqlParameter pMobile = new SqlParameter();
            pMobile.ParameterName = "@Mobile";
            pMobile.SqlDbType = System.Data.SqlDbType.VarChar;
            pMobile.Value = (object?)staff.Mobile ?? DBNull.Value;

            SqlParameter pEmail = new SqlParameter();
            pEmail.ParameterName = "@Email";
            pEmail.SqlDbType = System.Data.SqlDbType.VarChar;
            pEmail.Value = (object?)staff.Email ?? DBNull.Value;

            SqlParameter pRemarks = new SqlParameter();
            pRemarks.ParameterName = "@Remarks";
            pRemarks.SqlDbType = System.Data.SqlDbType.VarChar;
            pRemarks.Value = (object?)staff.Remarks ?? DBNull.Value;

            SqlParameter pId = new SqlParameter();
            pId.ParameterName = "@StaffID";
            pId.SqlDbType = System.Data.SqlDbType.Int;
            pId.Value = staff.StaffID;

            cmd.Parameters.Add(pDepartmentId);
            cmd.Parameters.Add(pName);
            cmd.Parameters.Add(pMobile);
            cmd.Parameters.Add(pEmail);
            cmd.Parameters.Add(pRemarks);

            if (isEditing)
            {
                cmd.Parameters.Add(pId);
            }

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public IActionResult StaffDelete(int id, int departmentId)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Staff_DeleteByPK";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@StaffID";
                p.SqlDbType = System.Data.SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Staff member deleted successfully.";
                return RedirectToAction("StaffList", new { departmentId = departmentId });
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to delete staff member due to related records.";
                return RedirectToAction("StaffList", new { departmentId = departmentId });
            }
        }

        public IActionResult StaffDetails(int id)
        {
            Staff model = GetStaffById(id);
            return View(model);
        }
    }
}
