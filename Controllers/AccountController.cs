//Liam Knipe St10355049
//REFERENCES: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ , https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-cookies
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio, https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
// https://www.codeproject.com/Articles/32545/Exploring-Session-in-ASP-Net , https://www.completecsharptutorial.com/asp-net-mvc5/asp-net-mvc-5-httpget-and-httppost-method-with-example.php#:~:text=HTTPGet%20method%20is%20default%20whereas,body%20of%20the%20HTTP%20request.
// https://dev.to/fabriziobagala/logging-in-aspnet-core-6-3mbh , https://stackoverflow.com/questions/11585/clearing-page-cache-in-asp-net
// https://www.c-sharpcorner.com/article/asp-net-core-3-1-authentication-and-authorization/ , https://www.youtube.com/watch?v=8_eMgS6UszY&list=PLIY8eNdw5tW_ZQawyxK0Dd1cZXwcNFWn8&ab_channel=SimpleSnippets
// https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_bootstrap.htm , https://www.guru99.com/insert-update-delete-asp-net.html#:~:text=The%20'sql'%20statement%20can%20be,are%20specified%20in%20ASP.Net.

using Microsoft.AspNetCore.Mvc;
using ST10355049.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudDevelopment.Models;
using Microsoft.AspNetCore.Http;

namespace ST10355049.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult SignUp()
        {
            return View("SignUp");
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(userTable userModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int rowsAffected = await userModel.InsertUserAsync();
                    if (rowsAffected > 0)
                    {
                        // User created successfully, sign them in and redirect to the "MyWork" page
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userModel.Email),
                            // Add other claims as needed
                        };

                        var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("MyWork", "ProductDisplay");
                    }
                    else
                    {
                        // User creation failed, handle the error
                        ModelState.AddModelError("", "Failed to create user.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // Log the exception details for debugging
                    throw; // Re-throw the exception for further handling
                }
            }

            // If the model state is invalid, return the view with errors
            return View(userModel);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            LoginModel loginModel = new LoginModel();
            int? userId = loginModel.ValidateUser(email, password);

            if (userId.HasValue)
            {
                // User authenticated successfully, sign them in and redirect to the "MyWork" page
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
        };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("MyWork", "ProductDisplay");
            }
            else
            {
                // User authentication failed, show an error message
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }
        }

    }
}






