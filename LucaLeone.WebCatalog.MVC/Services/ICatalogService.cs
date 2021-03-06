﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LucaLeone.WebCatalog.MVC.Models;

namespace LucaLeone.WebCatalog.MVC.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<Product>> GetCatalogPageAsync(int page, int numElem = 10);
        Task<bool> AddProductAsync(NewProduct newProduct);
        Task<IEnumerable<Product>> SearchProductsAsync(string productName);
        Task<Product> DeleteProductAsync(Guid id);
        Task<Product> EditProductAsync(Product newProduct);
        Task<Product> GetProduct(Guid id);
    }
}