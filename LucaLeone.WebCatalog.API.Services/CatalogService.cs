using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LucaLeone.WebCatalog.API.DataAccessors;
using LucaLeone.WebCatalog.API.DTO;
using LucaLeone.WebCatalog.API.Entities;
using Microsoft.EntityFrameworkCore;
using PowerUp;

namespace LucaLeone.WebCatalog.API.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly CatalogContext _context;
        private readonly IMapper _mapper;

        public CatalogService(CatalogContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetCatalogPageAsync(GetCatalogDto getCatalog)
        {
            var elems2Skip = (getCatalog.Page - 1) * getCatalog.MaxNumElem; // skip n pages
            var result = await _context.Products
                                 .OrderByDescending(p => p.LastUpdated)
                                 .Skip(elems2Skip)
                                 .Take(getCatalog.MaxNumElem)
                                 .ToArrayAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(result);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(SearchDto search)
        {
            if(!string.IsNullOrWhiteSpace(search.Name))
                search.Name = search.Name.Trim();
            var result = await _context.Products
                                 .Where(prod => string.IsNullOrEmpty(search.Name) || prod.Name.Contains(search.Name))
                                 .Where(prod => prod.Price >= search.MinPrice)
                                 .Where(prod => search.MaxPrice.IsNull() || prod.Price <= search.MaxPrice.Value)
                                 .OrderByDescending(p => p.LastUpdated)
                                 .ToArrayAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(result);
        }

        public async Task<ProductDto> GetProduct(Guid id)
        {
            var result = await _context.Products
                                 .FirstOrDefaultAsync(prod => prod.Id.Equals(id));
            return _mapper.Map<ProductDto>(result);
        }

        public async Task<Guid> AddProductAsync(ProductDto newProduct)
        {
            var productEntity = _mapper.Map<Product>(newProduct);
            productEntity.Id = new Guid();
            productEntity.CreateDate = DateTime.UtcNow;
            productEntity.LastUpdated = DateTime.UtcNow;
            _context.Products.Add(productEntity);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1 ? productEntity.Id : Guid.Empty;
        }

        public async Task<ProductDto> EditProductAsync(Guid id, ProductDto editProduct)
        {
            var productToEdit = _context.Products.FirstOrDefault(p => p.Id.Equals(id));
            if (productToEdit.IsNotNull())
            {
                productToEdit.Name = editProduct.Name;
                productToEdit.Photo = editProduct.Photo;
                productToEdit.Price = editProduct.Price;
                productToEdit.LastUpdated = DateTime.UtcNow;
                var saveResult = await _context.SaveChangesAsync();
                return saveResult == 1 ? editProduct : null;
            }
            return null;
        }

        public async Task<ProductDto> DeleteProductAsync(Guid id)
        {
            var productToDelete = _context.Products.FirstOrDefault(p => p.Id.Equals(id));
            if (productToDelete.IsNotNull())
            {
                _context.Products.Remove(productToDelete);
                int saveResult = await _context.SaveChangesAsync();
                return saveResult == 1 ? _mapper.Map<ProductDto>(productToDelete) : null;
            }

            return null;
        }

        public async Task<bool> InitDb()
        {
            if (_context.Products.Any())
                return true;

            var baseList = new List<ProductDto>
            {
                new ProductDto
                {
                    Name = "Silver lamp led red, white and green",
                    Photo =
                        @"http://media.4rgos.it/i/Argos/4612676_R_Z001A?$Web$&$DefaultPDP570$",
                    Price = 23.50m
                },
                new ProductDto
                {
                    Name = "Black leather chair, super comfort!",
                    Photo =
                        @"https://www.modernmanhattan.com/image/data/demo/mmh-88535-Zahara-Black-Leather-Club-Chair-5.jpg",
                    Price = 85.35m
                },
                new ProductDto
                {
                    Name = "Wooden desk, the comfort is guaranteed!",
                    Photo =
                        @"https://cdn.shopify.com/s/files/1/0223/2583/products/Weathered_Oak_Kneehole_Desk_RNG063_1024x1024.jpg",
                    Price = 135.99m
                },
                new ProductDto
                {
                    Name = "Rolling stones Poster, only for real rockers",
                    Photo =
                        @"https://upload.wikimedia.org/wikipedia/it/0/04/Logo_Rolling_Stones.jpg",
                    Price = 12.99m
                }
            };
            await _context.Products.AddRangeAsync(_mapper.Map<IEnumerable<Product>>(baseList));
            var saveResult = await _context.SaveChangesAsync();
            return saveResult > 0;
        }

        public async Task<bool> EraseDb()
        {
            var saveResult = await _context.Database.ExecuteSqlCommandAsync("delete from Products");
            return saveResult > 0;
        }
    }
}