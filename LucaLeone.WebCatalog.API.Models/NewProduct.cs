using System.ComponentModel.DataAnnotations;

namespace LucaLeone.WebCatalog.API.Models
{
    public class NewProduct : IProductBuilder
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Url]
        public string Photo { get; set; }

        [Required]
        [Range(0.99, double.MaxValue)]
        public decimal Price { get; set; }

        public Product BuildProduct()
        {
            return new Product(Name, Photo, Price);
        }
    }
}