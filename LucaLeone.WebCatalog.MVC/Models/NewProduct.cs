using System.ComponentModel.DataAnnotations;

namespace LucaLeone.WebCatalog.MVC.Models
{
    public class NewProduct
    {
        [Required]
        public string Name { get; set; }

        public string Photo { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}