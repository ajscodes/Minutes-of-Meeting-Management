using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MOM.Models;
using System.Data;

namespace MOM.Controllers
{
    public class DepartmentController : Controller
    {

        private IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult DepartmentList()
        {
            List<Department> DepartmentList = new List<Department>();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectAll";
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Department d = new Department();

                d.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                d.DepartmentName = reader["DepartmentName"].ToString() ?? String.Empty;
                d.Created = Convert.ToDateTime(reader["Created"]);
                d.Modified = Convert.ToDateTime(reader["Modified"]);

                DepartmentList.Add(d);
            }

            reader.Close();
            con.Close();

            return View(DepartmentList);
        }

    

        [HttpGet]
        public IActionResult DepartmentAddEdit()
        {
            var model = new Department();
            return View(model);
        }

        [HttpPost]
        public IActionResult DepartmentAddEdit(Department model)
        {
            if (ModelState.IsValid)
            {
                // Save logic here
                TempData["Message"] = "Department saved successfully!";
                return RedirectToAction("DepartmentList");
            }
            return View(model);
        }

        public IActionResult DepartmentDetails(int id)
        {
            var model = new Department
            {
                DepartmentID = id,
                DepartmentName = "Customer Service",
                Created = DateTime.Now.AddDays(-10),
                Modified = DateTime.Now
            };

            return View(model);
        }

    }
}
