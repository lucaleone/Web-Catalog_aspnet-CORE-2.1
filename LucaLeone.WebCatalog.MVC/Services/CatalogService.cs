using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LucaLeone.WebCatalog.MVC.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LucaLeone.WebCatalog.MVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly string _remoteServiceBaseUrl;
        private readonly HttpClient hc;

        public CatalogService(IConfiguration configuration)
        {
            Configuration = configuration;
            _remoteServiceBaseUrl = Configuration.GetValue<string>("WebCatalog.API_endpoint");
            hc = new HttpClient();
            hc.BaseAddress = new Uri(_remoteServiceBaseUrl);
            hc.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public IConfiguration Configuration { get; }

        // GET api/Catalog?page=4
        public async Task<IEnumerable<Product>> GetCatalogPageAsync(int page, int maxNumElem = 10)
        {
            string catalogPageUri = $"?page={page}&maxNumElem={maxNumElem}";
            var response = await hc.GetAsync(catalogPageUri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<Product>>();
            return products;
        }

        public async Task<Product> GetProduct(Guid id)
        {
            string getProductUri = $"/api/Catalog/GetProduct?id={id}";
            var response = await hc.GetAsync(getProductUri);
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadAsAsync<Product>();

            return product;
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string productName)
        {
            string searchProductUri = $"/api/Catalog/Search?name={productName}";
            var response = await hc.GetAsync(searchProductUri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<Product>>();
            return products;
        }

        public async Task<bool> AddProductAsync(NewProduct newProduct)
        {
            string addProductUri = "/api/Catalog/AddProduct";

            string json = JsonConvert.SerializeObject(newProduct);
            var tmp = new StringContent(json, Encoding.UTF8);
            var response = await hc.PutAsJsonAsync(addProductUri, newProduct);
            response.EnsureSuccessStatusCode();

            return response.IsSuccessStatusCode;
        }

        public async Task<Product> DeleteProductAsync(Guid id)
        {
            string deleteProductUri = $"/api/Catalog/DeleteProduct?id={id}";
            var response = await hc.DeleteAsync(deleteProductUri);
            response.EnsureSuccessStatusCode(); // Throw on error code.
            string dataString = await response.Content.ReadAsStringAsync();
            Product product = JsonConvert.DeserializeObject<Product>(dataString);

            return product;
        }

        public async Task<Product> EditProductAsync(Product editProduct)
        {
            string EditProductUri = $"/api/Catalog/EditProduct?id={editProduct.Id}";
            string json = JsonConvert.SerializeObject(editProduct);
            var tmp = new StringContent(json, Encoding.UTF8);
            var response = await hc.PutAsync(EditProductUri,
                new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode(); // Throw on error code.

            string dataString = await response.Content.ReadAsStringAsync();
            Product product = JsonConvert.DeserializeObject<Product>(dataString);

            return product;
        }
    }
}