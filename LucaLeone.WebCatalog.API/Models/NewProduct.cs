namespace LucaLeone.WebCatalog.Models
{
    public class NewProduct : IProductBuilder
    {
        public string Name { get; set; }
        public string Photo { get; set; }
        public decimal Price { get; set; }

        public Product BuildProduct()
        {
            return new Product(Name, Photo, Price);
        }
    }
}