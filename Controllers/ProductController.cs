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
