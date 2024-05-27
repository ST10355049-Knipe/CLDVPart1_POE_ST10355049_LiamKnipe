//Liam Knipe St10355049
//REFERENCES: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/ , https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-cookies
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio, https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0
// https://www.codeproject.com/Articles/32545/Exploring-Session-in-ASP-Net , https://www.completecsharptutorial.com/asp-net-mvc5/asp-net-mvc-5-httpget-and-httppost-method-with-example.php#:~:text=HTTPGet%20method%20is%20default%20whereas,body%20of%20the%20HTTP%20request.
// https://dev.to/fabriziobagala/logging-in-aspnet-core-6-3mbh , https://stackoverflow.com/questions/11585/clearing-page-cache-in-asp-net
// https://www.c-sharpcorner.com/article/asp-net-core-3-1-authentication-and-authorization/ , https://www.youtube.com/watch?v=8_eMgS6UszY&list=PLIY8eNdw5tW_ZQawyxK0Dd1cZXwcNFWn8&ab_channel=SimpleSnippets
// https://www.tutorialspoint.com/asp.net_mvc/asp.net_mvc_bootstrap.htm , https://www.guru99.com/insert-update-delete-asp-net.html#:~:text=The%20'sql'%20statement%20can%20be,are%20specified%20in%20ASP.Net.


using ST10355049.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;

namespace ST10355049.Controllers
{
    public class ProductController : Controller
    {
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(productTable products)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("MyWork", "ProductDisplay");
            }

            try
            {
                int rowsAffected = await products.InsertProductAsync();

                if (rowsAffected > 0)
                {
                    // Insertion successful
                    // Clear the model state and reset the form fields
                    ModelState.Clear();
                    products = new productTable();

                    // Redirect to the MyWork page
                    return RedirectToAction("MyWork", "ProductDisplay");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create product.");
                    return View("AddProduct", products);
                }
            }
            catch (Exception ex)
            {
                // Log the exception and display an error message
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "An error occurred while creating the product. Error: " + ex.Message);
                return View("AddProduct", products);
            }
        }
    }
}
