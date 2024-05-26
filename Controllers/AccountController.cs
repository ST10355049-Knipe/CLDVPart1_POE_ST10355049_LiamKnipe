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






