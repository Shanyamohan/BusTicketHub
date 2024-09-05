using Demo2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using BusTicketHub.Models;
namespace Demo2.Controllers
{

    public class HomeController : Controller
    {
        public readonly string _conStr = "Server=192.168.0.23,1427;Initial Catalog=interns;Integrated Security=False;user id=interns;password=Wel#123@Team;TrustServerCertificate=True;";
        [HttpPost]
        public IActionResult LoginVarifcation(Login login)
        {
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                con.Open();
                string cmdText = "SELECT COUNT(*) FROM Bus_Customer_Registration WHERE phone_no = @PhoneNo AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(cmdText, con))
                {
                    cmd.Parameters.AddWithValue("@PhoneNo", login.phone);
                    cmd.Parameters.AddWithValue("@Password", login.password);

                    int result = (int)cmd.ExecuteScalar();

                    if (result > 0)
                    {
                        return Ok(); // Or any appropriate action
                    }
                }
            }

            // If no match is found, redirect to HomePage
            return RedirectToAction("HomePage");

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public void ToRegister()
        {
            RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Index1(Customer model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(_conStr))
                    {
                        con.Open(); // Ensure the connection is open before executing the command
                        var cmdText = "INSERT INTO Bus_Customer_Registration (name, email, phone_no, password) VALUES (@Name, @Email, @PhoneNo, @Password)";

                        using (var command = new SqlCommand(cmdText, con))
                        {
                            command.Parameters.AddWithValue("@Name", model.name);
                            command.Parameters.AddWithValue("@Email", model.email);
                            command.Parameters.AddWithValue("@PhoneNo", model.phone);
                            command.Parameters.AddWithValue("@Password", model.password);

                            int rowsAffected = command.ExecuteNonQuery(); // Execute the command after ensuring the connection is open
                            if (rowsAffected > 0)
                            {
                                return RedirectToAction("HomePage"); // Redirect to a success page or action
                            }
                            else
                            {
                                ModelState.AddModelError("", "An error occurred while saving the data.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }


            return View(model);
        }

        public IActionResult HomePage()
        {
            return View();
        }
       

    }
}
