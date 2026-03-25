using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using MOM.Models;

namespace MOM.Controllers
{
    public class AdminAccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AdminAccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Register Page
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(AdminUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string connStr = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand checkCmd = conn.CreateCommand())
                    {
                        checkCmd.CommandType = CommandType.StoredProcedure;
                        checkCmd.CommandText = "PR_AdminUser_SelectByEmail";
                        checkCmd.Parameters.AddWithValue("@Email", model.Email);

                        using (SqlDataReader dr = checkCmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                TempData["ErrorMessage"] = "Email already exists.";
                                return View(model);
                            }
                        }
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PR_AdminUser_Register";

                        cmd.Parameters.AddWithValue("@FullName", model.FullName);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Password", model.Password);

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "Registration completed successfully.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error during registration: " + ex.Message;
                return View(model);
            }
        }

        // Login Page
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AdminLoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                string connStr = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PR_AdminUser_Login";

                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Password", model.Password);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                HttpContext.Session.SetString("AdminID", dr["AdminID"].ToString());
                                HttpContext.Session.SetString("FullName", dr["FullName"].ToString());
                                HttpContext.Session.SetString("Email", dr["Email"].ToString());
                                
                                string city = dr["City"] != DBNull.Value ? dr["City"].ToString() : "";
                                HttpContext.Session.SetString("City", city);

                                TempData["SuccessMessage"] = "Login successful.";
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.ErrorMessage = "Invalid email or password.";
                            }
                        }
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error during login: " + ex.Message;
                return View(model);
            }
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
