using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace LucaLeone.WebCatalog.API.DTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Url]
        public string Photo { get; set; }

        [Required]
#if DEBUG
        // To fix Fixture creation System.OverflowException
        [Range(typeof(decimal), "0,99", "79228162514264337593543950335",
            ErrorMessage = "The maximum price must be greater than 1.")]
#else
        [Range(0.99d, (double) decimal.MaxValue, ErrorMessage = "The maximum price must be greater than 1.")]
#endif
        public decimal Price { get; set; }
    }
}