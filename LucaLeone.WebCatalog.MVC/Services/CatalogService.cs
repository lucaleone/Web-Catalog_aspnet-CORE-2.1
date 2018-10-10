using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LucaLeone.WebCatalog.MVC.Models;
using Newtonsoft.Json;

namespace LucaLeone.WebCatalog.MVC.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly string _remoteServiceBaseUrl;
        private readonly HttpClient hc;

        public CatalogService()
        {
            _remoteServiceBaseUrl = @"https://localhost:44303/api/catalog";
            hc = new HttpClient
            {
                BaseAddress = new Uri(_remoteServiceBaseUrl)
            };
            hc.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET api/Catalog?page=4
        public async Task<IEnumerable<Product>> GetCatalogPageAsync(int page, int maxNumElem = 10)
        {
            string catalogPageUri = $"?page={page}&maxNumElem={maxNumElem}";
            var response = await hc.GetAsync(catalogPageUri);
            response.EnsureSuccessStatusCode(); // Throw on error code.
            string dataString = await response.Content.ReadAsStringAsync();
            IEnumerable<Product> products =
                JsonConvert.DeserializeObject<IEnumerable<Product>>(dataString);

            return products;
        }

        public async Task<Product> GetProduct(Guid id)
        {
            string getProductUri = $"/api/Catalog/GetProduct?id={id}";
            var response = await hc.GetAsync(getProductUri);
            response.EnsureSuccessStatusCode(); // Throw on error code.
            string dataString = await response.Content.ReadAsStringAsync();
            Product products = JsonConvert.DeserializeObject<Product>(dataString);

            return products;
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(
            string productName,
            int? minPrice = null,
            int? maxPrice = null)
        {
            string searchProductUri =
                $"/api/Catalog/Search?name={productName}&minPrice={minPrice}&maxPrice={maxPrice}";
            var response = await hc.GetAsync(searchProductUri);
            response.EnsureSuccessStatusCode();
            string dataString = await response.Content.ReadAsStringAsync();
            IEnumerable<Product> products =
                JsonConvert.DeserializeObject<IEnumerable<Product>>(dataString);

            return products;
        }

        public async Task<bool> AddProductAsync(NewProduct newProduct)
        {
            string addProductUri = "/api/Catalog/AddProduct";

            string json = JsonConvert.SerializeObject(newProduct);
            var tmp = new StringContent(json, Encoding.UTF8);
            var response = await hc.PostAsync(addProductUri,
                new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode(); // Throw on error code.

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