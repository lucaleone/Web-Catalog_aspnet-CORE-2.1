using System;
using System.ComponentModel.DataAnnotations;
namespace LucaLeone.WebCatalog.API.DTO
{
    public class ProductDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Url]
        public string Photo { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}