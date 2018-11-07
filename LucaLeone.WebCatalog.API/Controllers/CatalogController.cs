using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LucaLeone.WebCatalog.API.DTO;
using LucaLeone.WebCatalog.API.Entities;
using LucaLeone.WebCatalog.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PowerUp;
using PowerUp.Web;

namespace LucaLeone.WebCatalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly ILogger _logger;


        public CatalogController(ICatalogService catalogService,
                                 ILogger<CatalogController> logger)
        {
            catalogService.ThrowIfNull(nameof(catalogService));
            logger.ThrowIfNull(nameof(logger));

            _catalogService = catalogService;
            _logger = logger;

            _logger.LogThisMethod();
        }


        /// <summary>
        ///     Search for products that match the query.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog?page=1
        /// </remarks>
        /// <returns>A list of the matching products</returns>
        /// <response code="200">Returns a list of the matching products.</response>
        /// <response code="400">If the query is not valid.</response>
        [HttpGet]
        [Produces(MediaType.ApplicationJson)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCatalog(
            [FromQuery] GetCatalogDto getCatalog)
        {
            _logger.LogThisMethod();
            var products = await _catalogService.GetCatalogPageAsync(getCatalog);
            return Ok(products);
        }

        /// <summary>
        ///     Search for products that match the query.
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog/GetProduct?name=lamp&amp;minPrice=10
        /// </remarks>
        /// <returns>A list of the matching products.</returns>
        /// <response code="200">Returns a list of the matching products.</response>
        /// <response code="400">If the query is not valid.</response>
        [HttpGet]
        [Route("[action]")]
        [Produces(MediaType.ApplicationJson)]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] SearchDto search)
        {
            _logger.LogThisMethod();
            var products = await _catalogService.SearchProductsAsync(search);
            return Ok(products);
        }

        /// <summary>
        ///     Search for a product by Id
        /// </summary>
        /// <remarks>
        ///     Sample request:
        ///     GET api/Catalog/GetProduct?id=8916c69a-8041-4768-8e0d-a391361ff732
        /// </remarks>
        /// <param name="id">Id of the Product to search</param>
        /// <returns>A Product with matching Id</returns>
        /// <response code="200">Returns a Product with matching Id</response>
        /// <response code="400">If the Id does not exist</response>
        [HttpGet]
        [Route("[action]", Name = nameof(GetProduct))]
        [Produces(MediaType.ApplicationJson)]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct([FromQuery] Guid id)
        {
            _logger.LogThisMethod();
            var product = await _catalogService.GetProduct(id);
            if (product.IsNull())
                return NotFound(id);
            return Ok(product);
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
        [HttpPut]
        [Route("[action]")]
        [Produces(MediaType.ApplicationJson)]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto newProduct)
        {
            _logger.LogThisMethod();
            var idProductAdded = await _catalogService.AddProductAsync(newProduct);
            var uri = Url.Link(nameof(GetProduct), new { id = idProductAdded });
            return Created(uri, newProduct);
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
        /// <param name="editProduct">Product edited</param>
        /// <returns>The updated Product</returns>
        /// <response code="200">Returns the updated Product</response>
        /// <response code="400">If the Id does not exist in the Catalog</response>
        [HttpPut]
        [Route("[action]")]
        [Produces(MediaType.ApplicationJson)]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditProduct([FromQuery] Guid id,
                                                     [FromBody] ProductDto editProduct)
        {
            _logger.LogThisMethod();
            var productEdited = await _catalogService.EditProductAsync(id, editProduct);
            if (productEdited.IsNull())
                return NotFound(id);
            return Ok(productEdited);
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
        [Produces(MediaType.ApplicationJson)]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteProduct([FromQuery] Guid id)
        {
            _logger.LogThisMethod();
            var productDeleted = await _catalogService.DeleteProductAsync(id);
            if (productDeleted.IsNull())
                return BadRequest();
            return Ok(productDeleted);
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
        [Produces(MediaType.TextPlain)]
        public async Task<IActionResult> InitDb()
        {
            _logger.LogThisMethod();
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
        [Produces(MediaType.TextPlain)]
        public async Task<IActionResult> EraseDb()
        {
            _logger.LogThisMethod();
            var res = await _catalogService.EraseDb();
            return Ok(res
                ? "Database cleaned, go back and refresh"
                : "Database was already empty, go back and refresh");
        }
    }
}