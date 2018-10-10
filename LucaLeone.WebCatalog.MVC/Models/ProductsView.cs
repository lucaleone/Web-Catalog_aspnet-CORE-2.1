using System.Collections.Generic;

namespace LucaLeone.WebCatalog.MVC.Models
{
    public class ProductsView
    {
        public IEnumerable<Product> Products { get; set; }
        public int Page { get; set; } = 1;
    }
}