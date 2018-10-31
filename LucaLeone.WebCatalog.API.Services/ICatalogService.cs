using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LucaLeone.WebCatalog.API.Models;

namespace LucaLeone.WebCatalog.API.Services
{
    public interface ICatalogService
    {
        Task<Product> GetProduct(Guid id);
        Task<IEnumerable<Product>> GetAllCatalogAsync();
        Task<IEnumerable<Product>> GetCatalogPageAsync(int page, int maxNumElem);

        Task<IEnumerable<Product>> SearchProductsAsync(string productName,
                                                       int minPrice,
                                                       int? maxPrice);

        Task<(bool, Product)> AddProductAsync(IProductBuilder newProduct);
        Task<Product> EditProductAsync(Guid id, NewProduct newProduct);
        Task<Product> DeleteProductAsync(Guid id);
        Task<bool> InitDb();
        Task<bool> EraseDb();
    }
}