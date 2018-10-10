using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LucaLeone.WebCatalog.Data;
using LucaLeone.WebCatalog.Models;
using FileHelpers;
using Microsoft.EntityFrameworkCore;

namespace LucaLeone.WebCatalog.Services
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
            int elem2Skip = (page - 1) * maxNumElem; // skip n pages
            return await _context.Products
                                 .OrderByDescending(p => p.LastUpdated)
                                 .Skip(elem2Skip)
                                 .Take(maxNumElem)
                                 .ToArrayAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(
            string productName,
            int? minPrice = null,
            int? maxPrice = null)
        {
            return await _context.Products
                                 .Where(prod =>
                                     string.IsNullOrEmpty(productName) ||
                                     prod.Name.Contains(productName))
                                 .Where(prod => minPrice == null || prod.Price >= minPrice)
                                 .Where(prod => maxPrice == null || prod.Price <= maxPrice)
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
            Product entity = newProduct.BuildProduct();

            _context.Products.Add(entity);
            int saveResult = await _context.SaveChangesAsync();
            UpdateExportProducts(entity);
            return (saveResult == 1, entity);
        }

        public async Task<Product> EditProductAsync(Guid id, NewProduct newProduct)
        {
            var product = _context.Products
                                  .Where(p => p.Id.Equals(id))
                                  .FirstOrDefault();
            if (product != null)
            {
                product.EditProduct(newProduct.Name, newProduct.Photo, newProduct.Price);
                int saveResult = await _context.SaveChangesAsync();
                GenerateExportProducts();
                return saveResult == 1 ? product : null;
            }

            return null;
        }

        public async Task<Product> DeleteProductAsync(Guid id)
        {
            var product = _context.Products
                                  .Where(p => p.Id.Equals(id))
                                  .FirstOrDefault();
            if (product != null)
            {
                _context.Products.Remove(product);
                int saveResult = await _context.SaveChangesAsync();
                GenerateExportProducts();
                return saveResult == 1 ? product : null;
            }

            return null;
        }

        public async Task<bool> eraseDb()
        {
            int saveResult = await _context.Database.ExecuteSqlCommandAsync("delete from Products");
            GenerateExportProducts();
            return saveResult > 0;
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
                Name = "Black leather chair, super confort!",
                Photo =
                    @"https://www.modernmanhattan.com/image/data/demo/mmh-88535-Zahara-Black-Leather-Club-Chair-5.jpg",
                Price = 85.35m
            }.BuildProduct());
            _context.Products.Add(new NewProduct
            {
                Name = "Wooden desk, the confort is guaranteed!",
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
            GenerateExportProducts();
            return saveResult == 1;
        }

        private void GenerateExportProducts()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Export", "Products.csv");
            var engine = new FileHelperAsyncEngine<Product>(Encoding.UTF8);
            engine.HeaderText = engine.GetFileHeader();
            PrepareExportFile();
            using (engine.BeginWriteFile(filePath))
            {
                engine.WriteNexts(_context.Products);
            }
        }

        private void UpdateExportProducts(Product prod)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Export", "Products.csv");
            var engine = new FileHelperAsyncEngine<Product>(Encoding.UTF8);
            PrepareExportFile();
            using (engine.BeginAppendToFile(filePath))
            {
                engine.WriteNext(prod);
            }
        }

        private void PrepareExportFile()
        {
            var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "Export");
            var filePath = Path.Combine(dirPath, "Products.csv");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            if (!File.Exists(filePath))
                using (var sw = File.CreateText(filePath))
                    sw.WriteLine(@"Id,Name,Photo,Price,LastUpdated");
        }
    }
}

// FUTURE IMPLEMENTATION
//private static void GenerateExportProducts(Product prod, FileMode fileMode = FileMode.Append)
//{
//    var path = Path.Combine(@"Export", "Products.csv");
//    using (var engine = new FileHelperAsyncEngine<Product>())
//    {
//        engine.HeaderText = engine.GetFileHeader();
//        switch (fileMode)
//        {
//            case FileMode.Append:
//                engine.BeginAppendToFile(@"Export\Products.csv");
//                break;
//            case FileMode.Create:
//            case FileMode.CreateNew:
//                engine.BeginWriteFile(@"Export\Products.csv");
//                break;
//            default:
//                return;
//        }
//        engine.WriteNext(prod);
//    }
//}