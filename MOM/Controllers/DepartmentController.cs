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
        public IActionResult DepartmentAddEdit(int? id)
        {
            if (id > 0)
            {
                Department department = GetDepartmentById(id.Value);
                return View(department);
            }
            else
            {
                var model = new Department();
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult DepartmentAddEdit(Department model)
        {
            if (ModelState.IsValid)
            {
                AddEditDepartment(model);
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

        public IActionResult DepartmentDelete(int id)
        {
            try
            {
                SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "PR_MOM_Department_DeleteByPK";
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter p = new SqlParameter();
                p.ParameterName = "@DepartmentID";
                p.SqlDbType = SqlDbType.Int;
                p.Value = id;

                cmd.Parameters.Add(p);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                TempData["Success"] = "Department deleted successfully.";
                return RedirectToAction("DepartmentList");
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to delete department due to related records.";
                return RedirectToAction("DepartmentList");
            }
        }

        public Department GetDepartmentById(int id)
        {
            Department department = new Department();

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "PR_MOM_Department_SelectByPK";
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter p = new SqlParameter();
            p.ParameterName = "@DepartmentID";
            p.SqlDbType = SqlDbType.Int;
            p.Value = id;

            cmd.Parameters.Add(p);

            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                department.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                department.DepartmentName = reader["DepartmentName"].ToString() ?? String.Empty;
                department.Created = Convert.ToDateTime(reader["Created"]);
                department.Modified = Convert.ToDateTime(reader["Modified"]);
            }

            reader.Close();
            con.Close();

            return department;
        }

        public void AddEditDepartment(Department department)
        {
            bool isEditing = false;

            SqlConnection con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            if (department.DepartmentID > 0)
            {
                isEditing = true;
                cmd.CommandText = "PR_MOM_Department_UpdateByPK";
            }
            else
            {
                cmd.CommandText = "PR_MOM_Department_Insert";
            }

            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter pName = new SqlParameter();
            pName.ParameterName = "@DepartmentName";
            pName.SqlDbType = SqlDbType.VarChar;
            pName.Value = department.DepartmentName;

            SqlParameter pId = new SqlParameter();
            pId.ParameterName = "@DepartmentID";
            pId.SqlDbType = SqlDbType.Int;
            pId.Value = department.DepartmentID;

            cmd.Parameters.Add(pName);

            if (isEditing)
            {
                cmd.Parameters.Add(pId);
            }

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }

    }
}
