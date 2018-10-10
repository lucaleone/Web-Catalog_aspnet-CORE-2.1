using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LucaLeone.WebCatalog.Data;
using LucaLeone.WebCatalog.Models;
using LucaLeone.WebCatalog.Services;
using LucaLeone.WebCatalog.Validation;
using System.IO;
using System.Net.Http;

namespace LucaLeone.WebCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _context;
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService, CatalogContext context)
        {
            _context = context;
            _catalogService = catalogService;
        }

        /// <summary>
        ///     Search for products that mach the query
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog?page=1
        /// </remarks>
        /// <param name="page">[Optional, default 1] Number of the page (elements to skip)</param>
        /// <param name="maxNumElem">[Optional, default 10]Max amount of products to return</param>
        /// <returns>A list of the matching products</returns>
        /// <response code="200">Returns a list of the matching products</response>
        /// <response code="400">If the query is not valid</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> GetCatalog([FromQuery] int page = 1, int maxNumElem = 10)
        {
            if (InputValidation.ValidateGetCatalogInput(page, ref maxNumElem) == false)
                return BadRequest("Query not valid");

            var Products = await _catalogService.GetCatalogPageAsync(page, maxNumElem);
            return Ok(Products);
        }

        /// <summary>
        ///     Search for products that mach the query
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog/GetProduct?name=lamp&amp;minPrice=10
        /// </remarks>
        /// <param name="name">Id of the Product to search</param>
        /// <param name="minPrice">[Optional] Min price of the product in €</param>
        /// <param name="maxPrice">[Optional] Max price of the product in €</param>
        /// <returns>A list of the matching products</returns>
        /// <response code="200">Returns a list of the matching products</response>
        /// <response code="400">If the query is not valid</response>
        [HttpGet]
        [Route("[action]")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Search([FromQuery] string name = null,
                                                [FromQuery] int? minPrice = null,
                                                [FromQuery] int? maxPrice = null)
        {
            if (InputValidation.ValidateSearchInput(ref name, minPrice, maxPrice) == false)
                return BadRequest("Query not valid");

            var Products = await _catalogService.SearchProductsAsync(name, minPrice, maxPrice);
            return Ok(Products);
        }

        /// <summary>
        ///     Search for a product by Id
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog/GetProduct?id=8916c69a-8041-4768-8e0d-a391361ff732
        /// </remarks>
        /// <param name="id">Id of the Product to search</param>
        /// <returns>A Product with matchind Id</returns>
        /// <response code="200">Returns a Product with matchind Id</response>
        /// <response code="400">If the Id does not exist</response>
        [HttpGet]
        [Route("[action]")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(Guid), 400)]
        public async Task<IActionResult> GetProduct([FromQuery] Guid id)
        {
            var Products = await _catalogService.GetProduct(id);
            if (Products != null)
                return Ok(Products);
            return BadRequest(id);
        }

        /// <summary>
        ///     Serves a CSV file with the entire catalog
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog/Export
        /// </remarks>
        /// <returns>A CSV file with the entire catalog</returns>
        /// <response code="200">Returns a CSV file with the entire catalog</response>
        /// <response code="400">If the file does not exist</response>
        [HttpGet]
        [Route("[action]")]
        [Produces("text/csv")]
        [ProducesResponseType(typeof(File), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Export()
        {
            var path = Path.Combine(@"Export", "Products.csv");
            if (!System.IO.File.Exists(path))
                return BadRequest("File does not exist");
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return File(
                    fileContents: await new StreamContent(stream).ReadAsByteArrayAsync(),
                    contentType: "text/csv",
                    // By setting a file download name the framework will
                    // automatically add the attachment Content-Disposition header
                    fileDownloadName: "ProductsExport.csv"
                );
            }
        }

        /// <summary>
        ///     Add a Product to the catalog
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     POST api/Catalog/AddProduct
        ///     {"name":"new product name", "price":"52.99",
        ///     "photo":"https://www.bhphotovideo.com/images/images2500x2500/dell_u2417h_24_16_9_ips_1222870.jpg"}
        /// </remarks>
        /// <param name="newProduct">Product to add</param>
        /// <returns>A newly-created Product</returns>
        /// <response code="201">Returns the newly-created Product</response>
        /// <response code="400">If the Product is not valid</response>
        [HttpPost]
        [Route("[action]")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct([FromBody] NewProduct newProduct)
        {
            if (InputValidation.ValidateProductInput(newProduct) == false)
                return BadRequest("New product not valid");
            (bool result, Product product) = await _catalogService.AddProductAsync(newProduct);
            if (result)
                return StatusCode(StatusCodes.Status201Created, product);
            return BadRequest("New product not added");
        }

        /// <summary>
        ///     Edit a Product in the catalog
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     PUT api/Catalog/EditProduct?id=8916c69a-8041-4768-8e0d-a391361ff732
        ///     {"name":"test modified", "price":"52.99", "photo":"test2new.png"}
        /// </remarks>
        /// <param name="id">Id of the Product to edit</param>
        /// <param name="newProduct">Customized product</param>
        /// <returns>The updated Product</returns>
        /// <response code="200">Returns the updated Product</response>
        /// <response code="400">If the Id does not exist in the Catalog</response>
        [HttpPut]
        [Route("[action]")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> EditProduct([FromQuery] Guid id,
                                                     [FromBody] NewProduct newProduct)
        {
            if (InputValidation.ValidateProductInput(newProduct) == false)
                return BadRequest("New product not valid");
            Product result = await _catalogService.EditProductAsync(id, newProduct);
            if (result != null)
                return Ok(result);
            return BadRequest("Error: ID doesn't exist");
        }

        /// <summary>
        ///     Delete a Product in the catalog
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     DELETE api/Catalog/DeleteProduct?id=8916c69a-8041-4768-8e0d-a391361ff732
        /// </remarks>
        /// <param name="id">Id of the Product to delete</param>
        /// <returns>The deleted Product</returns>
        /// <response code="200">Returns the deleted Product</response>
        /// <response code="400">If the Id does not exist in the Catalog</response>
        [HttpDelete]
        [Route("[action]")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid id)
        {
            Product result = await _catalogService.DeleteProductAsync(id);
            if (result != null)
                return Ok(result);
            return BadRequest("Error: ID doesn't exist");
        }

        /// <summary>
        ///     DEV ONLY: initialize the Product table
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog/initDb
        /// </remarks>
        /// <returns>A string with the result of the DB initialization</returns>
        /// <response code="200">Returns a string with the result of the DB initialization</response>
        [HttpGet]
        [Route("[action]")]
        [Produces("text/plain")]
        public async Task<IActionResult> InitDb()
        {
            await _catalogService.InitDb();
            return Ok("DB init done! go back and refresh");
        }

        /// <summary>
        ///     DEV ONLY: erase the content of the Product table
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog/eraseDb
        /// </remarks>
        /// <returns>A string with the result of the DB cleaning</returns>
        /// <response code="200">Returns a string with the result of the DB cleaning</response>
        [HttpGet]
        [Route("[action]")]
        [Produces("text/plain")]
        public async Task<IActionResult> EraseDb()
        {
            bool res = await _catalogService.eraseDb();
            return Ok(res
                ? "Database cleaned, go back and refresh"
                : "Database was already empty, go back and refresh");
        }
    }
}