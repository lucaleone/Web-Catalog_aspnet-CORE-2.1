using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LucaLeone.WebCatalog.Models;

namespace LucaLeone.WebCatalog.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<Product>> GetAllCatalogAsync();
        Task<IEnumerable<Product>> GetCatalogPageAsync(int page, int maxNumElem = 10);

        Task<IEnumerable<Product>> SearchProductsAsync(string productName,
                                                       int? minPrice = null,
                                                       int? maxPrice = null);

        Task<(bool, Product)> AddProductAsync(IProductBuilder newProduct);
        Task<Product> EditProductAsync(Guid id, NewProduct newProduct);
        Task<Product> DeleteProductAsync(Guid id);
        Task<bool> InitDb();
        Task<bool> eraseDb();
        Task<Product> GetProduct(Guid id);
    }
}