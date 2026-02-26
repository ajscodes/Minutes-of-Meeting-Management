using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MOM.Models;

namespace MOM.Controllers
{
    public class StaffController : Controller
    {

        private IConfiguration _configuration;

        public StaffController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult StaffList(int departmentId)
        {
            List<Staff> list = new List<Staff>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Staff_SelectAll";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

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



            reader.Close();
            con.Close();

            return View(list);
        }

        [HttpGet]
        public IActionResult StaffAddEdit(int departmentId, int? id)
        {
            ViewBag.DepartmentName = "Customer Service";

            if (id == null)
            {
                var model = new Staff
                {
                    DepartmentID = departmentId
                };

                return View(model);
            }

            var staff = new Staff
            {
                StaffID = id.Value,
                DepartmentID = departmentId,
                StaffName = "John Smith",
                Mobile = "9876543210",
                Email = "john.smith@company.com",
                Remarks = "Manager"
            };

            return View(staff);
        }

        [HttpPost]
        public IActionResult StaffAddEdit(Staff model)
        {
            if (ModelState.IsValid)
            {
                // Save to DB logic here
                TempData["Message"] = "Staff member saved successfully!";
                return RedirectToAction("StaffList", new { departmentId = model.DepartmentID });
            }
            
            ViewBag.DepartmentName = "Customer Service"; // Re-populate ViewBags if needed
            return View(model);
        }
        public IActionResult StaffDetails(int id)
        {
            // Mock data for UI demonstration
            var model = new Staff
            {
                StaffID = id,
                DepartmentID = 1,
                StaffName = "John Smith",
                Mobile = "9876543210",
                Email = "john.smith@company.com",
                Remarks = "Senior Manager - Operations",
                Created = DateTime.Now.AddMonths(-6),
                Modified = DateTime.Now.AddDays(-2),
                Department = new Department
                {
                    DepartmentID = 1,
                    DepartmentName = "Operations"
                }
            };

            return View(model);
        }
    }
}
