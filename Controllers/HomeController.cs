using Demo2.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using BusTicketHub.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;


namespace Demo2.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public readonly string _conStr = "Server=192.168.0.23,1427;Initial Catalog=interns;Integrated Security=False;user id=interns;password=Wel#123@Team;TrustServerCertificate=True;";
        [HttpPost]
        
        public IActionResult LoginVarifcation(Login login)
        {
          

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                con.Open();
                string cmdText = "SELECT * FROM Bus_Customer_Registration WHERE phone_no = @PhoneNo AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(cmdText, con))
                {
                    cmd.Parameters.AddWithValue("@PhoneNo", login.phone);
                    cmd.Parameters.AddWithValue("@Password", login.password);

                    var result = cmd.ExecuteScalar();

                    if (result!=null)
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                new Claim(ClaimTypes.Name,login.phone )
                            }),
                            Expires = DateTime.UtcNow.AddHours(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                            Issuer = _configuration["Jwt:Issuer"],
                            Audience = _configuration["Jwt:Issuer"]
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        var tokenString = tokenHandler.WriteToken(token);

                        // Store the JWT token in a cookie
                        Response.Cookies.Append("BusHubCookie", tokenString, new CookieOptions
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax,
                            Secure = true // Set to true if using HTTPS
                        });

                        return Ok(new { Token = tokenString });
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
