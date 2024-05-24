using Microsoft.AspNetCore.Mvc;
using ST10355049.Models;
using System.Data.SqlClient;

namespace ST10355049.Controllers
{
    public class ProductDisplayController : Controller
    {
        

        private readonly ILogger<ProductDisplayController> _logger;

        public ProductDisplayController(ILogger<ProductDisplayController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> MyWork()
        {
            try
            {
                // Fetch all products from the database
                List<productTable> products = await productTable.GetAllProductsAsync();

                // Check if products are null or empty
                if (products == null || !products.Any())
                {
                    _logger.LogWarning("No products were fetched from the database.");
                    return View("Error", "No products were fetched from the database.");
                }

                // Pass the products to the view
                return View("~/Views/Home/MyWork.cshtml", products);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while fetching products from the database.");

                // Return an error view
                return View("Error", "An error occurred while fetching products from the database.");
            }
        }

        public async Task<IActionResult> Index()
        {
            // Get all products from the database
            List<productTable> products = await productTable.GetAllProductsAsync();

            // Return the Index view with all products
            return View(products);
        }
    }
}
