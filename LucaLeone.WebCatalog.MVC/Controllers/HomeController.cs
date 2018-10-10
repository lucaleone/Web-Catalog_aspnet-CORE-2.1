using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LucaLeone.WebCatalog.MVC.Models;
using LucaLeone.WebCatalog.MVC.Services;
using LucaLeone.WebCatalog.MVC.Validation;

namespace LucaLeone.WebCatalog.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICatalogService _catalogService;

        public HomeController(ICatalogService catalogService) =>
            _catalogService = catalogService;

        public async Task<IActionResult> Index(int page = 1, int maxNumElem = 10)
        {
            CatalogInputValidation.ValidateGetCatalogInput(ref page, ref maxNumElem);

            var products = await _catalogService.GetCatalogPageAsync(page, maxNumElem);
            var model = new ProductsView
            {
                Products = products,
                Page = page
            };
            return View(model);
        }

        [HttpDelete("{id}")]
        [Route("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid id)
        {
            var products = await _catalogService.DeleteProductAsync(id);
            return View(products);
            //return View("DeleteProduct", products);
            //return BadRequest("The product selected does not exist");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Search([FromQuery] string name = null,
                                                [FromQuery] int? minPrice = null,
                                                [FromQuery] int? maxPrice = null)
        {
            if (CatalogInputValidation.ValidateSearchInput(name, minPrice, maxPrice) == false)
                return BadRequest();

            var products = await _catalogService.SearchProductsAsync(name);
            var model = new ProductsView
            {
                Products = products
            };
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Description page";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Luca Leone";

            return View();
        }

        [HttpGet]
        [Route("AddProduct")]
        public IActionResult AddProduct()
        {
            ViewData["Message"] = "Add Product";

            return View();
        }

        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct(NewProduct newProduct)
        {
            var result = await _catalogService.AddProductAsync(newProduct);
            if (result)
                return RedirectToAction("Index");
            return BadRequest("Add product error");
        }

        [HttpGet]
        [Route("EditProduct/{id}")]
        public async Task<IActionResult> EditProduct([FromRoute] Guid id)
        {
            ViewData["Message"] = "Edit Product";
            var products = await _catalogService.GetProduct(id);
            return View(products);
        }

        [HttpGet]
        [Route("EditProduct")]
        public async Task<IActionResult> EditProduct([FromQuery] Product newProduct)
        {
            var result = await _catalogService.EditProductAsync(newProduct);
            if (result != null)
                return RedirectToAction("Index");
            return BadRequest("Edit product error");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
