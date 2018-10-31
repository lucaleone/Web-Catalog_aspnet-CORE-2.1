using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using LucaLeone.WebCatalog.API.DataAccessors;
using LucaLeone.WebCatalog.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LucaLeone.WebCatalog.API.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogContext _context;

        public CatalogService(CatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllCatalogAsync()
        {
            return await _context.Products.ToArrayAsync();
        }

        public async Task<IEnumerable<Product>> GetCatalogPageAsync(int page, int maxNumElem = 10)
        {
            var elems2Skip = (page - 1) * maxNumElem; // skip n pages
            return await _context.Products
                                 .OrderByDescending(p => p.LastUpdated)
                                 .Skip(elems2Skip)
                                 .Take(maxNumElem)
                                 .ToArrayAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string productName, int minPrice, int? maxPrice)
        {
            return await _context.Products
                                 .Where(prod => string.IsNullOrEmpty(productName) || prod.Name.Contains(productName))
                                 .Where(prod =>  prod.Price >= minPrice)
                                 .Where(prod =>  prod.Price <= maxPrice.GetValueOrDefault())
                                 .OrderByDescending(p => p.LastUpdated)
                                 .ToArrayAsync();
        }

        public async Task<Product> GetProduct(Guid id)
        {
            return await _context.Products
                                 .Where(prod => prod.Id.Equals(id))
                                 .FirstOrDefaultAsync();
        }

        public async Task<(bool, Product)> AddProductAsync(IProductBuilder newProduct)
        {
            try
            {
                var entity = newProduct.BuildProduct();
                _context.Products.Add(entity);
                await _context.SaveChangesAsync();
                return (true, entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<Product> EditProductAsync(Guid id, NewProduct newProduct)
        {
            var product = _context.Products
                                  .FirstOrDefault(p => p.Id.Equals(id));
            if (product != null)
            {
                product.EditProduct(newProduct.Name, newProduct.Photo, newProduct.Price);
                int saveResult = await _context.SaveChangesAsync();
                return saveResult == 1 ? product : null;
            }

            return null;
        }

        public async Task<Product> DeleteProductAsync(Guid id)
        {
            var product = _context.Products
                                  .FirstOrDefault(p => p.Id.Equals(id));
            if (product != null)
            {
                _context.Products.Remove(product);
                int saveResult = await _context.SaveChangesAsync();
                return saveResult == 1 ? product : null;
            }

            return null;
        }

        public async Task<bool> InitDb()
        {
            if (_context.Products.Any())
                return true;

            _context.Products.Add(new NewProduct
            {
                Name = "Silver lamp led red, white and green",
                Photo = @"http://media.4rgos.it/i/Argos/4612676_R_Z001A?$Web$&$DefaultPDP570$",
                Price = 23.50m
            }.BuildProduct());
            _context.Products.Add(new NewProduct
            {
                Name = "Black leather chair, super comfort!",
                Photo =
                    @"https://www.modernmanhattan.com/image/data/demo/mmh-88535-Zahara-Black-Leather-Club-Chair-5.jpg",
                Price = 85.35m
            }.BuildProduct());
            _context.Products.Add(new NewProduct
            {
                Name = "Wooden desk, the comfort is guaranteed!",
                Photo =
                    @"https://cdn.shopify.com/s/files/1/0223/2583/products/Weathered_Oak_Kneehole_Desk_RNG063_1024x1024.jpg",
                Price = 135.99m
            }.BuildProduct());
            _context.Products.Add(new NewProduct
            {
                Name = "Rolling stones Poster, only for real rockers",
                Photo = @"https://upload.wikimedia.org/wikipedia/it/0/04/Logo_Rolling_Stones.jpg",
                Price = 12.99m
            }.BuildProduct());

            int saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> EraseDb()
        {
            int saveResult = await _context.Database.ExecuteSqlCommandAsync("delete from Products");
            return saveResult > 0;
        }
    }
}